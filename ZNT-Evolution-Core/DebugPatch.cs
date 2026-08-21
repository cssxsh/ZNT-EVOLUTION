using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MonoMod.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ZNT.Evolution.Core.Editor;
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
    [HarmonyPatch(typeof(Challenge), "IsFailed")]
    [HarmonyPatch(typeof(Challenge), "IsCompleted")]
    public static void IsCompleted(Challenge __instance)
    {
        if (Traverse.Create(__instance).Field<List<ChallengeRule>>("checkList").Value != null) return;
        __instance.Initialize();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AchievementManager), "OnCreate")]
    public static void OnCreate(AchievementManager __instance)
    {
        __instance.enabled = SteamManager.Initialized && SteamManager.Instance.GetUserName() != "Goldberg";
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
        Traverse.Create(__instance).Field<int>("randomSeed").Value = __instance.GetInstanceID();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Character), "OnVisionLost")]
    public static bool OnVisionLost(GameObject target) => target is not null;

    private static bool CheckOneWay(this Collider2D collider, Moveable mover)
    {
        if (!OneWayEditor.TryGetOneWay(collider, out var wall)) return true;
        return wall.Direction == Vector2.up && wall.BlockLayer(mover.Body.gameObject.layer);
    }

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
        if (!hit || DetectionHelper.CastCheck[0].collider.CheckOneWay(__instance)) return;
        Traverse.Create(__instance).Field<LayerMask>("groundLayers").Value = __state & ~mask;
    }

    [HarmonyFinalizer]
    [HarmonyPatch(typeof(Moveable), "UpdateIsGrounded")]
    public static void UpdateIsGrounded(Moveable __instance, LayerMask __state)
    {
        Traverse.Create(__instance).Field<LayerMask>("groundLayers").Value = __state;
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
        if (!hit || DetectionHelper.CastCheck[0].collider.CheckOneWay(__instance.Mover)) return;
        Traverse.Create(__instance.Mover).Field<LayerMask>("groundLayers").Value = __state & ~mask;
    }

    [HarmonyFinalizer]
    [HarmonyPatch(typeof(TankBehaviour), "AfterStepping")]
    [HarmonyPatch(typeof(CharacterBehaviour), "AfterStepping")]
    public static void AfterStepping(CharacterBehaviour __instance, LayerMask __state)
    {
        Traverse.Create(__instance.Mover).Field<LayerMask>("groundLayers").Value = __state;
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
                Traverse.Create(__instance).Method("HitGround", 0.0f).GetValue();
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
        Traverse.Create(__instance).Method("SetTarget", target).GetValue();
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
        Traverse.Create(component).Field<bool>("exploded").Value = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WeatherRain), "CreateEffect")]
    public static bool CreateEffect(WeatherRain __instance)
    {
        var rain = Traverse.Create(__instance).Field<RainEffect>("rainEffect").Value;
        return rain is null;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(WeatherRain), "OnEditorClose")]
    public static void OnEditorClose(WeatherRain __instance)
    {
        var rain = Traverse.Create(__instance).Field<RainEffect>("rainEffect").Value;
        var intensity = Traverse.Create(__instance).Field<float>("intensity").Value;
        var length = Traverse.Create(__instance).Field<float>("length").Value;
        var angle = Traverse.Create(__instance).Field<float>("angle").Value;
        var speed = Traverse.Create(__instance).Field<Vector2>("speed").Value;
        var density = Traverse.Create(__instance).Field<Vector4>("density").Value;
        rain?.UpdateSettings(intensity, length, angle, speed, density);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(WeatherRain), "OnDestroy")]
    public static void OnDestroy(WeatherRain __instance)
    {
        var rain = Traverse.Create(__instance).Field<RainEffect>("rainEffect").Value;
        rain?.gameObject.SetActive(false);
        Traverse.Create(__instance).Field<RainEffect>("rainEffect").Value = null;
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
        var element = Traverse.Create(__instance).Field<LevelElement>("element").Value;
        var level = Traverse.Create(__instance).Field<LevelLoaderManager>("levelManager").Value;
        var system = Traverse.Create(__instance).Field<Rotorz.Tile.TileSystem>("tileSystem").Value;
        if (system.GetTileOrNull(ti)?.gameObject == __instance.gameObject) return false;
        level.PaintTile(
            system: system,
            element: element,
            index: ti,
            paintShape: Toolbox.PaintShape.Square,
            paintSize: 1U,
            refreshSurrounding: true);
        var tile = system.GetTileOrNull(ti);
        if (tile == null) return false;
        __instance.gameObject.CopyTo(tile.gameObject);
        __instance.OnCopy?.Invoke(tile.gameObject, __instance.Type is ObjectSettings.ElementType.Brush);
        tile.gameObject.BroadcastMessage(
            methodName: "ObjectMovedInEditor",
            parameter: position,
            options: SendMessageOptions.DontRequireReceiver);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Framework.Events.SignalReceiver), "GetType")]
    public static Type GetType(Type __result, string typeName)
    {
        if (__result != null) return __result;
        __result = AccessTools.TypeByName(typeName);
        if (__result == null) return null;
        var cached = Traverse.Create<Framework.Events.SignalReceiver>()
            .Field<Dictionary<string, Type>>("cachedType").Value;
        return cached[typeName] = __result;
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