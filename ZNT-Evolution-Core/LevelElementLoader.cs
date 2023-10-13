using System.IO;
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

        public static LevelElement LoadFormFolder(string path, LevelElement.Type type)
        {
            if (Directory.Exists(path))
            {
                Logger.LogInfo($"load form folder '{path}'.");
            }
            else
            {
                Logger.LogInfo($"folder {path} not exists.");
                return null;
            }

            switch (type)
            {
                case LevelElement.Type.Brush:
                    return LoadBrushFormFolder(path: path);
                case LevelElement.Type.Decor:
                    return LoadDecorFormFolder(path: path);
            }

            return null;
        }

        private static LevelElement LoadBrushFormFolder(string path)
        {
            var bundle = LoadAssetBundle(path: Path.Combine(path, "resources.bundle"));
            Logger.LogDebug($"resources.bundle -> {bundle} -> {bundle.GetAllAssetNames().Join()}");

            var sprites = CustomAssetUtility
                .LoadComponentFromPath<tk2dSpriteCollectionData>(source: Path.Combine(path, "sprites.json"));
            Logger.LogDebug($"sprites.json -> {sprites} -> ${sprites.materials[0]}");

            var animation = CustomAssetUtility
                .LoadComponentFromPath<tk2dSpriteAnimation>(source: Path.Combine(path, "animation.json"));
            Logger.LogDebug($"animation.name -> {animation}");

            var asset = CustomAssetUtility
                .DeserializeAssetFromPath<HumanAsset>(source: Path.Combine(path, "asset.json"));
            Logger.LogDebug($"asset.json -> {asset}");

            var element = CustomAssetUtility
                .DeserializeAssetFromPath<LevelElement>(source: Path.Combine(path, "element.json"));
            Logger.LogDebug($"element.json -> {element} -> ${element.Title}");
            AssetElementBinder.PushToIndex(element);

            return element;
        }

        private static LevelElement LoadDecorFormFolder(string path)
        {
            var bundle = LoadAssetBundle(path: Path.Combine(path, "resources.bundle"));
            Logger.LogDebug($"resources.bundle -> {bundle} -> {bundle.GetAllAssetNames().Join()}");

            var material = bundle.LoadAsset<Material>("sprites");
            if (File.Exists(Path.Combine(path, "sprite.info.json")))
            {
                var info = CustomAssetUtility
                    .DeserializeInfoFromPath<SpriteInfo>(source: Path.Combine(path, "sprite.info.json"));
                var sprites = CreateSprite(material, info);
                Logger.LogDebug($"CreateSprite -> {sprites} from {sprites.material}");
            }
            else
            {
                var sprites = CreateSingleSprite(material);
                Logger.LogDebug($"CreateSingleSprite -> {sprites} from {sprites.material}");
            }

            var element = CustomAssetUtility
                .DeserializeAssetFromPath<LevelElement>(source: Path.Combine(path, "element.json"));
            Logger.LogDebug($"element.json -> {element} to {element.Title}");
            AssetElementBinder.PushToIndex(element);

            return element;
        }

        private static AssetBundle LoadAssetBundle(string path)
        {
            AssetBundle bundle;
            {
                var request = AssetBundle.LoadFromFileAsync(path);
                while (!request.isDone)
                {
                    Thread.Sleep(100);
                }

                bundle = request.assetBundle;
            }

            foreach (var name in bundle.GetAllAssetNames())
            {
                Logger.LogDebug($"[{bundle.name}] load path: {name}");
                var request = bundle.LoadAssetAsync(name);
                while (!request.isDone)
                {
                    Thread.Sleep(100);
                }

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
            impl.gameObject.hideFlags = HideFlags.HideAndDontSave;

            return impl;
        }
        
        public static Dictionary<string, FMODAsset> LoadBank(string bank)
        {
            FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: true);
            return AssetElementBinder.FetchFMODAsset(path: $"bank:/{bank}");
        }
    }
}