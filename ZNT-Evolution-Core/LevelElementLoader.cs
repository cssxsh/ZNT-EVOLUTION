using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                            try
                            {
                                FMODUnity.RuntimeManager.LoadBank(asset: bank, loadSamples: true);
                            }
                            catch (FMODUnity.BankLoadException e)
                            {
                                Logger.LogError(e);
                            }
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
                    yield return LoadBrushFormFolder(path: path, bundle: bundle);
                    break;
                case LevelElement.Type.Decor:
                    yield return LoadDecorFormFolder(path: path, bundle: bundle);
                    break;
            }
        }

        private static IEnumerator<LevelElement> LoadBrushFormFolder(string path, AssetBundle bundle)
        {
            if (bundle.LoadAsset("bank_") is TextAsset bank)
            {
                var fmod = AssetElementBinder.FetchFMODAsset(path: $"bank:/{bank.name}");
                Logger.LogDebug($"bank:/{bank.name} -> {fmod.Keys.Join()}");
            }

            if (File.Exists(Path.Combine(path, "sprites.json")))
            {
                var sprites = CustomAssetUtility
                    .LoadComponentFromPath<tk2dSpriteCollectionData>(source: Path.Combine(path, "sprites.json"));
                Logger.LogDebug($"sprites.json -> {sprites} -> {sprites.materials[0]}");
            }

            if (File.Exists(Path.Combine(path, "sprite.info.json")))
            {
                var material = bundle.LoadAsset<Material>("sprites");
                var info = CustomAssetUtility
                    .DeserializeInfoFromPath<SpriteInfo>(source: Path.Combine(path, "sprite.info.json"));
                var sprites = CreateSprite(material, info);
                Logger.LogDebug($"CreateSprite -> {sprites} from {sprites.material}");
            }

            var animation = CustomAssetUtility
                .LoadComponentFromPath<tk2dSpriteAnimation>(source: Path.Combine(path, "animation.json"));
            Logger.LogDebug($"animation.json -> {animation}");

            var brush = bundle.LoadAsset<Rotorz.Tile.OrientedBrush>("brush");
            var variation = brush.DefaultOrientation.GetVariation(0);
            Logger.LogDebug($"brush -> {brush.name} -> {variation.name}");

            var asset = CustomAssetUtility
                .DeserializeInfoFromPath<TagInfo>(source: Path.Combine(path, "asset.json"));
            switch (asset.Tag)
            {
                case var tag when tag.HasFlag(Tag.Human):
                    var human = CustomAssetUtility
                        .DeserializeAssetFromPath<HumanAsset>(source: Path.Combine(path, "asset.json"));
                    Logger.LogDebug($"asset.json -> {human}");
                    break;
                case var tag when tag.HasFlag(Tag.WorldEnemy):
                    var enemy = CustomAssetUtility
                        .DeserializeAssetFromPath<WorldEnemyAsset>(source: Path.Combine(path, "asset.json"));
                    Logger.LogDebug($"asset.json -> {enemy}");
                    break;
                case var tag when tag.HasFlag(Tag.Zombie):
                    var zombie = CustomAssetUtility
                        .DeserializeAssetFromPath<ZombieAsset>(source: Path.Combine(path, "asset.json"));
                    Logger.LogDebug($"asset.json -> {zombie}");
                    break;
                case var tag when tag.HasFlag(Tag.Decor):
                    var decor = CustomAssetUtility
                        .DeserializeAssetFromPath<DecorAsset>(source: Path.Combine(path, "asset.json"));
                    Logger.LogDebug($"asset.json -> {decor}");
                    break;
                case var tag when tag.HasFlag(Tag.Interactable):
                    var breakable = CustomAssetUtility
                        .DeserializeAssetFromPath<BreakablePropAsset>(source: Path.Combine(path, "asset.json"));
                    Logger.LogDebug($"asset.json -> {breakable}");
                    break;
                case var tag when tag.HasFlag(Tag.Breakable):
                    var sentry = CustomAssetUtility
                        .DeserializeAssetFromPath<SentryGunAsset>(source: Path.Combine(path, "asset.json"));
                    Logger.LogDebug($"asset.json -> {sentry}");
                    break;
            }
            // TODO TriggerAsset MovingObjectAsset

            var element = CustomAssetUtility
                .DeserializeAssetFromPath<LevelElement>(source: Path.Combine(path, "element.json"));
            var id = AssetElementBinder.PushToIndex(element);
            Logger.LogDebug($"element.json -> {id} -> {element.Title}");

            yield return element;
        }

        private static IEnumerator<LevelElement> LoadDecorFormFolder(string path, AssetBundle bundle)
        {
            var material = bundle.LoadAsset<Material>("sprites");
            if (File.Exists(Path.Combine(path, "sprite.info.json")))
            {
                var info = CustomAssetUtility
                    .DeserializeInfoFromPath<SpriteInfo>(source: Path.Combine(path, "sprite.info.json"));
                var sprites = CreateSprite(material, info);
                Logger.LogDebug($"CreateSprite -> {sprites} from {sprites.material}");

                var element = CustomAssetUtility
                    .DeserializeAssetFromPath<LevelElement>(source: Path.Combine(path, "element.json"));
                Logger.LogDebug($"element.json -> {element} to {element.Title}");

                for (var index = 0; index < sprites.spriteDefinitions.Length; index++)
                {
                    var impl = Object.Instantiate(element);
                    impl.SpriteIndex = index;
                    impl.name = string.Format(element.name, index + 1, sprites.name);
                    impl.Title = string.Format(element.Title, index + 1, sprites.spriteDefinitions[index].name);
                    impl.hideFlags = HideFlags.HideAndDontSave;

                    var id = AssetElementBinder.PushToIndex(impl);
                    Logger.LogInfo($"LevelElement {id} - {impl.Title} Loaded");
                    yield return impl;
                }
            }
            else
            {
                var sprites = CreateSingleSprite(material);
                Logger.LogDebug($"CreateSingleSprite -> {sprites} from {sprites.material}");

                var element = CustomAssetUtility
                    .DeserializeAssetFromPath<LevelElement>(source: Path.Combine(path, "element.json"));
                Logger.LogDebug($"element.json -> {element} to {element.Title}");

                var id = AssetElementBinder.PushToIndex(element);
                Logger.LogInfo($"LevelElement {id} - {element.Title} Loaded");
                yield return element;
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

        public static IEnumerator LoadBanks(string folder, bool loadSamples = false)
        {
            var main = new List<string>();
            foreach (var file in Directory.EnumerateFiles(path: folder, searchPattern: "*.bank"))
            {
                var bank = Path.GetFileNameWithoutExtension(file);
                if (!bank.EndsWith(".strings")) continue;
                var master = bank.ReplaceLast(".strings", "");
                if (FMODUnity.Settings.Instance.MasterBanks.Contains(master)) continue;
                try
                {
                    FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: loadSamples);
                }
                catch (FMODUnity.BankLoadException e)
                {
                    Logger.LogWarning(e);
                    continue;
                }

                main.Add(item: bank.ReplaceLast(".strings", ""));
                yield return Wait.ForEndOfFrame;
            }

            foreach (var file in Directory.EnumerateFiles(path: folder, searchPattern: "*.bank"))
            {
                var bank = Path.GetFileNameWithoutExtension(file);
                if (bank.EndsWith(".strings")) continue;
                if (FMODUnity.Settings.Instance.MasterBanks.Contains(bank)) continue;
                if (FMODUnity.Settings.Instance.Banks.Contains(bank)) continue;
                if (main.Contains(bank)) continue;
                try
                {
                    Logger.LogInfo($"load Bank {bank}");
                    FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: loadSamples);
                }
                catch (FMODUnity.BankLoadException e)
                {
                    Logger.LogWarning(e);
                    continue;
                }

                var path = $"bank:/{bank}";
                foreach (var (_, asset) in AssetElementBinder.FetchFMODAsset(path: path))
                {
                    Logger.LogInfo($"[{bank}] fetch {asset.path}");
                }
                yield return Wait.ForEndOfFrame;
            }
        }
    }
}