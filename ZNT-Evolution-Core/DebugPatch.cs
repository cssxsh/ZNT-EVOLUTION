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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Character), "OnVisionLost")]
    public static bool OnVisionLost(GameObject target) => target is not null;

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
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var key in component.Fields.Keys)
            {
                if (overrider != null && overrider.OverrideMemberUi(__instance, component, key)) continue;
                __instance.SetDefaultUi(component, key);
            }

            if (prev == mainContainer.childCount) UnityEngine.Object.Destroy(text.gameObject);
        }

        var scrollRect = Traverse.Create(__instance)
            .Field<ScrollRect>("scrollRect").Value;
        scrollRect.Rebuild(CanvasUpdate.PostLayout);
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