using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UIWidgets;
using UnityEngine;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal class SnakeFeetPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HumanBehaviour), "OnAttackHit")]
    public static void OnAttackHit(HumanBehaviour __instance)
    {
        if (__instance.Frozen) return;
        var frequency = __instance.Vision.Frequency;
        __instance.Vision.Frequency = 1748;
        if (__instance.Weapon.Attack.Target is null) __instance.Vision.Update();
        __instance.Vision.Frequency = frequency;
    }

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
    [HarmonyPatch(typeof(LevelSettingsMenu), "InitGeneralSettings")]
    public static void InitGeneralSettings(LevelSettingsMenu __instance)
    {
        Traverse.Create(__instance).Field<Spinner>("maxZombieSpinner").Value.Max = 1024;
        Traverse.Create(__instance).Field<Spinner>("maxEnemySpinner").Value.Max = 1024;
        Traverse.Create(__instance).Field<SpinnerFloat>("maxZoomSpinner").Value.Max = 1024f;
    }
}