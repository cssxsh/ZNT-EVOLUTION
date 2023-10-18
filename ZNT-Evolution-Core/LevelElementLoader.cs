using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;

namespace ZNT.Evolution.Core
{
    public static class LevelElementLoader
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("LevelElementLoader");

        public static Dictionary<string, LevelElement> LoadFormFolder(string path, LevelElement.Type type)
        {
            if (Directory.Exists(path) && Directory.EnumerateFiles(path).Any())
            {
                Logger.LogInfo($"load form folder '{path}'.");
            }
            else
            {
                Logger.LogInfo($"folder '{path}' does not exist or is empty.");
                return new Dictionary<string, LevelElement>(0);
            }
            
            lock (typeof(LevelElementLoader))
            {
                switch (type)
                {
                    case LevelElement.Type.Brush:
                        return LoadBrushFormFolder(path: path);
                    case LevelElement.Type.Decor:
                        return LoadDecorFormFolder(path: path);
                    default:
                        return null;
                }
            }
        }

        private static Dictionary<string, LevelElement> LoadBrushFormFolder(string path)
        {
            var dictionary = new Dictionary<string, LevelElement>(1);
            
            var bundle = LoadAssetBundle(path: Path.Combine(path, "resources.bundle"));
            Logger.LogDebug($"resources.bundle -> {bundle} -> {bundle.GetAllAssetNames().Join()}");

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
                case var tag when tag.HasFlag(Tag.Decor):
                    var decor = CustomAssetUtility
                        .DeserializeAssetFromPath<DecorAsset>(source: Path.Combine(path, "asset.json"));
                    Logger.LogDebug($"asset.json -> {decor}");
                    break;
            }

            var element = CustomAssetUtility
                .DeserializeAssetFromPath<LevelElement>(source: Path.Combine(path, "element.json"));
            Logger.LogDebug($"element.json -> {element} -> {element.Title}");

            var id = AssetElementBinder.PushToIndex(element);
            dictionary.Add(id, element);

            return dictionary;
        }

        private static Dictionary<string, LevelElement> LoadDecorFormFolder(string path)
        {
            var dictionary = new Dictionary<string, LevelElement>(1);
            
            var bundle = LoadAssetBundle(path: Path.Combine(path, "resources.bundle"));
            Logger.LogDebug($"resources.bundle -> {bundle} -> {bundle.GetAllAssetNames().Join()}");

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
                    dictionary.Add(id, impl);
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
                dictionary.Add(id, element);
            }

            return dictionary;
        }

        private static AssetBundle LoadAssetBundle(string path)
        {
            AssetBundle bundle;
            {
                var request = AssetBundle.LoadFromFileAsync(path);
                while (!request.isDone) Thread.Sleep(100);

                bundle = request.assetBundle;
            }

            foreach (var name in bundle.GetAllAssetNames())
            {
                Logger.LogDebug($"[{bundle.name}] load path: {name}");
                var request = bundle.LoadAssetAsync(name);
                while (!request.isDone) Thread.Sleep(100);

                if (request.asset == null)
                {
                    Logger.LogWarning("null !!!");
                    continue;
                }

                request.asset.hideFlags = HideFlags.HideAndDontSave;

                switch (request.asset)
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
                    case TextAsset asset:
                        if (name == "bank.strings" || name == "bank_")
                        {
                            try
                            {
                                FMODUnity.RuntimeManager.LoadBank(asset: asset, loadSamples: true);
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

            return bundle;
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

        public static void LoadBanks(string folder, bool loadSamples = false)
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
                    FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: loadSamples);
                }
                catch (FMODUnity.BankLoadException e)
                {
                    Logger.LogWarning(e);
                    continue;
                }
                var path = $"bank:/{bank}";
                Logger.LogInfo($"load {path}");
                foreach (var (_, asset) in AssetElementBinder.FetchFMODAsset(path: path))
                {
                    Logger.LogDebug($"[{bank}] {asset.path}");
                }
            }
        }
    }
}