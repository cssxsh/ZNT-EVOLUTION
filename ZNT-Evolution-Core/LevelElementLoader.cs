using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;

namespace ZNT.Evolution.Core
{
    public static class LevelElementLoader
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("LevelElementLoader");

        public static IEnumerator LoadFormFolder(string path, LevelElement.Type type)
        {
            if (!Directory.EnumerateFiles(path).Any())
            {
                Logger.LogWarning($"folder '{path}' does not exist or is empty.");
                yield break;
            }

            Logger.LogInfo($"load LevelElement form folder '{path}'.");

            AssetBundle bundle;
            {
                var file = Path.Combine(path, "resources.bundle");
                var request = AssetBundle.LoadFromFileAsync(file);
                yield return request;
                bundle = request.assetBundle;
                if (bundle == null)
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
                if (asset == null) continue;

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
                        break;
                    case TextAsset bank:
                        if (name == "bank.strings" || name == "bank_")
                        {
                            var task = Task.Run(() =>
                            {
                                try
                                {
                                    FMODUnity.RuntimeManager.LoadBank(asset: bank, loadSamples: true);
                                }
                                catch (FMODUnity.BankLoadException e)
                                {
                                    Logger.LogError(e);
                                }
                            });

                            yield return new WaitUntil(() => task.IsCompleted);
                        }

                        Logger.LogDebug($"[{bundle.name}] {asset.name}");
                        break;
                    default:
                        Logger.LogDebug($"[{bundle.name}] {request.asset.name}");
                        break;
                }
            }

