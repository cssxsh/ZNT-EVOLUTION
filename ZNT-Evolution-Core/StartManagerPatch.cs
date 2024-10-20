using System;
using System.Collections;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    public static class StartManagerPatch
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("StartManager");

        private static IEnumerator Loading;

        [HarmonyPatch(typeof(StartManager), "Start"), HarmonyPostfix]
        public static void Start(StartManager __instance, ref IEnumerator __result)
        {
            __result = Loading = LoadAsset(prefix: __result);
        }

        private static IEnumerator LoadAsset(IEnumerator prefix)
        {
            yield return prefix;
            Logger.LogInfo("Initialized");
            yield return LevelElementLoader.LoadBanks(folder: Application.streamingAssetsPath, loadSamples: true);
            Logger.LogInfo("Load Bank OK");
            foreach (var type in (LevelElement.Type[])Enum.GetValues(typeof(LevelElement.Type)))
            {
                var path = Path.Combine(Application.dataPath, type.ToString());

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    if (directory.EndsWith(".bak")) continue;
                    if (directory.EndsWith(" - 副本")) continue;
                    var target = Path.GetFullPath(directory);
                    yield return LevelElementLoader.LoadFormFolder(path: target, type: type);
                }
            }
            Logger.LogInfo("Load LevelElement OK");
            var apply = Path.Combine(Application.dataPath, "Apply");
            if (!Directory.Exists(apply)) Directory.CreateDirectory(apply);
            foreach (var directory in Directory.EnumerateDirectories(apply))
            {
                if (directory.EndsWith(".bak")) continue;
                if (directory.EndsWith(" - 副本")) continue;
                var target = Path.GetFullPath(directory);
                yield return LevelElementLoader.ApplyFormFolder(path: target);
            }
            Logger.LogInfo("Apply LevelElement OK");
        }

        [HarmonyPatch(typeof(StartManager), methodName: "LoadNextScene"), HarmonyPrefix]
        public static bool LoadNextScene(StartManager __instance) => !Loading.MoveNext();
    }
}