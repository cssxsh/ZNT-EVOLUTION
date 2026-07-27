using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MonoMod.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZNT.Evolution.Core.Editor;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class DebugPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Challenge), "IsFailed")]
    [HarmonyPatch(typeof(Challenge), "IsCompleted")]
    public static void IsCompleted(Challenge __instance)
    {
        if (Traverse.Create(__instance).Field<List<ChallengeRule>>("checkList").Value != null) return;
        __instance.Initialize();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Material), "GetTexture", typeof(string))]
    public static bool GetTexture(Material __instance, string name) => __instance.HasProperty(name);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(I2.Loc.LocalizationManager), "GetTermTranslation")]
    public static string GetTermTranslation(string __result, string Term) => __result ?? Term;

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

    [HarmonyPostfix]
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

    [HarmonyPostfix]
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
                __instance.SendMessage(methodName: "HitGround", value: 0.0f);
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
        __instance.SendMessage(methodName: "SetTarget", value: target);
        return false;
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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PhysicObjectBehaviour), "Health", MethodType.Getter)]
    public static Health GetHealth(Health __result, PhysicObjectBehaviour __instance)
    {
        if (__result) return __result;
        return Traverse.Create(__instance).Field<Health>("health").Value = __instance.GetComponent<Health>();
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
        system.EraseTile(ti);
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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SupportedTypeBinder), "SetName")]
    public static bool SetName(SupportedTypeBinder __instance, MemberInfo member)
    {
        if (member.IsDefined(typeof(SerializeInEditorAttribute))) return true;
        Traverse.Create(__instance).Field<Text>("text").Value.text = member.Name;
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SupportedTypeBinder), "BindVector4Field")]
    public static void BindVector4Field(SupportedTypeBinder __instance, EditorComponent component, MemberInfo member)
    {
        var value = member.GetMemberValue<Vector4>(component.Data);
        var components = Traverse.Create(__instance).Field<UIBehaviour[]>("uiComponents").Value;
        ((InputField)components[2]).text = $"{value.z}";
        ((InputField)components[3]).text = $"{value.w}";
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
        if (string.IsNullOrEmpty(__instance.ComponentName) || string.IsNullOrEmpty(__instance.MethodName)) return;
        var type = Traverse.Create(__instance).Method("GetType", __instance.ComponentName).GetValue<Type>();
        var name = __instance.MethodName.ReplaceLast(" (param)", string.Empty);
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                   BindingFlags.FlattenHierarchy;
        try
        {
            var with_param = type.GetMethod(name, flags, null, [typeof(Parameters)], null);
            if (with_param is not null && with_param.IsDefined(typeof(SignalReceiverAttribute)))
            {
                Traverse.Create(__instance).Field<bool>("methodHasParam").Value = true;
                return;
            }

            var no_param = type.GetMethod(name, flags, null, [], null);
            if (no_param is not null && no_param.IsDefined(typeof(SignalReceiverAttribute)))
            {
                Traverse.Create(__instance).Field<bool>("methodHasParam").Value = false;
                return;
            }

            Traverse.Create(__instance).Field<bool>("methodHasParam").Value = with_param is not null;
        }
        catch (Exception)
        {
            // ignored
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SignalReceiverLinker), "OnAwake")]
    public static void OnAwake(SignalReceiverLinker __instance)
    {
        __instance.ExcludedComponents ??= __instance.GetComponentsInChildren<BaseComponent>(includeInactive: true)
            .Where(component => !component.EditorVisibility).ToList();
        __instance.ExcludedGameObjects ??= [];
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SignalSenderLinker), "OnAwake")]
    public static void OnAwake(SignalSenderLinker __instance)
    {
        __instance.ExcludedComponents ??= __instance.GetComponentsInChildren<BaseComponent>(includeInactive: true)
            .Where(component => !component.EditorVisibility).ToList();
        __instance.ExcludedGameObjects ??= [];
    }
}