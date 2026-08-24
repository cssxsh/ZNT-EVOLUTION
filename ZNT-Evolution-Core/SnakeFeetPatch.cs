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
}