﻿using HarmonyLib;
using UnityEngine;

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
    }
}