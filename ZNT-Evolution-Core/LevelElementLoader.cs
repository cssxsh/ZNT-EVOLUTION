using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;

namespace ZNT.Evolution.Core;

public static class LevelElementLoader
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(LevelElementLoader));

    public static IEnumerator LoadFromFolder(string path, LevelElement.Type type)
    {
        if (!Directory.EnumerateFiles(path).Any())
        {
            Logger.LogWarning($"folder '{path}' does not exist or is empty.");
            yield break;
        }

        Logger.LogInfo($"load LevelElement from folder '{path}'.");

        AssetBundle bundle;
        {
            var file = Path.Combine(path, "resources.bundle");
            var request = AssetBundle.LoadFromFileAsync(file);
            yield return request;
            bundle = request.assetBundle;
            if (bundle is null)
            {
                Logger.LogWarning($"AssetBundle '{file}' cannot read");
                yield break;
            }
        }

        Logger.LogDebug($"resources.bundle -> {bundle} -> {bundle.GetAllAssetNames().Join()}");

        foreach (var name in bundle.GetAllAssetNames())
        {
            Logger.LogDebug($"[{bundle.name}] load path: {name}");
            var request = bundle.LoadAssetAsync(name);
            yield return request;
            var asset = request.asset;
            if (asset is null) continue;

            switch (asset)
            {
                case Sprite sprite:
                    Logger.LogDebug($"[{bundle.name}] {sprite}");
                    Logger.LogDebug($"[{bundle.name}] {sprite.texture}");
                    break;
                case Material material:
                    Logger.LogDebug($"[{bundle.name}] {material}");
                    Logger.LogDebug($"[{bundle.name}] {material.mainTexture}");
                    Logger.LogDebug($"[{bundle.name}] {material.shader}");
                    break;
                case Shader shader:
                    Logger.LogDebug($"[{bundle.name}] {shader}");
                    break;
                case Rotorz.Tile.OrientedBrush brush:
                    Logger.LogDebug($"[{bundle.name}] {brush}");
                    Logger.LogDebug($"[{bundle.name}] {brush.DefaultOrientation.GetVariation(0)}");
                    break;
                case TextAsset bank when name == "bank.strings" || name == "bank_":
                    FMODUnity.RuntimeManager.LoadBank(asset: bank, loadSamples: true);
                    yield return new WaitUntil(() => !FMODUnity.RuntimeManager.AnyBankLoading());
                    Logger.LogDebug($"[{bundle.name}] {asset.name}");
                    break;
                default:
                    Logger.LogDebug($"[{bundle.name}] {asset.name}");
                    break;
            }

            UnityEngine.Object.DontDestroyOnLoad(asset);
        }

        try
        {
            switch (type)
            {
                case LevelElement.Type.Brush:
                    bundle.LoadBrushFromFolder(path: path);
                    break;
                case LevelElement.Type.Decor:
                    bundle.LoadDecorFromFolder(path: path);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        catch (Exception e)
        {
            Logger.LogWarning(e);
        }
    }

    private static void LoadBrushFromFolder(this AssetBundle bundle, string path)
    {
        if (bundle.LoadAsset("bank_") is TextAsset bank)
        {
            foreach (var (_, fmod) in AssetElementBinder.FetchFMODAsset(path: $"bank:/{bank.name}"))
            {
                Logger.LogDebug($"[{bank.name}] fetch {fmod.path}");
            }
        }

        if (File.Exists(Path.Combine(path, "sprites.json")))
        {
            var sprites = DeserializeObject<tk2dSpriteCollectionData>(folder: path, file: "sprites.json");
            Logger.LogDebug($"sprites.json -> {sprites} -> {sprites.materials[0]}");
        }

        if (File.Exists(Path.Combine(path, "sprite.info.json")))
        {
            var material = bundle.LoadAsset<Material>("sprites");
            var info = DeserializeObject<SpriteInfo>(folder: path, file: "sprite.info.json");
            var sprites = info.WithMaterial(material).Create();
            Logger.LogDebug($"sprite.info.json -> {sprites} from {sprites.material}");
        }

        if (File.Exists(Path.Combine(path, "sprite.merge.json")))
        {
            var material = bundle.LoadAsset<Material>("sprites");
            var merge = DeserializeObject<SpriteMerge>(folder: path, file: "sprite.merge.json");
            var sprites = merge.WithMaterial(material).Create();
            Logger.LogDebug($"sprite.merge.json -> {sprites} from {sprites.material}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.sprite.info.json"))
        {
            var filename = Path.GetFileName(file);
            var info = DeserializeObject<SpriteInfo>(folder: path, file: filename);
            var sprites = info.Create();
            Logger.LogDebug($"{filename} -> {sprites} from {sprites.material}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.sprite.merge.json"))
        {
            var filename = Path.GetFileName(file);
            var merge = DeserializeObject<SpriteMerge>(folder: path, file: file);
            var sprites = merge.Create();
            Logger.LogDebug($"{filename} -> {sprites} from {sprites.material}");
        }

        var animation = DeserializeObject<tk2dSpriteAnimation>(folder: path, file: "animation.json");
        Logger.LogDebug($"animation.json -> {animation}");

        foreach (var file in Directory.EnumerateFiles(path, "*.animations.json"))
        {
            var filename = Path.GetFileName(file);
            var animations = DeserializeObject<CharacterAnimationAsset>(folder: path, file: file);
            Logger.LogDebug($"{filename} -> {animations}");
        }

        var brush = bundle.LoadAsset<Rotorz.Tile.OrientedBrush>("brush")
                    ?? DeserializeObject<BrushInfo>(folder: path, file: "brush.info.json").Create();
        var variation = brush.DefaultOrientation.GetVariation(0);
        Logger.LogDebug($"brush -> {brush} -> {variation}");

        var preview = bundle.LoadAsset<Sprite>("preview");
        if (preview) Logger.LogDebug($"preview -> {preview} -> {preview.texture}");

        foreach (var file in Directory.EnumerateFiles(path, "*.object.merge.json"))
        {
            var filename = Path.GetFileName(file);
            var merge = DeserializeObject<ObjectMerge>(folder: path, file: filename);
            var component = merge.Create();
            Logger.LogDebug($"{filename} -> {component}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.explosion.json"))
        {
            var filename = Path.GetFileName(file);
            var explosion = DeserializeObject<ExplosionAsset>(folder: path, file: filename);
            Logger.LogDebug($"{filename} -> {explosion}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.physic.json"))
        {
            var filename = Path.GetFileName(file);
            var physic = DeserializeObject<PhysicObjectAsset>(folder: path, file: file);
            Logger.LogDebug($"{filename} -> {physic} from {physic.Explosion}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.human.json"))
        {
            var filename = Path.GetFileName(file);
            var human = DeserializeObject<HumanAsset>(folder: path, file: file);
            Logger.LogDebug($"{filename} -> {human} from {human.AnimationLibrary}");
        }

        var asset = DeserializeObject<LevelElementInfo>(folder: path, file: "element.json").CustomAsset;
        if (!File.Exists(Path.Combine(path, "asset.json"))) asset = null;
        switch (asset)
        {
            case null:
                break;
            case var _ when asset.EndsWith("HumanAsset"):
                var human = DeserializeObject<HumanAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {human} from {human.AnimationLibrary}");
                break;
            case var _ when asset.EndsWith("WorldEnemyAsset"):
                var enemy = DeserializeObject<WorldEnemyAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {enemy} from {enemy.AnimationLibrary}");
                break;
            case var _ when asset.EndsWith("ZombieAsset"):
                var zombie = DeserializeObject<ZombieAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {zombie} from {zombie.AnimationLibrary}");
                break;
            case var _ when asset.EndsWith("DecorAsset"):
                var decor = DeserializeObject<DecorAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {decor} from {decor.Animation}");
                if (animation.GetClipByName(decor.ActivateAnimation) == null)
                {
                    var source = DeserializeObject<LevelElement>(folder: path, file: "element.json");
                    Logger.LogDebug($"element.json -> {source} to {source.Title}");
                    for (var index = 0; index < animation.clips.Length; index++)
                    {
                        var clone = UnityEngine.Object.Instantiate(decor);
                        clone.name = string.Format(decor.name, index + 1);
                        clone.ActivateAnimation = string.Format(decor.ActivateAnimation, index + 1);
                        if (animation.GetClipByName(clone.ActivateAnimation) == null) break;
                        clone.DeactivateAnimation = string.Format(decor.DeactivateAnimation, index + 1);
                        if (animation.GetClipByName(clone.DeactivateAnimation) == null) break;
                        clone.ActiveAnimation = string.Format(decor.ActiveAnimation, index + 1);
                        if (animation.GetClipByName(clone.ActiveAnimation) == null) break;
                        clone.InactiveAnimation = string.Format(decor.InactiveAnimation, index + 1);
                        if (animation.GetClipByName(clone.InactiveAnimation) == null) break;

                        var impl = UnityEngine.Object.Instantiate(source);
                        impl.name = string.Format(source.name, index + 1);
                        impl.Title = string.Format(source.Title, index + 1);
                        impl.Brush = UnityEngine.Object.Instantiate(source.Brush);
                        impl.Brush.name = $"brush_{impl.name}";
                        impl.CustomAsset = clone;

                        var i = impl.Bind();
                        Logger.LogInfo($"LevelElement {i} - {impl.Title} Loaded");
                    }

                    UnityEngine.Object.Destroy(decor);
                    UnityEngine.Object.Destroy(source);
                    return;
                }

                break;
            case var _ when asset.EndsWith("BreakablePropAsset"):
                var breakable = DeserializeObject<BreakablePropAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {breakable} from {breakable.Animation}");
                break;
            case var _ when asset.EndsWith("SentryGunAsset"):
                var sentry = DeserializeObject<SentryGunAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {sentry} from {sentry.Animation}");
                break;
            case var _ when asset.EndsWith("MovingObjectAsset"):
                var moving = DeserializeObject<MovingObjectAsset>(folder: path, file: "asset.json");
                Traverse.Create(moving).Field<tk2dSpriteAnimation>("library").Value ??= animation;
                Logger.LogDebug($"asset.json -> {moving}");
                break;
            case var _ when asset.EndsWith("PhysicObjectAsset"):
                var physic = DeserializeObject<PhysicObjectAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {physic}");
                break;
            case var _ when asset.EndsWith("TriggerAsset"):
                var trigger = DeserializeObject<TriggerAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {trigger} from {trigger.Animation}");
                break;
            case var _ when asset.EndsWith("DetectionAsset"):
                var detection = DeserializeObject<DetectionAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {detection}");
                break;
            case var _ when asset.EndsWith("ExplosionAsset"):
                var explosion = DeserializeObject<ExplosionAsset>(folder: path, file: "asset.json");
                Traverse.Create(explosion).Field<bool>("autoExplode").Value = false;
                Logger.LogDebug($"asset.json -> {explosion}");
                break;
            case var _ when asset.EndsWith("ScreamAsset"):
                var scream = DeserializeObject<ScreamAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {scream}");
                break;
            case var _ when asset.EndsWith("SpawnPointAsset"):
                var spawn = DeserializeObject<SpawnPointAsset>(folder: path, file: "asset.json");
                Logger.LogDebug($"asset.json -> {spawn}");
                break;
        }

        var element = DeserializeObject<LevelElement>(folder: path, file: "element.json");
        Logger.LogDebug($"element.json -> {element} to {element.Title}");

        var id = element.Bind();
        Logger.LogInfo($"LevelElement {id} - {element.Title} Loaded");
    }

    private static void LoadDecorFromFolder(this AssetBundle bundle, string path)
    {
        var material = bundle.LoadAsset<Material>("sprites");
        if (File.Exists(Path.Combine(path, "sprite.info.json")))
        {
            var info = DeserializeObject<SpriteInfo>(folder: path, file: "sprite.info.json");
            var sprites = info.WithMaterial(material).Create();
            Logger.LogDebug($"sprite.info.json -> {sprites} from {sprites.material}");

            if (File.Exists(Path.Combine(path, "animation.json")))
            {
                var animation = DeserializeObject<tk2dSpriteAnimation>(folder: path, file: "animation.json");
                Logger.LogDebug($"animation.json -> {animation}");
            }

            var element = DeserializeObject<LevelElement>(folder: path, file: "element.json");
            Logger.LogDebug($"element.json -> {element} to {element.Title}");

            if (element.Brush)
            {
                var brush = element.DecorToBrush();
                var id = brush.Bind();
                Logger.LogInfo($"LevelElement {id} - {brush} Loaded");
            }

            if (element.DecorType == LevelElement.DecorStyle.Animated)
            {
                var id = element.Bind();
                Logger.LogInfo($"LevelElement {id} - {element} Loaded");
                return;
            }

            for (var index = 0; index < sprites.spriteDefinitions.Length; index++)
            {
                var impl = UnityEngine.Object.Instantiate(element);
                impl.SpriteIndex = index;
                impl.name = string.Format(element.name, index + 1, sprites.name);
                impl.Title = string.Format(element.Title, index + 1, sprites.spriteDefinitions[index].name);
                impl.hideFlags = HideFlags.HideAndDontSave;

                var id = impl.Bind();
                Logger.LogInfo($"LevelElement {id} - {impl.Title} Loaded");
            }

            UnityEngine.Object.Destroy(element);
        }
        else
        {
            var sprites = material.CreateSingleSprite();
            Logger.LogDebug($"CreateSingleSprite -> {sprites} from {sprites.material}");

            var element = DeserializeObject<LevelElement>(folder: path, file: "element.json");
            Logger.LogDebug($"element.json -> {element} to {element.Title}");

            var id = element.Bind();
            Logger.LogInfo($"LevelElement {id} - {element.Title} Loaded");
        }
    }

    public static IEnumerator ApplyFromFolder(string path)
    {
        if (!Directory.EnumerateFiles(path).Any())
        {
            Logger.LogWarning($"folder '{path}' does not exist or is empty.");
            yield break;
        }

        Logger.LogInfo($"apply LevelElement from folder '{path}'.");

        AssetBundle bundle;
        {
            var file = Path.Combine(path, "resources.bundle");
            var request = AssetBundle.LoadFromFileAsync(file);
            yield return request;
            bundle = request.assetBundle;
            if (bundle is null)
            {
                Logger.LogWarning($"AssetBundle '{file}' cannot read");
                yield break;
            }
        }

        Logger.LogDebug($"resources.bundle -> {bundle} -> {bundle.GetAllAssetNames().Join()}");

        foreach (var name in bundle.GetAllAssetNames())
        {
            Logger.LogDebug($"[{bundle.name}] load path: {name}");
            var request = bundle.LoadAssetAsync(name);
            yield return request;
            var asset = request.asset;
            if (asset is null) continue;

            switch (asset)
            {
                case Sprite sprite:
                    Logger.LogDebug($"[{bundle.name}] {sprite}");
                    Logger.LogDebug($"[{bundle.name}] {sprite.texture}");
                    break;
                case Material material:
                    Logger.LogDebug($"[{bundle.name}] {material}");
                    Logger.LogDebug($"[{bundle.name}] {material.mainTexture}");
                    Logger.LogDebug($"[{bundle.name}] {material.shader}");
                    break;
                case Shader shader:
                    Logger.LogDebug($"[{bundle.name}] {shader}");
                    break;
                case TextAsset bank when name == "bank.strings" || name == "bank_":
                    FMODUnity.RuntimeManager.LoadBank(asset: bank, loadSamples: true);
                    yield return new WaitUntil(() => !FMODUnity.RuntimeManager.AnyBankLoading());
                    Logger.LogDebug($"[{bundle.name}] {asset.name}");
                    break;
                default:
                    Logger.LogDebug($"[{bundle.name}] {asset.name}");
                    break;
            }
        }

        foreach (var addition in Directory.EnumerateFiles(path, "*.addition.json"))
        {
            try
            {
                switch (Path.GetFileName(addition))
                {
                    case "animation.addition.json":
                        bundle.ApplyAnimationFromFolder(path: path);
                        break;
                    case "element.addition.json":
                        bundle.ApplyElementFromFolder(path: path);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(e);
            }
        }
    }

    private static void ApplyAnimationFromFolder(this AssetBundle bundle, string path)
    {
        if (File.Exists(Path.Combine(path, "sprite.info.json")))
        {
            var material = bundle.LoadAsset<Material>("sprites");
            var info = DeserializeObject<SpriteInfo>(folder: path, file: "sprite.info.json");
            var sprites = info.WithMaterial(material).Create();
            Logger.LogDebug($"sprite.info.json -> {sprites} from {sprites.material}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.sprite.info.json"))
        {
            var filename = Path.GetFileName(file);
            var info = DeserializeObject<SpriteInfo>(folder: path, file: filename);
            var sprites = info.Create();
            Logger.LogDebug($"{filename} -> {sprites} from {sprites.material}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.sprite.merge.json"))
        {
            var filename = Path.GetFileName(file);
            var merge = DeserializeObject<SpriteMerge>(folder: path, file: file);
            var sprites = merge.Create();
            Logger.LogDebug($"{filename} -> {sprites} from {sprites.material}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.animations.json"))
        {
            var filename = Path.GetFileName(file);
            var animations = DeserializeObject<CharacterAnimationAsset>(folder: path, file: file);
            Logger.LogDebug($"{filename} -> {animations}");
        }

        var animation = DeserializeObject<AnimationAddition>(folder: path, file: "animation.addition.json");
        animation.Apply();
        Logger.LogInfo($"{animation.Targets.Length} animations apply");
    }

    private static void ApplyElementFromFolder(this AssetBundle _, string path)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.object.merge.json"))
        {
            var filename = Path.GetFileName(file);
            var merge = DeserializeObject<ObjectMerge>(folder: path, file: filename);
            var component = merge.Create();
            Logger.LogDebug($"{filename} -> {component}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.explosion.json"))
        {
            var filename = Path.GetFileName(file);
            var explosion = DeserializeObject<ExplosionAsset>(folder: path, file: filename);
            Logger.LogDebug($"{filename} -> {explosion}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.physic.json"))
        {
            var filename = Path.GetFileName(file);
            var physic = DeserializeObject<PhysicObjectAsset>(folder: path, file: file);
            Logger.LogDebug($"{filename} -> {physic} from {physic.Explosion}");
        }

        var element = DeserializeObject<LevelElementAddition>(folder: path, file: "element.addition.json");
        element.Apply();
        Logger.LogInfo($"{element.Targets.Length} elements apply");
    }

    public static void LoadAssetFromFolder(string path)
    {
        Logger.LogInfo($"load Asset from folder '{path}'.");

        foreach (var file in Directory.EnumerateFiles(path, "*.sprite.info.json"))
        {
            var filename = Path.GetFileName(file);
            var info = DeserializeObject<SpriteInfo>(folder: path, file: filename);
            var sprites = info.Create();
            Logger.LogDebug($"{filename} -> {sprites} from {sprites.material}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.sprite.merge.json"))
        {
            var filename = Path.GetFileName(file);
            var merge = DeserializeObject<SpriteMerge>(folder: path, file: file);
            var sprites = merge.Create();
            Logger.LogDebug($"{filename} -> {sprites} from {sprites.material}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.animations.json"))
        {
            var filename = Path.GetFileName(file);
            var animations = DeserializeObject<CharacterAnimationAsset>(folder: path, file: file);
            Logger.LogDebug($"{filename} -> {animations}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.explosion.json"))
        {
            var filename = Path.GetFileName(file);
            var explosion = DeserializeObject<ExplosionAsset>(folder: path, file: filename);
            Logger.LogDebug($"{filename} -> {explosion}");
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.physic.json"))
        {
            var filename = Path.GetFileName(file);
            var physic = DeserializeObject<PhysicObjectAsset>(folder: path, file: file);
            Logger.LogDebug($"{filename} -> {physic} from {physic.Explosion}");
        }
    }

    private static tk2dSpriteCollectionData CreateSingleSprite(this Material material)
    {
        var impl = tk2dSpriteCollectionData.CreateFromTexture(
            texture: material.mainTexture,
            size: tk2dSpriteCollectionSize.Explicit(0.5f, 12.0f),
            names: new[] { "single" },
            regions: new[] { new Rect(0, 0, material.mainTexture.width, material.mainTexture.height) },
            anchors: new[] { Vector2.zero }
        );

        impl.name = material.name.Replace("_mat", "");
        impl.gameObject.hideFlags = HideFlags.HideAndDontSave;
        impl.material = material;
        impl.materials[0] = material;
        impl.spriteDefinitions[0].material = material;
        impl.spriteDefinitions[0].name = impl.name.Replace("sprites_", "");

        UnityEngine.Object.DontDestroyOnLoad(impl);
        return impl;
    }

    private static LevelElement DecorToBrush(this LevelElement origin)
    {
        var impl = UnityEngine.Object.Instantiate(origin);
        impl.name = origin.name + "_tiles";
        impl.hideFlags = HideFlags.HideAndDontSave;
        impl.ElementType = LevelElement.Type.Brush;

        impl.CustomAsset = HookAsset.Invoke(body =>
        {
            Logger.LogWarning($"{impl.name} Asset Hook!");
            var animator = body.GetComponentInChildren<tk2dSpriteAnimator>();
            animator.Library = impl.AnimationLibrary;
            animator.DefaultClipId = impl.AnimClipId;
            animator.Sprite.SetSprite(impl.SpriteCollection, impl.SpriteIndex);
        });

        impl.AddIdentifier = true;
        impl.AddColliderInEditor = true;
        impl.AddObjectSettings = true;

        UnityEngine.Object.DontDestroyOnLoad(impl);
        return impl;
    }

    private static T DeserializeObject<T>(string folder, string file)
    {
        return CustomAssetUtility.DeserializeObjectFromPath<T>(source: Path.Combine(folder, file));
    }

    public static IEnumerator LoadBanks(string folder, bool loadSamples = false)
    {
        var main = new List<string>();

        foreach (var file in Directory.EnumerateFiles(path: folder, searchPattern: "*.strings.bank"))
        {
            var bank = Path.GetFileNameWithoutExtension(file);
            var master = bank.ReplaceLast(".strings", "");
            if (FMODUnity.Settings.Instance.MasterBanks.Contains(master)) continue;
            main.Add(master);
            Logger.LogInfo($"load Bank {bank}");
            FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: loadSamples);
            yield return new WaitUntil(() => !FMODUnity.RuntimeManager.AnyBankLoading());
        }

        foreach (var file in Directory.EnumerateFiles(path: folder, searchPattern: "*.bank"))
        {
            var bank = Path.GetFileNameWithoutExtension(file);
            if (bank.EndsWith(".strings")) continue;
            if (FMODUnity.Settings.Instance.MasterBanks.Contains(bank)) continue;
            if (FMODUnity.Settings.Instance.Banks.Contains(bank)) continue;
            if (main.Contains(bank)) continue;
            Logger.LogInfo($"load Bank {bank}");
            FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: loadSamples);
            yield return new WaitUntil(() => !FMODUnity.RuntimeManager.AnyBankLoading());
            foreach (var (_, asset) in AssetElementBinder.FetchFMODAsset(path: $"bank:/{bank}"))
            {
                Logger.LogInfo($"[{bank}] fetch {asset.path}");
            }
        }
    }
}