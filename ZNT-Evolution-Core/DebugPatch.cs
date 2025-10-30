using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpawnCharacterChooser), "OnCreate")]
    public static void OnCreate(SpawnCharacterChooser __instance)
    {
        var spawn = Traverse.Create(__instance).Field("spawn").Field<Enum>("spawnType").Value;
        var characters = Traverse.Create(__instance).Field<List<CharacterAsset>>("selectableCharacters").Value;
        switch (spawn.ToString())
        {
            case "Human":
                characters.AddRange(LevelElementIndex.Index.Values.Cast<LevelElement>()
                    .Where(element => element.Useable)
                    .Select(element => element.CustomAsset)
                    .OfType<HumanAsset>()
                    .Where(asset => !characters.Contains(asset))
                    .Distinct());
                break;
            case "Zombie":
                characters.AddRange(LevelElementIndex.Index.Values.Cast<LevelElement>()
                    .Where(element => element.Useable)
                    .Select(element => element.CustomAsset)
                    .OfType<ZombieAsset>()
                    .Where(asset => !characters.Contains(asset))
                    .Distinct());
                break;
            default:
                characters.AddRange(LevelElementIndex.Index.Values.Cast<LevelElement>()
                    .Where(element => element.Useable)
                    .Select(element => element.CustomAsset)
                    .OfType<CharacterAsset>()
                    .Where(asset => !characters.Contains(asset))
                    .Distinct());
                break;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RayConeDetection), "FindGameObjects")]
    public static void FindGameObjects(RayConeDetection __instance, C5.HashedArrayList<GameObject> __result)
    {
        if (__instance.CastAll) return;
        var rays = Traverse.Create(__instance).Field<Vector2[]>("rays").Value;
        var inverted = Traverse.Create(__instance).Field<int>("inverted").Value;
        __result.Clear();
        foreach (var ray in rays)
        {
            DetectionHelper.RayCast(
                __result,
                __instance.Origin.position,
                ray * inverted,
                __instance.Distance,
                __instance.Trigger.IgnoreLayers,
                __instance.Trigger.Layers,
                __instance.Trigger.IgnoreWithTags,
                __instance.Trigger.WithTags,
                __instance.Trigger.IgnoreWithoutTags,
                __instance.Trigger.WithoutTags,
                __instance.Trigger.WithAllTags,
                __instance.Trigger.WithoutAllTags,
                __instance.Trigger.InvertTagsMatch);
        }
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
        if (hit && DetectionHelper.CastCheck[0].collider.CheckOneWay(__instance)) return;
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
    public static void AfterStepping(CharacterBehaviour __instance, out LayerMask __state)
    {
        __state = __instance.Mover.GroundLayers;
        var mask = LayerMask.GetMask("Stairs", "Stairs Top");
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
    [HarmonyPatch(typeof(HumanBehaviour), "AttackTarget")]
    public static bool AttackTarget(HumanBehaviour __instance, bool moveToTarget, Transform target)
    {
        __instance.Mover.UpdateIsGrounded();
        if (moveToTarget || !__instance.CanAttack()) return true;
        __instance.SendMessage(methodName: "SetTarget", value: target);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HumanBehaviour), "OnDespawned")]
    [HarmonyPatch(typeof(ZombieBehaviour), "OnDespawned")]
    [HarmonyPatch(typeof(CharacterBehaviour), "OnDespawned")]
    public static void OnDespawned(CharacterBehaviour __instance)
    {
        var prefab = __instance.GetComponent<PoolRetriever>()?.Prefab?.GetComponent<CharacterBehaviour>();
        if (prefab is null) return;
        __instance.SensesIgnored = prefab.SensesIgnored;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PhysicObjectBehaviour), "Health", MethodType.Getter)]
    public static Health GetHealth(Health __result, PhysicObjectBehaviour __instance)
    {
        if (__result) return __result;
        return Traverse.Create(__instance).Field<Health>("health").Value = __instance.GetComponent<Health>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PathologicalGames.PrefabPool), "nameInstance")]
    public static void AddPoolRetriever(Transform instance)
    {
        instance.gameObject.GetComponentSafe<PoolRetriever>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ObjectSettings), "CopyObject")]
    public static bool CopyObject(ObjectSettings __instance, Rotorz.Tile.TileIndex ti)
    {
        if (__instance.Type != ObjectSettings.ElementType.Brush) return true;
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
        __instance.OnCopy?.Invoke(tile.gameObject, true);
        tile.gameObject.BroadcastMessage(
            methodName: "ObjectCopiedInEditor",
            parameter: true,
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
    [HarmonyPatch(typeof(SignalReceiverLinker), "OnAwake")]
    public static void OnAwake(SignalReceiverLinker __instance)
    {
        __instance.ExcludedComponents ??= __instance.GetComponentsInChildren<BaseComponent>(includeInactive: true)
            .Where(component => !component.EditorVisibility).ToList();
        __instance.ExcludedGameObjects ??= new List<GameObject>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SignalSenderLinker), "OnAwake")]
    public static void OnAwake(SignalSenderLinker __instance)
    {
        __instance.ExcludedComponents ??= __instance.GetComponentsInChildren<BaseComponent>(includeInactive: true)
            .Where(component => !component.EditorVisibility).ToList();
        __instance.ExcludedGameObjects ??= new List<GameObject>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SteamManager), "Initialized", MethodType.Getter)]
    public static bool Initialized() => Steamworks.SteamFriends.GetPersonaName() != "Goldberg";
}