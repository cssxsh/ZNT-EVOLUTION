using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

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

    internal static readonly Dictionary<Collider2D, int> Opponents = new();

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
            var opponents = Opponents.GetValueOrDefault(hit.collider, 0);
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
}