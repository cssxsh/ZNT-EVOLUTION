using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MonoMod.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
// ReSharper disable Unity.PreferAddressByIdToGraphicsParams
namespace ZNT.Evolution.Core;

internal static class DebugPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SteamManager), "DeleteSteamAppId")]
    public static bool DeleteSteamAppId(SteamManager __instance) => false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Material), "mainTexture", MethodType.Getter)]
    public static bool GetMainTexture(Material __instance) => __instance.HasProperty("_MainTex");

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(JValue), "WriteTo")]
    [HarmonyPatch(typeof(JsonWriter), "WriteToken", typeof(JsonReader), typeof(int))]
    public static IEnumerable<CodeInstruction> WriteToken(IEnumerable<CodeInstruction> instructions)
    {
        var _ToInt64 = AccessTools.Method(
            typeof(Convert), nameof(Convert.ToInt64), [typeof(object), typeof(IFormatProvider)]);
        var _Write_long = AccessTools.Method(
            typeof(JsonWriter), nameof(JsonWriter.WriteValue), [typeof(long)]);
        var _Write_object = AccessTools.Method(
            typeof(JsonWriter), nameof(JsonWriter.WriteValue), [typeof(object)]);
        foreach (var instruction in instructions)
        {
            if (instruction.OperandIs(_ToInt64))
            {
                yield return instruction.Clone(OpCodes.Pop);
            }
            else if (instruction.OperandIs(_Write_long))
            {
                yield return instruction.Clone(_Write_object);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(I2.Loc.LocalizationManager), "GetTermTranslation")]
    public static string GetTermTranslation(string __result, string Term) => __result ?? Term;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EndGameCondition), "Start")]
    public static void Start(EndGameCondition __instance)
    {
        if (ComponentSingleton<LevelSettings>.Instance.LevelType is LevelType.Cutscene) return;
        ComponentSingleton<LevelSettings>.Instance.Challenge.Initialize();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AchievementManager), "OnCreate")]
    public static void OnCreate(AchievementManager __instance)
    {
        __instance.enabled = SteamManager.Initialized && SteamManager.Instance.GetUserName() is not "Goldberg";
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Trigger), "OnCreate")]
    public static void OnCreate(Trigger __instance)
    {
        __instance.Detection ??= __instance.GetComponent<TriggerDetection>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SpawnPoint), "Start")]
    public static void Start(SpawnPoint __instance)
    {
        __instance.RandomSeed = __instance.GetInstanceID();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Character), "OnVisionLost")]
    public static bool OnVisionLost(GameObject target) => target is not null;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Moveable), "UpdateIsGrounded")]
    public static void UpdateIsGrounded(Moveable __instance, out LayerMask __state)
    {
        __state = __instance.GroundLayers;
        if (__instance.State is MoveableState.Climbing or MoveableState.Stepping or MoveableState.StartClimbing) return;
        var mask = LayerMask.GetMask("Stairs", "Stairs Top");
        var hit = Physics2D.RaycastNonAlloc(
            origin: __instance.Body.position,
            direction: Vector2.down,
            results: DetectionHelper.CastCheck,
            distance: 0.9f,
            layerMask: mask) > 0;
        if (!hit || __instance.MatchOneWay(DetectionHelper.CastCheck[0].collider, Vector2.up)) return;
        __instance.SetGroundLayers(__state & ~mask);
    }

    [HarmonyFinalizer]
    [HarmonyPatch(typeof(Moveable), "UpdateIsGrounded")]
    public static void UpdateIsGrounded(Moveable __instance, LayerMask __state)
    {
        __instance.SetGroundLayers(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(TankBehaviour), "AfterStepping")]
    [HarmonyPatch(typeof(CharacterBehaviour), "AfterStepping")]
    public static void AfterStepping(CharacterBehaviour __instance, bool move, out LayerMask __state)
    {
        __state = __instance.Mover.GroundLayers;
        if (!move) return;
        var mask = LayerMask.GetMask("Stairs", "Stairs Top");
        var hit = Physics2D.RaycastNonAlloc(
            origin: __instance.Body.position + Vector2.up * (__instance is TankBehaviour ? 1.2f : 0.85f),
            direction: Vector2.down,
            results: DetectionHelper.CastCheck,
            distance: __instance is TankBehaviour ? 1.5f : 1.0f,
            layerMask: mask) > 0;
        if (!hit || __instance.Mover.MatchOneWay(DetectionHelper.CastCheck[0].collider, Vector2.up)) return;
        __instance.Mover.SetGroundLayers(__state & ~mask);
    }

    [HarmonyFinalizer]
    [HarmonyPatch(typeof(TankBehaviour), "AfterStepping")]
    [HarmonyPatch(typeof(CharacterBehaviour), "AfterStepping")]
    public static void AfterStepping(CharacterBehaviour __instance, LayerMask __state)
    {
        __instance.Mover.SetGroundLayers(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Moveable), "SetSpeed")]
    public static void SetSpeed(Moveable __instance)
    {
        __instance.UpdateIsGrounded();
        if (!__instance.IsGrounded) return;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (__instance.State)
        {
            case MoveableState.Jumping:
            case MoveableState.JumpFalling:
            case MoveableState.Falling:
            case MoveableState.Pushed:
                __instance.HitGround(0.0f);
                break;
            default:
                return;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Health), "ProxyTarget", MethodType.Setter)]
    public static void SetProxyTarget(Health __instance, Health value)
    {
        Traverse.Create(__instance).Field<bool>("isProxy").Value = value is not null;
        Traverse.Create(__instance).Field<string>("proxyId").Value ??= __instance.name;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CharacterAsset), "SetHealth")]
    public static bool SetHealth(CharacterAsset __instance, Character character)
    {
        var health = character.Behaviour.Health;
        health.MaxHp = health.Hp = __instance.Hp;
        health.Invincible = __instance.Invincible;
        health.DamageMultipliers.Clear();
        health.DamageMultipliers.AddRange(__instance.DamageMultipliers);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SentryGunAsset), "SetHealth")]
    public static bool SetHealth(SentryGunAsset __instance, SentryGunBehaviour behaviour)
    {
        var health = behaviour.Health;
        health.MaxHp = health.Hp = __instance.Hp;
        health.Invincible = __instance.Invincible;
        health.DamageMultipliers.Clear();
        health.DamageMultipliers.AddRange(__instance.DamageMultipliers);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HumanBehaviour), "AttackTarget")]
    public static bool AttackTarget(HumanBehaviour __instance, bool moveToTarget, Transform target)
    {
        __instance.Mover.UpdateIsGrounded();
        if (moveToTarget || !__instance.CanAttack()) return true;
        __instance.SetTarget(target);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HumanBehaviour), "OnStateChanged")]
    public static void OnStateChanged(HumanBehaviour __instance)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (__instance.PreviousState)
        {
            case BehaviourState.Alerted:
            {
                if (__instance.IgnoreHumanAlertTimer.Started) break;
                __instance.IgnoreHumanAlertTimer.Start(__instance.AlertedTimer.Duration);
                __instance.AlertReporter.ReportAlertEnd();
            }
                break;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch("CharacterBehaviour+ResetCharacterBehaviour, Assembly-CSharp", "Reset")]
    public static void Reset(CharacterBehaviour component)
    {
        component.SensesIgnored = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch("PhysicObjectBehaviour+ResetPhysicBarrelBehaviour, Assembly-CSharp", "Reset")]
    public static void Reset(PhysicObjectBehaviour component)
    {
        component.Exploded = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WeatherRain), "CreateEffect")]
    public static bool CreateEffect(WeatherRain __instance) => __instance.RainEffect is null;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WeatherRain), "OnEditorClose")]
    public static void OnEditorClose(WeatherRain __instance)
    {
        var rain = __instance.RainEffect;
        rain?.UpdateSettings(
            __instance.Intensity,
            __instance.Length,
            __instance.Angle,
            __instance.Speed,
            __instance.Density);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WeatherRain), "OnDestroy")]
    public static void OnDestroy(WeatherRain __instance)
    {
        var rain = __instance.RainEffect;
        rain?.gameObject.SetActive(false);
        __instance.RainEffect = null;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PathologicalGames.PrefabPool), "nameInstance")]
    public static void AddPoolRetriever(Transform instance)
    {
        _ = instance.gameObject.GetComponentSafe<PoolRetriever>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(
        typeof(AbstractPoolManager<GamePoolManager>), "Spawn",
        typeof(Transform), typeof(Transform), typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(bool))]
    public static void Spawn(
        AbstractPoolManager<GamePoolManager> __instance, Transform __result,
        Transform prefab, Transform parent, Vector3 position, Quaternion rotation, bool receiveDespawn, bool cleanName)
    {
        if (cleanName) __result.name = $"{prefab.name}({__instance.PoolName})";
        if (__instance.ExecutionMode.HasAny(Execution.SceneMode)) return;
        if (rotation == default) rotation = Quaternion.identity;
        __result.SetParent(parent, true);
        __result.localPosition = position;
        __result.localRotation = rotation;
        __result.BroadcastMessage(methodName: "OnSpawned", options: SendMessageOptions.DontRequireReceiver);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AbstractPoolManager<GamePoolManager>), "Despawn", typeof(Transform))]
    public static void Despawn(AbstractPoolManager<GamePoolManager> __instance, Transform despawn)
    {
        if (despawn is null) return;
        if (__instance.ExecutionMode.HasAny(Execution.SceneMode)) return;
        despawn.SetParent(null, true);
        despawn.BroadcastMessage(methodName: "OnDespawned", options: SendMessageOptions.DontRequireReceiver);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ObjectSettings), "CopyObject")]
    public static bool CopyObject(ObjectSettings __instance, Rotorz.Tile.TileIndex ti)
    {
        var position = __instance.transform.position;
        if (__instance.Type is not ObjectSettings.ElementType.Brush) return true;
        var element = __instance.Element;
        var level = __instance.LevelManager;
        var system = __instance.TileSystem;
        if (system.GetTileOrNull(ti)?.gameObject == __instance.gameObject) return false;
        level.PaintTile(
            system: system,
            element: element,
            index: ti,
            paintShape: Toolbox.PaintShape.Square,
            paintSize: 1U,
            refreshSurrounding: true);
        var tile = system.GetTileOrNull(ti);
        if (tile is null) return false;
        __instance.gameObject.CopyTo(tile.gameObject);
        __instance.OnCopy?.Invoke(tile.gameObject, __instance.Type is ObjectSettings.ElementType.Brush);
        tile.gameObject.BroadcastMessage(
            methodName: "ObjectMovedInEditor",
            parameter: position,
            options: SendMessageOptions.DontRequireReceiver);
        return false;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Framework.Events.SignalReceiver), "GetType")]
    public static IEnumerable<CodeInstruction> GetType(IEnumerable<CodeInstruction> instructions)
    {
        var Type_GetType = AccessTools.Method(
            typeof(Type), nameof(Type.GetType), [typeof(string)]);
        var AccessTools_TypeByName = AccessTools.Method(
            typeof(AccessTools), nameof(AccessTools.TypeByName), [typeof(string)]);
        foreach (var instruction in instructions)
        {
            if (instruction.OperandIs(Type_GetType))
            {
                yield return instruction.Clone(AccessTools_TypeByName);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Framework.Events.SignalReceiver), "Initialize")]
    public static void Initialize(Framework.Events.SignalReceiver __instance)
    {
        if (__instance.ComponentName is null or "" || __instance.MethodName is null or "") return;
        Traverse.Create(__instance).Field<bool>("methodHasParam").Value |= __instance.MethodName.EndsWith(" (param)");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SignalReceiverLinker), "OnAwake")]
    public static void OnAwake(SignalReceiverLinker __instance)
    {
        __instance.ExcludedComponents ??= [];
        __instance.ExcludedGameObjects ??= [];
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SignalSenderLinker), "OnAwake")]
    public static void OnAwake(SignalSenderLinker __instance)
    {
        __instance.ExcludedComponents ??= [];
        __instance.ExcludedGameObjects ??= [];
    }
}