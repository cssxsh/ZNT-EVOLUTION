using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class TextAssetPatch
{
    private static readonly Dictionary<TextAsset, byte[]> Cache = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TextAsset), nameof(TextAsset.bytes), MethodType.Getter)]
    public static byte[] GetBytes(byte[] __result, TextAsset __instance)
    {
        return Cache.GetValueOrDefault(__instance, __result);
    }

    public static void SetBytes(this TextAsset text, byte[] value)
    {
        if (value is null) Cache.Remove(text);
        else Cache[text] = value;
    }
}