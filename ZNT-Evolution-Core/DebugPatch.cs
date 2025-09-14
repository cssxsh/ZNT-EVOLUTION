using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    public static class DebugPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneLoader), "LoadNextScene")]
        public static void LoadNextScene(ref string sceneName)
        {
            BepInEx.Logging.Logger.CreateLogSource("SceneLoader")
                .LogInfo($"LoadNextScene: {sceneName}");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomAssetObject), "LoadFromAsset")]
        public static void LoadFromAsset(CustomAssetObject __instance, GameObject gameObject)
        {
            BepInEx.Logging.Logger.CreateLogSource("CustomAssetObject")
                .LogInfo($"LoadFromAsset: {gameObject} for {__instance}");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Challenge), "IsCompleted")]
        public static void IsCompleted(Challenge __instance)
        {
            if (Traverse.Create(__instance).Field<List<ChallengeRule>>("checkList").Value == null)
            {
                __instance.Initialize();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Material), "GetTexture", typeof(string))]
        public static bool GetTexture(Material __instance, string name) => __instance.HasProperty(name);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(I2.Loc.LocalizationManager), "GetTermTranslation")]
        public static string GetTermTranslation(string __result, string Term) => __result ?? Term;
    }
}