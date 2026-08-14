using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class TextAssetPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TextAsset), nameof(TextAsset.bytes), MethodType.Getter)]
    public static byte[] GetBytes(byte[] __result, TextAsset __instance)
    {
        return __instance is BankAsset bank ? bank.data : __result;
    }
}