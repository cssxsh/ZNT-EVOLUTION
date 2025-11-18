using HarmonyLib;

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
}