            switch (type)
            {
                case LevelElement.Type.Brush:
                    var brush = Task.Run(() =>
                    {
                        try
                        {
                            bundle.LoadBrushFormFolder(path: path);
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning(e);
                        }
                    });
                    yield return new WaitUntil(() => brush.IsCompleted);
                    break;
                case LevelElement.Type.Decor:
                    var decor = Task.Run(() =>
                    {
                        try
                        {
                            bundle.LoadDecorFormFolder(path: path);
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning(e);
                        }
                    });
                    yield return new WaitUntil(() => decor.IsCompleted);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void LoadBrushFormFolder(this AssetBundle bundle, string path)
        {
            if (bundle.LoadAsset(name: "bank_") is TextAsset bank)
            {
                foreach (var (_, fmod) in AssetElementBinder.FetchFMODAsset(path: $"bank:/{bank.name}"))
                {
                    Logger.LogDebug($"[{bank.name}] fetch {fmod.path}");
                }
            }

            if (File.Exists(Path.Combine(path, "sprites.json")))
            {
                var sprites = LoadComponent<tk2dSpriteCollectionData>(folder: path, file: "sprites.json");
                Logger.LogDebug($"sprites.json -> {sprites} -> {sprites.materials[0]}");
            }

            if (File.Exists(Path.Combine(path, "sprite.info.json")))
            {
                var material = bundle.LoadAsset<Material>("sprites");
                var info = DeserializeInfo<SpriteInfo>(folder: path, file: "sprite.info.json");
                var sprites = CreateSprite(material, info);
                Logger.LogDebug($"CreateSprite -> {sprites} from {sprites.material}");
            }

            if (File.Exists(Path.Combine(path, "sprite.merge.json")))
            {
                var material = bundle.LoadAsset<Material>("sprites");
                var merge = DeserializeInfo<SpriteMerge>(folder: path, file: "sprite.merge.json");
                var sprites = MergeSprite(material, merge);
                Logger.LogDebug($"MergeSprite -> {sprites} from {sprites.material}");
            }

            var animation = LoadComponent<tk2dSpriteAnimation>(folder: path, file: "animation.json");
            Logger.LogDebug($"animation.json -> {animation}");

            var brush = bundle.LoadAsset<Rotorz.Tile.OrientedBrush>(name: "brush")
                        ?? CreateBrush(DeserializeInfo<BrushInfo>(folder: path, file: "brush.info.json"));
            var variation = brush.DefaultOrientation.GetVariation(0);
            Logger.LogDebug($"brush -> {brush.name} -> {variation.name}");

            var asset = DeserializeInfo<LevelElementInfo>(folder: path, file: "element.json").CustomAsset;
            switch (asset)
            {
                case null:
                    break;
                case var _ when asset.EndsWith("HumanAsset"):
                    var human = DeserializeAsset<HumanAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {human}");
                    break;
                case var _ when asset.EndsWith("WorldEnemyAsset"):
                    var enemy = DeserializeAsset<WorldEnemyAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {enemy}");
                    break;
                case var _ when asset.EndsWith("ZombieAsset"):
                    var zombie = DeserializeAsset<ZombieAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {zombie}");
                    break;
                case var _ when asset.EndsWith("DecorAsset"):
                    var decor = DeserializeAsset<DecorAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {decor}");
                    if (animation.GetClipByName(decor.ActivateAnimation) == null)
                    {
                        var source = DeserializeAsset<LevelElement>(folder: path, file: "element.json");
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
                    var breakable = DeserializeAsset<BreakablePropAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {breakable}");
                    break;
                case var _ when asset.EndsWith("SentryGunAsset"):
                    var sentry = DeserializeAsset<SentryGunAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {sentry}");
                    break;
                case var _ when asset.EndsWith("MovingObjectAsset"):
                    var moving = DeserializeAsset<MovingObjectAsset>(folder: path, file: "asset.json");
                    Traverse.Create(moving).Field("library").SetValue(animation);
                    Logger.LogDebug($"asset.json -> {moving}");
                    break;
                case var _ when asset.EndsWith("PhysicObjectAsset"):
                    var physic = DeserializeAsset<PhysicObjectAsset>(folder: path, file: "asset.json");
                    Traverse.Create(physic).Field("library").SetValue(animation);
                    Logger.LogDebug($"asset.json -> {physic}");
                    break;
                case var _ when asset.EndsWith("TriggerAsset"):
                    var trigger = DeserializeAsset<TriggerAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {trigger}");
                    break;
                case var _ when asset.EndsWith("DetectionAsset"):
                    var detection = DeserializeAsset<DetectionAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {detection}");
                    break;
                case var _ when asset.EndsWith("ExplosionAsset"):
                    var explosion = DeserializeAsset<ExplosionAsset>(folder: path, file: "asset.json");
                    Traverse.Create(explosion).Field("autoExplode").SetValue(false);
                    Logger.LogDebug($"asset.json -> {explosion}");
                    break;
                case var _ when asset.EndsWith("ScreamAsset"):
                    var scream = DeserializeAsset<ScreamAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {scream}");
                    break;
            }

            var element = DeserializeAsset<LevelElement>(folder: path, file: "element.json");
            Logger.LogDebug($"element.json -> {element} to {element.Title}");

            var id = element.Bind();
            Logger.LogInfo($"LevelElement {id} - {element.Title} Loaded");
        }

        private static void LoadDecorFormFolder(this AssetBundle bundle, string path)
        {
            var material = bundle.LoadAsset<Material>(name: "sprites");
            if (File.Exists(Path.Combine(path, "sprite.info.json")))
            {
                var info = DeserializeInfo<SpriteInfo>(folder: path, file: "sprite.info.json");
                var sprites = CreateSprite(material, info);
                Logger.LogDebug($"CreateSprite -> {sprites} from {sprites.material}");

                if (File.Exists(Path.Combine(path, "animation.json")))
                {
                    var animation = LoadComponent<tk2dSpriteAnimation>(folder: path, file: "animation.json");
                    Logger.LogDebug($"animation.json -> {animation}");
                }

                var element = DeserializeAsset<LevelElement>(folder: path, file: "element.json");
                Logger.LogDebug($"element.json -> {element} to {element.Title}");

                if (element.Brush != null)
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
                var sprites = CreateSingleSprite(material);
                Logger.LogDebug($"CreateSingleSprite -> {sprites} from {sprites.material}");

                var element = DeserializeAsset<LevelElement>(folder: path, file: "element.json");
                Logger.LogDebug($"element.json -> {element} to {element.Title}");

                var id = element.Bind();
                Logger.LogInfo($"LevelElement {id} - {element.Title} Loaded");
            }
        }

        public static IEnumerator ApplyFormFolder(string path)
        {
            if (!Directory.EnumerateFiles(path).Any())
            {
                Logger.LogWarning($"folder '{path}' does not exist or is empty.");
                yield break;
            }

            Logger.LogInfo($"apply LevelElement form folder '{path}'.");

            AssetBundle bundle;
            {
                var file = Path.Combine(path, "resources.bundle");
                var request = AssetBundle.LoadFromFileAsync(file);
                yield return request;
                bundle = request.assetBundle;
                if (bundle == null)
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
                if (asset == null) continue;

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
                    case TextAsset bank:
                        if (name == "bank.strings" || name == "bank_")
                        {
                            var task = Task.Run(() =>
                            {
                                try
                                {
                                    FMODUnity.RuntimeManager.LoadBank(asset: bank, loadSamples: true);
                                }
                                catch (FMODUnity.BankLoadException e)
                                {
                                    Logger.LogError(e);
                                }
                            });

                            yield return new WaitUntil(() => task.IsCompleted);
                        }

                        Logger.LogDebug($"[{bundle.name}] {asset.name}");
                        break;
                    default:
                        Logger.LogDebug($"[{bundle.name}] {request.asset.name}");
                        break;
                }
            }

            var apply = Task.Run(() =>
            {
                try
                {
                    bundle.ApplyFormFolder(path: path);
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e);
                }
            });
            yield return new WaitUntil(() => apply.IsCompleted);
        }

