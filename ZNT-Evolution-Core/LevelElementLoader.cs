﻿using System;
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
                var request = AssetBundle.LoadFromFileAsync(Path.Combine(path, "resources.bundle"));
                yield return request;
                bundle = request.assetBundle;
            }

            Logger.LogDebug($"resources.bundle -> {bundle} -> {bundle.GetAllAssetNames().Join()}");

            foreach (var name in bundle.GetAllAssetNames())
            {
                Logger.LogDebug($"[{bundle.name}] load path: {name}");
                var request = bundle.LoadAssetAsync(name);
                yield return request;
                var asset = request.asset;
                if (asset == null) continue;
                asset.hideFlags = HideFlags.HideAndDontSave;

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
                            LoadBrushFormFolder(path: path, bundle: bundle);
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
                            LoadDecorFormFolder(path: path, bundle: bundle);
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

        private static void LoadBrushFormFolder(string path, AssetBundle bundle)
        {
            if (bundle.LoadAsset("bank_") is TextAsset bank)
            {
                var fmod = AssetElementBinder.FetchFMODAsset(path: $"bank:/{bank.name}");
                Logger.LogDebug($"bank:/{bank.name} -> {fmod.Keys.Join()}");
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

            var brush = bundle.LoadAsset<Rotorz.Tile.OrientedBrush>("brush");
            var variation = brush.DefaultOrientation.GetVariation(0);
            Logger.LogDebug($"brush -> {brush.name} -> {variation.name}");

            var asset = DeserializeInfo<TagInfo>(folder: path, file: "asset.json");
            switch (asset)
            {
                case var _ when asset.Tag.HasFlag(Tag.Human):
                    var human = DeserializeAsset<HumanAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {human}");
                    break;
                case var _ when asset.Tag.HasFlag(Tag.WorldEnemy):
                    var enemy = DeserializeAsset<WorldEnemyAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {enemy}");
                    break;
                case var _ when asset.Tag.HasFlag(Tag.Zombie):
                    var zombie = DeserializeAsset<ZombieAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {zombie}");
                    break;
                case var _ when asset.Tag.HasFlag(Tag.Decor):
                    var decor = DeserializeAsset<DecorAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {decor}");
                    break;
                case var _ when asset.Tag.HasFlag(Tag.Interactable):
                    var breakable = DeserializeAsset<BreakablePropAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {breakable}");
                    break;
                case var _ when asset.Tag.HasFlag(Tag.Breakable):
                    var sentry = DeserializeAsset<SentryGunAsset>(folder: path, file: "asset.json");
                    Logger.LogDebug($"asset.json -> {sentry}");
                    break;
                case var _ when asset.Tag == 0:
                    var moving = DeserializeAsset<MovingObjectAsset>(folder: path, file: "asset.json");
                    Traverse.Create(moving).Field("library").SetValue(animation);
                    Logger.LogDebug($"asset.json -> {moving}");
                    break;
            }

            var element = DeserializeAsset<LevelElement>(folder: path, file: "element.json");
            Logger.LogDebug($"element.json -> {element} to {element.Title}");

            var id = AssetElementBinder.PushToIndex(element);
            Logger.LogInfo($"LevelElement {id} - {element.Title} Loaded");
        }

        private static void LoadDecorFormFolder(string path, AssetBundle bundle)
        {
            var material = bundle.LoadAsset<Material>("sprites");
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

                if (element.DecorType == LevelElement.DecorStyle.Animated)
                {
                    var id = AssetElementBinder.PushToIndex(element);
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

                    var id = AssetElementBinder.PushToIndex(impl);
                    Logger.LogInfo($"LevelElement {id} - {impl.Title} Loaded");
                }
            }
            else
            {
                var sprites = CreateSingleSprite(material);
                Logger.LogDebug($"CreateSingleSprite -> {sprites} from {sprites.material}");

                var element = DeserializeAsset<LevelElement>(folder: path, file: "element.json");
                Logger.LogDebug($"element.json -> {element} to {element.Title}");

                var id = AssetElementBinder.PushToIndex(element);
                Logger.LogInfo($"LevelElement {id} - {element.Title} Loaded");
            }
        }

        private static tk2dSpriteCollectionData CreateSingleSprite(Material material)
        {
            var impl = tk2dSpriteCollectionData.CreateFromTexture(
                texture: material.mainTexture,
                size: tk2dSpriteCollectionSize.Explicit(0.5F, 12),
                names: new[] { "single" },
                regions: new[] { new Rect(1, 1, material.mainTexture.width, material.mainTexture.height) },
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

            clone.name = material.name.Replace("_mat", "");
            clone.gameObject.hideFlags = HideFlags.HideAndDontSave;
            clone.material = material;
            clone.materials[0] = material;
            clone.textures[0] = material.mainTexture;
            foreach (var definition in clone.spriteDefinitions) definition.material = material;

            return clone;
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

            foreach (var file in Directory.EnumerateFiles(path: folder, searchPattern: "*.bank"))
            {
                var bank = Path.GetFileNameWithoutExtension(file);
                if (!bank.EndsWith(".strings")) continue;
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