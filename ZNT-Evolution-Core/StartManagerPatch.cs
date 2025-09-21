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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartManager), "Start")]
        public static void Start(StartManager __instance, ref IEnumerator __result)
        {
            __result = Loading = LoadAsset(prefix: __result);
        }

        private static IEnumerator LoadAsset(IEnumerator prefix)
        {
            Logger.LogInfo("Initializing");
            yield return prefix;
            Logger.LogInfo("Loading Bank");
            yield return LevelElementLoader.LoadBanks(folder: Application.streamingAssetsPath, loadSamples: true);
            Logger.LogInfo("Loading LevelElement");
            foreach (var type in (LevelElement.Type[])Enum.GetValues(typeof(LevelElement.Type)))
            {
                var path = Path.Combine(Application.dataPath, type.ToString());
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    if (directory.EndsWith(".bak")) continue;
                    if (directory.EndsWith(" - 副本")) continue;
                    var target = Path.GetFullPath(directory);
                    yield return LevelElementLoader.LoadFromFolder(path: target, type: type);
                }
            }

            var apply = Path.Combine(Application.dataPath, "Apply");
            if (!Directory.Exists(apply)) Directory.CreateDirectory(apply);
            foreach (var directory in Directory.EnumerateDirectories(apply))
            {
                if (directory.EndsWith(".bak")) continue;
                if (directory.EndsWith(" - 副本")) continue;
                var target = Path.GetFullPath(directory);
                yield return LevelElementLoader.ApplyFormFolder(path: target);
            }

            Logger.LogInfo("Loading Patch");
            foreach (var file in Directory.EnumerateFiles(Application.streamingAssetsPath, "*.patch"))
            {
                var request = AssetBundle.LoadFromFileAsync(file);
                yield return request;
                var fonts = request.assetBundle.LoadAllAssets<TMPro.TMP_FontAsset>();
                TMPro.TMP_Settings.fallbackFontAssets.AddRange(fonts);
                var font = request.assetBundle.LoadAsset<TMPro.TMP_FontAsset>("wqy-microhei_CN SDF.asset");
                if (font) TMPro.TMP_Settings.defaultFontAsset.fallbackFontAssets.Insert(0, font);
                Logger.LogInfo($"Loaded Patch {file}");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartManager), "LoadNextScene")]
        public static bool LoadNextScene(StartManager __instance) => !Loading.MoveNext();
    }
}