        private static void ApplyFormFolder(this AssetBundle bundle, string path)
        {
            var material = bundle.LoadAsset<Material>("sprites");
            var info = DeserializeInfo<SpriteInfo>(folder: path, file: "sprite.info.json");
            var sprites = CreateSprite(material, info);
            Logger.LogDebug($"CreateSprite -> {sprites} from {sprites.material}");

            var addition = DeserializeInfo<AnimationAddition>(folder: path, file: "animation.addition.json");
            var animations = addition.Apply();
            Logger.LogInfo($"{animations.Count} animations apply");
        }

        private static tk2dSpriteCollectionData CreateSingleSprite(Material material)
        {
            var impl = tk2dSpriteCollectionData.CreateFromTexture(
                texture: material.mainTexture,
                size: tk2dSpriteCollectionSize.Explicit(0.5F, 12),
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

            return impl;
        }

        private static tk2dSpriteCollectionData CreateSprite(Material material, SpriteInfo info)
        {
            var impl = tk2dSpriteCollectionData.CreateFromTexture(
                texture: material.mainTexture,
                size: tk2dSpriteCollectionSize.Explicit(orthoSize: info.OrthoSize, targetHeight: info.TargetHeight),
                names: info.Names,
                regions: info.Regions,
                anchors: info.Anchors
            );

            impl.name = material.name.Replace("_mat", "");
            impl.gameObject.hideFlags = HideFlags.HideAndDontSave;
            impl.material = material;
            impl.materials[0] = material;
            foreach (var definition in impl.spriteDefinitions) definition.material = material;

            return impl;
        }

        private static tk2dSpriteCollectionData MergeSprite(Material material, SpriteMerge merge)
        {
            var clone = UnityEngine.Object.Instantiate(merge.Source);

            clone.name = merge.Name ?? material.name.Replace("_mat", "");
            clone.gameObject.hideFlags = HideFlags.HideAndDontSave;
            clone.material = material;
            clone.materials[0] = material;
            clone.textures[0] = material.mainTexture;
            foreach (var definition in clone.spriteDefinitions) definition.material = material;

            return clone;
        }

        private static Rotorz.Tile.OrientedBrush CreateBrush(BrushInfo info)
        {
            var impl = ScriptableObject.CreateInstance<Rotorz.Tile.OrientedBrush>();

            impl.hideFlags = HideFlags.HideAndDontSave;
            impl.name = info.Name;
            impl.group = 1;
            impl.forceLegacySideways = info.ForceLegacySideways;
            impl.applyPrefabTransform = info.ApplyPrefabTransform;
            Traverse.Create(impl).Field("_userFlags").SetValue(info.UserFlags);
            impl.forceOverrideFlags = info.ForceOverrideFlags;
            impl.Coalesce = info.Coalesce;
            impl.AddOrientation(mask: 0).AddVariation(variation: info.Variation, weight: 50);

            return impl;
        }

        private static ISet<tk2dSpriteAnimation> Apply(this AnimationAddition addition)
        {
            var animations = Resources.FindObjectsOfTypeAll<tk2dSpriteAnimation>();
            return addition.Clips.Zip(addition.Targets, (clip, name) =>
            {
                foreach (var animation in animations)
                {
                    if (animation.name != name) continue;
                    animation.clips = animation.clips.AddToArray(clip);
                    Traverse.Create(animation).Field("clipNameCache").SetValue(null);
                    Traverse.Create(animation).Field("idNameCache").SetValue(null);
                    animation.InitializeClipCache();
                    return animation;
                }

                throw new KeyNotFoundException(message: $"tk2dSpriteAnimation(name: {name})");
            }).ToHashSet();
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

            return impl;
        }

        private static T DeserializeAsset<T>(string folder, string file) where T : CustomAsset
        {
            return CustomAssetUtility.DeserializeAssetFromPath<T>(source: Path.Combine(folder, file));
        }

        private static T DeserializeInfo<T>(string folder, string file) where T : EvolutionInfo
        {
            return CustomAssetUtility.DeserializeInfoFromPath<T>(source: Path.Combine(folder, file));
        }

        private static T LoadComponent<T>(string folder, string file) where T : Component
        {
            return CustomAssetUtility.LoadComponentFromPath<T>(source: Path.Combine(folder, file));
        }

        public static IEnumerator LoadBanks(string folder, bool loadSamples = false)
        {
            var main = new List<string>();

            foreach (var file in Directory.EnumerateFiles(path: folder, searchPattern: "*.strings.bank"))
            {
                var bank = Path.GetFileNameWithoutExtension(file);
                var master = bank.ReplaceLast(".strings", "");
                if (FMODUnity.Settings.Instance.MasterBanks.Contains(master)) continue;
                var task = Task.Run(() =>
                {
                    try
                    {
                        FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: loadSamples);
                    }
                    catch (FMODUnity.BankLoadException e)
                    {
                        Logger.LogWarning(e);
                    }
                });
                yield return new WaitUntil(() => task.IsCompleted);

                main.Add(item: bank.ReplaceLast(".strings", ""));
            }

            foreach (var file in Directory.EnumerateFiles(path: folder, searchPattern: "*.bank"))
            {
                var bank = Path.GetFileNameWithoutExtension(file);
                if (bank.EndsWith(".strings")) continue;
                if (FMODUnity.Settings.Instance.MasterBanks.Contains(bank)) continue;
                if (FMODUnity.Settings.Instance.Banks.Contains(bank)) continue;
                if (main.Contains(bank)) continue;
                var task = Task.Run(() =>
                {
                    try
                    {
                        Logger.LogInfo($"load Bank {bank}");
                        FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: loadSamples);
                        foreach (var (_, asset) in AssetElementBinder.FetchFMODAsset(path: $"bank:/{bank}"))
                        {
                            Logger.LogInfo($"[{bank}] fetch {asset.path}");
                        }
                    }
                    catch (FMODUnity.BankLoadException e)
                    {
                        Logger.LogWarning(e);
                    }
                });
                yield return new WaitUntil(() => task.IsCompleted);
            }
        }
    }
}