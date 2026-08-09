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
        if (__instance.Frozen || !__instance.Vision.enabled) return;
        if (__instance.Weapon.Attack.HasTarget()) return;
        var frequency = __instance.Vision.Frequency;
        try
        {
            __instance.Vision.Frequency = 1748;
            __instance.Vision.Update();
        }
        finally
        {
            __instance.Vision.Frequency = frequency;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpawnCharacterChooser), "OnCreate")]
    public static void OnCreate(SpawnCharacterChooser __instance)
    {
        var spawn = Traverse.Create(__instance).Field<CharacterSpawnPoint>("spawn").Value;
        if (Traverse.Create(spawn).Field<Enum>("spawnType").Value.ToString() is not "Human") return;
        var characters = Traverse.Create(__instance).Field<List<CharacterAsset>>("selectableCharacters").Value;
        characters.AddRange(LevelElementIndex.Index.Values.Cast<LevelElement>()
            .Where(element => element.Useable)
            .Select(element => element.CustomAsset)
            .OfType<HumanAsset>()
            .Where(asset => !characters.Contains(asset) && asset.AnimationLibrary.AnimationExists("rise"))
            .Distinct());
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
    [HarmonyPatch(typeof(ExplosionEffect), "OnApplyOnGameObject")]
    public static void OnApplyOnGameObject(ExplosionEffect __instance, GameObject target, out float __state)
    {
        __state = __instance.Damage;
        if (!target.HasAnyTags(Tag.Human)) return;
        var count = Physics2D.LinecastNonAlloc(
            start: __instance.Trigger.Detection.Origin.position,
            end: target.transform.position,
            results: DetectionHelper.DistanceCheck,
            layerMask: LayerMask.GetMask("Zombie Stopper"));
        var total = 0;
        for (var i = 0; i < count; i++)
        {
            var hit = DetectionHelper.DistanceCheck[i];
            var stopper = hit.collider.GetComponentInParent<Stopper>();
            if (stopper is null) continue;
            var opponents = Traverse.Create(stopper).Field<bool>("blockOpponents").Value
                ? Traverse.Create(stopper).Field<int>("MaxOpponents").Value
                : 0;
            total += opponents;
        }

        __instance.Damage -= total * 50.0f;
    }

    [HarmonyFinalizer]
    [HarmonyPatch(typeof(ExplosionEffect), "OnApplyOnGameObject")]
    public static void OnApplyOnGameObject(ExplosionEffect __instance, float __state)
    {
        __instance.Damage = __state;
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