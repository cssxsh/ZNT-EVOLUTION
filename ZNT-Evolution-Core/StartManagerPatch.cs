using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;
using ZNT.Evolution.Core.Effect;
using ZNT.Evolution.Core.Mod;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class StartManagerPatch
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(StartManager));

    private static IEnumerator ToCoroutine(this Task task, YieldInstruction instruction = null)
    {
        while (!task.IsCompleted) yield return instruction;
        if (task.Exception == null) yield break;
        Logger.LogError(task.Exception.InnerExceptions.Count == 1
            ? task.Exception.GetBaseException()
            : task.Exception);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartManager), "Start")]
    public static IEnumerator Start(IEnumerator __result, StartManager __instance)
    {
        yield return __result;
        EvolutionCorePlugin.Instance.StartCoroutine(__instance.Evolution());
    }

    private static IEnumerator Evolution(this StartManager starter)
    {
        Traverse.Create(starter).Field<bool>("isLoading").Value = true;
        yield return Initialize();
        yield return LoadModsFolder();
        yield return LoadBank();
        yield return LoadAssetFolder();
        yield return LoadBrushFolder();
        yield return LoadDecorFolder();
        yield return LoadApplyFolder();
        Traverse.Create(starter).Field<bool>("isLoading").Value = false;
        starter.LoadNextScene();
    }

    private static IEnumerator Initialize()
    {
        Logger.LogInfo("Initializing");
        yield return CustomAssetUtility.LoadBuildIn<CustomAsset>(asset =>
        {
            CustomAssetUtility.Cache[asset.NameAndType()] = asset;
            switch (asset)
            {
                case HumanAsset { BlockOpponents: true, MaxOpponentsBlock: 0 } human:
                    human.BlockOpponents = false;
                    Logger.LogDebug($"Fix BlockOpponents for {human}");
                    break;
                case HumanAsset { name: "Bishop" or "Priest" or "Virgin" or "Preacher" } human:
                    human.CharacterType = CharacterType.Cultist;
                    Logger.LogDebug($"Fix CharacterType for {human}");
                    break;
                case PhysicObjectAsset { DamageCharacterOnTrigger: true, DamageRadius: 0 } physic:
                    physic.DamageCharacterOnTrigger = false;
                    Logger.LogDebug($"Fix DamageCharacterOnTrigger for {physic}");
                    break;
                case MovingObjectAsset { Speed: 50.0f } moving:
                    moving.Speed = 15.0f;
                    Logger.LogDebug($"Fix Speed for {moving}");
                    break;
                case LevelElement { name: "drone_exterminator" } drone:
                    drone.Title = "Drone Exterminator";
                    Logger.LogDebug($"Fix Title for {drone}");
                    break;
                case LevelElement { name: "human_daftpunk_1" } daft:
                    daft.Title = "Human Daft Punk 1";
                    Logger.LogDebug($"Fix Title for {daft}");
                    daft.CustomAsset.HierarchyName = "Daft Punk 1";
                    Logger.LogDebug($"Fix HierarchyName for {daft.CustomAsset}");
                    break;
                case LevelElement { name: "human_daftpunk_2" } daft:
                    daft.Title = "Human Daft Punk 2";
                    Logger.LogDebug($"Fix Title for {daft}");
                    break;
                case LevelElement { name: "human_perchman" } man:
                    man.Title = "Human Soundman";
                    Logger.LogDebug($"Fix Title for {man}");
                    man.CustomAsset.HierarchyName = "Soundman";
                    Logger.LogDebug($"Fix HierarchyName for {man.CustomAsset}");
                    break;
                case LevelElement { name: "human_sniper" } sniper:
                    sniper.Title = "Human Sniper 1";
                    Logger.LogDebug($"Fix Title for {sniper}");
                    sniper.CustomAsset.HierarchyName = "Sniper 1";
                    Logger.LogDebug($"Fix HierarchyName for {sniper.CustomAsset}");
                    break;
                case LevelElement { name: "human_survivor_molotov" } survivor:
                    survivor.Title = "Human Survivor Molotov";
                    Logger.LogDebug($"Fix Title for {survivor}");
                    survivor.CustomAsset.HierarchyName = "Survivor Molotov";
                    Logger.LogDebug($"Fix HierarchyName for {survivor.CustomAsset}");
                    break;
                case LevelElement { name: "chopter" } chopper:
                    chopper.Title = "Chopper";
                    if (chopper.AssetId is "2ffed9d7ca4fcd3479073a4189277809") chopper.name = "chopper";
                    Logger.LogDebug($"Fix Title for {chopper.AssetId}");
                    break;
                case LevelElement { name: "sewer_ladder", AssetId: "818b6793cae9cca49b49bfb01e4f45aa" } ladder:
                    ladder.name = "city_" + ladder.name;
                    Logger.LogDebug($"Fix Name for {ladder.AssetId}");
                    break;
                case LevelElement { CustomAsset: HumanAsset human } element:
                {
                    if (human.HierarchyName is "Rick") break;
                    if (human.HierarchyName is "Zombinator") break;
                    if (element.Title.Replace("Human ", "") == human.HierarchyName) break;
                    element.CustomAsset.HierarchyName = element.Title.Replace("Human ", "");
                    Logger.LogDebug($"Fix HierarchyName for {element.CustomAsset}");
                }
                    break;
                case LevelElement { CustomAsset: not null, Brush: Rotorz.Tile.OrientedBrush brush } element:
                {
                    if (brush.DefaultOrientation.GetVariation(0) is not GameObject prefab) break;
                    if (element.CustomAsset.Prefab == prefab.transform) break;
                    if (element.CustomAsset.name == prefab.name)
                    {
                        element.CustomAsset.Prefab = prefab.transform;
                        Logger.LogDebug($"Fix Prefab for {element.CustomAsset}");
                    }
                    else
                    {
                        brush.DefaultOrientation.SetVariation(0, element.CustomAsset.Prefab.gameObject);
                        Logger.LogDebug($"Fix Brush for {element}");
                    }
                }
                    break;
            }
        });
        yield return CustomAssetUtility.LoadBuildIn<tk2dSpriteCollectionData>(sprites =>
        {
            CustomAssetUtility.Cache[sprites.NameAndType()] = sprites;
        });
        yield return CustomAssetUtility.LoadBuildIn<tk2dSpriteAnimation>(animation =>
        {
            CustomAssetUtility.Cache[animation.NameAndType()] = animation;
            switch (animation)
            {
                case { name: "anim_blood" }:
                {
                    var explosion = animation.GetClipByName("blood_explosion");
                    var sprites = animation.FirstValidClip.frames[0].spriteCollection;
                    foreach (var frame in explosion.frames) frame.spriteCollection ??= sprites;
                    Logger.LogDebug($"Fix blood_explosion for {animation}");
                }
                    break;
                case { name: "anim_astrogoliath" or "anim_clown" or "anim_machine_gunner" }:
                {
                    var talk = animation.GetClipByName("talk");
                    new AnimationAddition([animation], [
                        new tk2dSpriteAnimationClip(talk)
                        {
                            name = "rise",
                            useableInLevelEditor = false,
                            wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
                        }
                    ]).Apply();
                    Logger.LogInfo($"Feat rise for {animation}");
                }
                    break;
                case { name: "anim_boss_1" }:
                {
                    var talk = animation.GetClipByName("stand_phone_talk");
                    new AnimationAddition([animation], [
                        new tk2dSpriteAnimationClip(talk)
                        {
                            name = "rise",
                            useableInLevelEditor = false,
                            wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
                        }
                    ]).Apply();
                    Logger.LogInfo($"Feat rise for {animation}");
                }
                    break;
                case { name: "anim_boss_chemist" }:
                {
                    var transform = animation.GetClipByName("transform");
                    new AnimationAddition([animation], [
                        new tk2dSpriteAnimationClip(transform)
                        {
                            name = "rise",
                            useableInLevelEditor = false,
                            wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
                        }
                    ]).Apply();
                    Logger.LogInfo($"Feat rise for {animation}");
                }
                    break;
                case { name: "anim_daft_punk_1" or "anim_daft_punk_2" }:
                {
                    var teleport = animation.GetClipByName("teleport_in");
                    new AnimationAddition([animation], [
                        new tk2dSpriteAnimationClip(teleport)
                        {
                            name = "rise",
                            useableInLevelEditor = false,
                            wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
                        }
                    ]).Apply();
                    Logger.LogInfo($"Feat rise for {animation}");
                }
                    break;
                case { name: "anim_traps" }:
                {
                    var missile = animation.GetClipByName("sentry_moon_canon_missile");
                    new AnimationAddition([animation], [
                        new tk2dSpriteAnimationClip
                        {
                            name = "empty",
                            frames = [missile.frames[1]],
                            fps = 1.0f,
                            loopStart = 0,
                            useableInLevelEditor = false,
                            staticAnimation = false,
                            wrapMode = tk2dSpriteAnimationClip.WrapMode.Single
                        }
                    ]).Apply();
                    Logger.LogInfo($"Feat empty for {animation}");
                }
                    break;
            }
        });
        yield return CustomAssetUtility.LoadBuildIn<TMPro.TMP_Asset>(asset =>
        {
            switch (asset)
            {
                case TMPro.TMP_FontAsset font:
                    TMPro.MaterialReferenceManager.AddFontAsset(font);
                    break;
                case TMPro.TMP_SpriteAsset emoji:
                    TMPro.MaterialReferenceManager.AddSpriteAsset(emoji);
                    break;
            }
        });
        yield return CustomAssetUtility.LoadPatch<Shader>(shader =>
        {
            CustomAssetUtility.Cache[shader.name] = shader;
            Logger.LogInfo($"Loaded Patch {shader}");
        });
        yield return CustomAssetUtility.LoadPatch<TMPro.TMP_Asset>(asset =>
        {
            switch (asset)
            {
                case TMPro.TMP_FontAsset font:
                    TMPro.MaterialReferenceManager.AddFontAsset(font);
                    Logger.LogInfo($"Loaded Patch {font}");
                    TMPro.TMP_Settings.fallbackFontAssets.RemoveAll(f => f is null);
                    TMPro.TMP_Settings.fallbackFontAssets.Add(font);
                    if (font.name is not "wqy-microhei SDF") break;
                    TMPro.TMP_Settings.defaultFontAsset.fallbackFontAssets.Insert(0, font);
                    break;
                case TMPro.TMP_SpriteAsset emoji:
                    TMPro.MaterialReferenceManager.AddSpriteAsset(emoji);
                    Logger.LogInfo($"Loaded Patch {emoji}");
                    break;
            }
        });
        InvisibleShield.PoolPrefab();
        FogOfWar.PoolPrefab();
        SphereBuffEffect.PoolPrefab();
        SphereLaoAerEffect.PoolPrefab();
        foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
        {
            if (!info.Metadata.GUID.Contains("znt")) continue;
            AnimationEventHandlerPatch.RegisterAnimationEvent(info.Instance.GetType().Assembly);
        }
    }

    private static IEnumerator LoadBank()
    {
        Logger.LogInfo("Loading Bank");
        yield return LevelElementLoader.LoadBanks(folder: Application.streamingAssetsPath, loadSamples: true);
    }

    private static IEnumerator LoadAssetFolder()
    {
        Logger.LogInfo("Loading Asset Folder");
        var asset = Path.Combine(Application.dataPath, "Asset");
        if (!Directory.Exists(asset)) yield break;
        yield return LevelElementLoader.LoadAssetFromFolder(path: asset);
    }

    private static IEnumerator LoadBrushFolder()
    {
        Logger.LogInfo("Loading Brush Folder");
        var brush = Path.Combine(Application.dataPath, nameof(LevelElement.Type.Brush));
        if (!Directory.Exists(brush)) yield break;
        foreach (var directory in Directory.EnumerateDirectories(brush))
        {
            if (directory.EndsWith(".bak")) continue;
            if (directory.EndsWith(" - 副本")) continue;
            if (directory.EndsWith("新建文件夹")) continue;
            var target = Path.GetFullPath(directory);
            yield return LevelElementLoader.LoadFromFolder(path: target, type: LevelElement.Type.Brush);
        }

        foreach (var element in LevelElementIndex.Index.Values.Cast<LevelElement>())
        {
            switch (element.CustomAsset)
            {
                case HumanAsset { RiseAsset: LazyRef lazy } human:
                    human.RiseAsset = lazy.Fetch() ?? lazy;
                    break;
            }
        }
    }

    private static IEnumerator LoadDecorFolder()
    {
        Logger.LogInfo("Loading Decor Folder");
        var decor = Path.Combine(Application.dataPath, nameof(LevelElement.Type.Decor));
        if (!Directory.Exists(decor)) yield break;
        foreach (var directory in Directory.EnumerateDirectories(decor))
        {
            if (directory.EndsWith(".bak")) continue;
            if (directory.EndsWith(" - 副本")) continue;
            if (directory.EndsWith("新建文件夹")) continue;
            var target = Path.GetFullPath(directory);
            yield return LevelElementLoader.LoadFromFolder(path: target, type: LevelElement.Type.Decor);
        }
    }

    private static IEnumerator LoadApplyFolder()
    {
        Logger.LogInfo("Loading Apply Folder");
        var apply = Path.Combine(Application.dataPath, "Apply");
        if (!Directory.Exists(apply)) yield break;
        foreach (var directory in Directory.EnumerateDirectories(apply))
        {
            if (directory.EndsWith(".bak")) continue;
            if (directory.EndsWith(" - 副本")) continue;
            if (directory.EndsWith("新建文件夹")) continue;
            var target = Path.GetFullPath(directory);
            yield return LevelElementLoader.ApplyFromFolder(path: target);
        }
    }

    private static IEnumerator LoadModsFolder()
    {
        ModManager.ModsPath ??= Path.Combine(Application.dataPath, "Mods");
        Logger.LogInfo($"Loading Mods Folder '{ModManager.ModsPath}'");
        if (!Directory.Exists(ModManager.ModsPath)) Directory.CreateDirectory(ModManager.ModsPath);
        yield return ModManager.LoadAll().ToCoroutine();
    }
}