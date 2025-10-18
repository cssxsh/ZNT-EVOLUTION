using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

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
    [HarmonyPatch(typeof(LevelEditor.SelectionMenu), "UpdateComponentMenu")]
    public static bool UpdateComponentMenu(LevelEditor.SelectionMenu __instance)
    {
        var mainContainer = Traverse.Create(__instance)
            .Field<RectTransform>("mainContainer").Value;
        mainContainer.DestroyChildren();
        mainContainer.anchoredPosition = Vector2.zero;
        var serializeGameObject = Traverse.Create(__instance)
            .Field<EditorGameObject>("serializeGameObject").Value;
        var componentsUpdate = Traverse.Create(__instance)
            .Field<List<IEditorUpdate>>("componentsUpdate").Value;
        foreach (var component in serializeGameObject.Components)
        {
            if (component?.Data is null) continue;
            if (component.Type == typeof(ObjectSettings)) continue;
            if (component.Fields.Count == 0) continue;
            var updater = component.Data as IEditorUpdate;
            var overrider = component.Data as IEditorOverride;
            if (updater != null) componentsUpdate.Add(updater);

            updater?.OnEditorOpen();
            var text = __instance.SetComponentHeader(component);
            var prev = mainContainer.childCount;
            foreach (var (member, _) in component.Fields)
            {
                if (overrider != null && overrider.OverrideMemberUi(__instance, component, member)) continue;
                __instance.SetDefaultUi(component, member);
            }

            if (prev == mainContainer.childCount) UnityEngine.Object.Destroy(text.gameObject);
        }

        var scrollRect = Traverse.Create(__instance)
            .Field<ScrollRect>("scrollRect").Value;
        scrollRect.Rebuild(CanvasUpdate.PostLayout);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ObjectSettings), "CopyObject")]
    private static bool CopyObject(ObjectSettings __instance, Rotorz.Tile.TileIndex ti)
    {
        if (__instance.Type != ObjectSettings.ElementType.Brush) return true;
        var element = Traverse.Create(__instance).Field<LevelElement>("element").Value;
        var level = Traverse.Create(__instance).Field<LevelLoaderManager>("levelManager").Value;
        var system = Traverse.Create(__instance).Field<Rotorz.Tile.TileSystem>("tileSystem").Value;
        system.EraseTile(ti);
        level.PaintTile(
            system: system,
            element: element,
            index: ti,
            paintShape: LevelEditor.Toolbox.PaintShape.Square,
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
    public static void GetType(string typeName, ref Type __result)
    {
        if (__result != null) return;
        __result = AccessTools.TypeByName(typeName);
        if (__result == null) return;
        var cached = Traverse.Create<Framework.Events.SignalReceiver>()
            .Field<Dictionary<string, Type>>("cachedType").Value;
        cached[typeName] = __result;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SteamManager), "Initialized", MethodType.Getter)]
    public static bool Initialized() => Steamworks.SteamFriends.GetPersonaName() != "Goldberg";
}