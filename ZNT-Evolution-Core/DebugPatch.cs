using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    public static class DebugPatch
    {
        [HarmonyPatch(typeof(SceneLoader), "LoadNextScene"), HarmonyPrefix]
        public static void LoadNextScene(ref string sceneName)
        {
            BepInEx.Logging.Logger.CreateLogSource("SceneLoader").LogInfo("LoadNextScene: " + sceneName);
        }

        [HarmonyPatch(typeof(CharacterAsset), "LoadFromAsset"), HarmonyPrefix]
        public static void LoadFromAsset(CharacterAsset __instance, GameObject gameObject)
        {
            BepInEx.Logging.Logger.CreateLogSource("CharacterAsset").LogInfo("LoadFromAsset: " + gameObject.name);
        }

        [HarmonyPatch(typeof(Material), "GetTexture", typeof(string)), HarmonyPrefix]
        public static bool GetTexture(Material __instance, string name) => __instance.HasProperty(name);
    }
}