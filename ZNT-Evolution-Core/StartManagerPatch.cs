using System.Collections;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using MonoMod.Utils;
using UnityEngine;
using ZNT.Evolution.Core.Asset;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class StartManagerPatch
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(StartManager));

    private static IEnumerator prefix;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartManager), "Start")]
    public static void Start(StartManager __instance, ref IEnumerator __result)
    {
        prefix = __result;
        EvolutionCorePlugin.Instance.StartCoroutine(LoadAsset(__instance));
    }

    private static IEnumerator LoadAsset(StartManager starter)
    {
        Logger.LogInfo("Initializing");
        yield return prefix;
        Logger.LogInfo("Loading Bank");
        yield return LevelElementLoader.LoadBanks(folder: Application.streamingAssetsPath, loadSamples: true);
        Logger.LogInfo("Loading LevelElement");
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        foreach (var asset in Resources.FindObjectsOfTypeAll<CustomAsset>())
        {
            CustomAssetUtility.Cache[asset.NameAndType()] = asset;
            switch (asset)
            {
                case HumanAsset { BlockOpponents: true, MaxOpponentsBlock: 0 } human:
                    human.BlockOpponents = false;
                    Logger.LogDebug($"Fix BlockOpponents for {human}");
                    break;
                case ExplosionAsset { HierarchyName: "" } explosion:
                    explosion.HierarchyName = explosion.name.SpacedPascalCase();
                    Logger.LogDebug($"Fix HierarchyName for {explosion}");
                    break;
                case PhysicObjectAsset { DamageCharacterOnTrigger: true, DamageRadius: 0 } physic:
                    physic.DamageRadius = physic.ColliderRadius;
                    Logger.LogDebug($"Fix DamageRadius for {physic}");
                    break;
            }
        }

        var assets = Path.Combine(Application.dataPath, "Asset");
        if (!Directory.Exists(assets)) Directory.CreateDirectory(assets);
        LevelElementLoader.LoadAssetFromFolder(path: assets);

        var brush = Path.Combine(Application.dataPath, nameof(LevelElement.Type.Brush));
        if (!Directory.Exists(brush)) Directory.CreateDirectory(brush);
        foreach (var directory in Directory.EnumerateDirectories(brush))
        {
            if (directory.EndsWith(".bak")) continue;
            if (directory.EndsWith(" - 副本")) continue;
            if (directory.EndsWith("新建文件夹")) continue;
            var target = Path.GetFullPath(directory);
            yield return LevelElementLoader.LoadFromFolder(path: target, type: LevelElement.Type.Brush);
        }

        var decor = Path.Combine(Application.dataPath, nameof(LevelElement.Type.Decor));
        if (!Directory.Exists(decor)) Directory.CreateDirectory(decor);
        foreach (var directory in Directory.EnumerateDirectories(decor))
        {
            if (directory.EndsWith(".bak")) continue;
            if (directory.EndsWith(" - 副本")) continue;
            if (directory.EndsWith("新建文件夹")) continue;
            var target = Path.GetFullPath(directory);
            yield return LevelElementLoader.LoadFromFolder(path: target, type: LevelElement.Type.Decor);
        }

        var apply = Path.Combine(Application.dataPath, "Apply");
        if (!Directory.Exists(apply)) Directory.CreateDirectory(apply);
        foreach (var directory in Directory.EnumerateDirectories(apply))
        {
            if (directory.EndsWith(".bak")) continue;
            if (directory.EndsWith(" - 副本")) continue;
            if (directory.EndsWith("新建文件夹")) continue;
            var target = Path.GetFullPath(directory);
            yield return LevelElementLoader.ApplyFromFolder(path: target);
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

        Logger.LogInfo("Loading EventHandle");
        foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
        {
            if (info.Metadata.Name == "UnityExplorer") continue;
            AnimationEventHandlerPatch.RegisterAnimationEvent(info.Instance.GetType().Assembly);
        }

        prefix = null;
        starter.LoadNextScene();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StartManager), "LoadNextScene")]
    public static bool LoadNextScene(StartManager __instance) => prefix is null;
}