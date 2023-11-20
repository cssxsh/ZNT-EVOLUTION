using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    public static class DebugPatch
    {
        [HarmonyPatch(typeof(SceneLoader), methodName: "LoadNextScene"), HarmonyPrefix]
        public static void LoadNextScene(ref string sceneName)
        {
            BepInEx.Logging.Logger.CreateLogSource("SceneLoader").LogInfo("LoadNextScene: " + sceneName);
        }

        [HarmonyPatch(typeof(CustomAssetObject), methodName: "LoadFromAsset"), HarmonyPrefix]
        public static void LoadFromAsset(CustomAssetObject __instance, GameObject gameObject)
        {
            BepInEx.Logging.Logger.CreateLogSource("CustomAssetObject").LogInfo("LoadFromAsset: " + gameObject.name);
        }

        [HarmonyPatch(typeof(Material), methodName: "GetTexture", argumentTypes: typeof(string)), HarmonyPrefix]
        public static bool GetTexture(Material __instance, string name) => __instance.HasProperty(name);
    }
}