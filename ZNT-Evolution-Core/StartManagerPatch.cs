using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;
using ZNT.Evolution.Core.Effect;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class StartManagerPatch
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(StartManager));

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartManager), "Start")]
    public static IEnumerator Start(IEnumerator __result, StartManager __instance)
    {
        yield return __result;
        EvolutionCorePlugin.Instance.StartCoroutine(LoadAsset(__instance));
    }

    private static IEnumerator LoadAsset(StartManager starter)
    {
        Logger.LogInfo("Initializing");
        Traverse.Create(starter).Field<bool>("isLoading").Value = true;
        yield return CustomAssetUtility.LoadBuildIn<CustomAsset>(asset =>
        {
            CustomAssetUtility.Cache[asset.NameAndType()] = asset;
            switch (asset)
            {
                case HumanAsset { BlockOpponents: true, MaxOpponentsBlock: 0 } human:
                    human.BlockOpponents = false;
                    Logger.LogInfo($"Fix BlockOpponents for {human}");
                    break;
                case HumanAsset { name: "Bishop" or "Priest" or "Virgin" or "Preacher" } human:
                    human.CharacterType = CharacterType.Cultist;
                    Logger.LogInfo($"Fix CharacterType for {human}");
                    break;
                case PhysicObjectAsset { DamageCharacterOnTrigger: true, DamageRadius: 0 } physic:
                    physic.DamageCharacterOnTrigger = false;
                    Logger.LogInfo($"Fix DamageCharacterOnTrigger for {physic}");
                    break;
                case MovingObjectAsset { Speed: 50.0f } moving:
                    moving.Speed = 15.0f;
                    Logger.LogInfo($"Fix Speed for {moving}");
                    break;
                case LevelElement
                {
                    name: "elevator_1_spawn" or "elevator_2_spawn" or "lawn_mower_spawn" or "moving_container_spawn",
                    Brush: Rotorz.Tile.OrientedBrush brush
                } element:
                {
                    if (brush.DefaultOrientation.GetVariation(0) is not GameObject prefab) break;
                    if (!prefab.TryGetComponent(out SpawnPoint spawn) || spawn is CharacterSpawnPoint) break;
                    Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowDamages").Value = true;
                    Logger.LogInfo($"Fix SpawnPoint for {element}");
                }
                    break;
                case LevelElement { name: "drone_exterminator" } drone:
                    drone.Title = "Drone Exterminator";
                    Logger.LogInfo($"Fix Title for {drone}");
                    break;
                case LevelElement { name: "human_daftpunk_1" } daft:
                    daft.Title = "Human Daft Punk 1";
                    Logger.LogInfo($"Fix Title for {daft}");
                    daft.CustomAsset.HierarchyName = "Daft Punk 1";
                    Logger.LogInfo($"Fix HierarchyName for {daft.CustomAsset}");
                    break;
                case LevelElement { name: "human_daftpunk_2" } daft:
                    daft.Title = "Human Daft Punk 2";
                    Logger.LogInfo($"Fix Title for {daft}");
                    break;
                case LevelElement { name: "human_perchman" } man:
                    man.Title = "Human Soundman";
                    Logger.LogInfo($"Fix Title for {man}");
                    man.CustomAsset.HierarchyName = "Soundman";
                    Logger.LogInfo($"Fix HierarchyName for {man.CustomAsset}");
                    break;
                case LevelElement { name: "human_sniper" } sniper:
                    sniper.Title = "Human Sniper 1";
                    Logger.LogInfo($"Fix Title for {sniper}");
                    sniper.CustomAsset.HierarchyName = "Sniper 1";
                    Logger.LogInfo($"Fix HierarchyName for {sniper.CustomAsset}");
                    break;
                case LevelElement { name: "human_survivor_molotov" } survivor:
                    survivor.Title = "Human Survivor Molotov";
                    Logger.LogInfo($"Fix Title for {survivor}");
                    survivor.CustomAsset.HierarchyName = "Survivor Molotov";
                    Logger.LogInfo($"Fix HierarchyName for {survivor.CustomAsset}");
                    break;
                case LevelElement { CustomAsset: HumanAsset human } element:
                {
                    if (human.HierarchyName is "Rick") break;
                    if (human.HierarchyName is "Zombinator") break;
                    if (element.Title.Replace("Human ", "") == human.HierarchyName) break;
                    element.CustomAsset.HierarchyName = element.Title.Replace("Human ", "");
                    Logger.LogInfo($"Fix HierarchyName for {element.CustomAsset}");
                }
                    break;
                case LevelElement { CustomAsset: not null, Brush: Rotorz.Tile.OrientedBrush brush } element:
                {
                    if (brush.DefaultOrientation.GetVariation(0) is not GameObject prefab) break;
                    if (element.CustomAsset.Prefab == prefab.transform) break;
                    if (element.CustomAsset.name == prefab.name)
                    {
                        element.CustomAsset.Prefab = prefab.transform;
                        Logger.LogInfo($"Fix Prefab for {element.CustomAsset}");
                    }
                    else
                    {
                        brush.DefaultOrientation.SetVariation(0, element.CustomAsset.Prefab.gameObject);
                        Logger.LogInfo($"Fix Brush for {element}");
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
                    Logger.LogInfo($"Fix blood_explosion for {animation}");
                }
                    break;
                case { name: "anim_astrogoliath" or "anim_clown" or "anim_machine_gunner" }:
                {
                    var transform = animation.GetClipByName("talk");
                    animation.clips = animation.clips.AddToArray(new tk2dSpriteAnimationClip(transform)
                    {
                        name = "rise",
                        useableInLevelEditor = false,
                        wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
                    });
                    Traverse.Create(animation)
                        .Field<Dictionary<string, tk2dSpriteAnimationClip>>("clipNameCache").Value = null;
                    Traverse.Create(animation)
                        .Field<Dictionary<string, int>>("idNameCache").Value = null;
                    animation.InitializeClipCache();
                    Logger.LogInfo($"Feat rise for {animation}");
                }
                    break;
                case { name: "anim_boss_1" }:
                {
                    var transform = animation.GetClipByName("stand_phone_talk");
                    animation.clips = animation.clips.AddToArray(new tk2dSpriteAnimationClip(transform)
                    {
                        name = "rise",
                        useableInLevelEditor = false,
                        wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
                    });
                    Traverse.Create(animation)
                        .Field<Dictionary<string, tk2dSpriteAnimationClip>>("clipNameCache").Value = null;
                    Traverse.Create(animation)
                        .Field<Dictionary<string, int>>("idNameCache").Value = null;
                    animation.InitializeClipCache();
                    Logger.LogInfo($"Feat rise for {animation}");
                }
                    break;
                case { name: "anim_boss_chemist" }:
                {
                    var transform = animation.GetClipByName("transform");
                    animation.clips = animation.clips.AddToArray(new tk2dSpriteAnimationClip(transform)
                    {
                        name = "rise",
                        useableInLevelEditor = false,
                        wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
                    });
                    Traverse.Create(animation)
                        .Field<Dictionary<string, tk2dSpriteAnimationClip>>("clipNameCache").Value = null;
                    Traverse.Create(animation)
                        .Field<Dictionary<string, int>>("idNameCache").Value = null;
                    animation.InitializeClipCache();
                    Logger.LogInfo($"Feat rise for {animation}");
                }
                    break;
                case { name: "anim_daft_punk_1" or "anim_daft_punk_2" }:
                {
                    var transform = animation.GetClipByName("teleport_in");
                    animation.clips = animation.clips.AddToArray(new tk2dSpriteAnimationClip(transform)
                    {
                        name = "rise",
                        useableInLevelEditor = false,
                        wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
                    });
                    Traverse.Create(animation)
                        .Field<Dictionary<string, tk2dSpriteAnimationClip>>("clipNameCache").Value = null;
                    Traverse.Create(animation)
                        .Field<Dictionary<string, int>>("idNameCache").Value = null;
                    animation.InitializeClipCache();
                    Logger.LogInfo($"Feat rise for {animation}");
                }
                    break;
                case { name: "anim_traps" }:
                {
                    var missile = animation.GetClipByName("sentry_moon_canon_missile");
                    animation.clips = animation.clips.AddToArray(new tk2dSpriteAnimationClip
                    {
                        name = "empty",
                        frames = [missile.frames[1]],
                        fps = 1.0f,
                        loopStart = 0,
                        useableInLevelEditor = false,
                        staticAnimation = false,
                        wrapMode = tk2dSpriteAnimationClip.WrapMode.Single
                    });
                    Traverse.Create(animation)
                        .Field<Dictionary<string, tk2dSpriteAnimationClip>>("clipNameCache").Value = null;
                    Traverse.Create(animation)
                        .Field<Dictionary<string, int>>("idNameCache").Value = null;
                    animation.InitializeClipCache();
                    Logger.LogInfo($"Feat empty for {animation}");
                }
                    break;
            }
        });
        InvisibleShield.PoolPrefab();
        FogOfWar.PoolPrefab();
        SphereBuffEffect.PoolPrefab();
        SphereLaoAerEffect.PoolPrefab();
        Logger.LogInfo("Loading Bank");
        yield return LevelElementLoader.LoadBanks(folder: Application.streamingAssetsPath, loadSamples: true);
        Logger.LogInfo("Loading LevelElement");
        var asset = Path.Combine(Application.dataPath, "Asset");
        if (!Directory.Exists(asset)) Directory.CreateDirectory(asset);
        yield return LevelElementLoader.LoadAssetFromFolder(path: asset);

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

        foreach (var effect in VisualEffectIndex.Index.Values.OfType<CustomVisualEffect>())
        {
            Logger.LogInfo($"Rebuild {effect}");
            Traverse.Create(effect).Field<Transform>("prefab").Value = Object.Instantiate(effect.Prefab);
            Object.DontDestroyOnLoad(effect.Prefab.gameObject);
            effect.Prefab.name = effect.name;
            effect.Prefab.gameObject.SetActive(false);
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var pool = effect.Prefab.GetComponent<PoolRetriever>();
            if (pool) Object.Destroy(pool);
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var animator = effect.Prefab.GetComponent<SpriteAnimator>();
            if (animator) animator.Animator.playAutomatically &= !(effect.animation?.PlayAnimation ?? false);
            effect.Prefab.gameObject.SetActive(true);
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

        Logger.LogInfo("Loading Patch");
        foreach (var file in Directory.EnumerateFiles(Application.streamingAssetsPath, "*.patch"))
        {
            var request = AssetBundle.LoadFromFileAsync(file);
            yield return request;
            var fonts = request.assetBundle.LoadAllAssets<TMPro.TMP_FontAsset>();
            TMPro.TMP_Settings.fallbackFontAssets.AddRange(fonts);
            var wqy = request.assetBundle.LoadAsset<TMPro.TMP_FontAsset>("wqy-microhei_CN SDF.asset");
            if (wqy) TMPro.TMP_Settings.defaultFontAsset.fallbackFontAssets.Insert(0, wqy);
            Logger.LogInfo($"Loaded Patch '{file}'");
        }

        Logger.LogInfo("Loading EventHandle");
        foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
        {
            if (!info.Metadata.GUID.Contains("znt")) continue;
            AnimationEventHandlerPatch.RegisterAnimationEvent(info.Instance.GetType().Assembly);
        }

        Traverse.Create(starter).Field<bool>("isLoading").Value = false;
        starter.LoadNextScene();
    }
}