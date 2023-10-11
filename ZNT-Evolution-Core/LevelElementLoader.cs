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

        public static LevelElement LoadFormFolder(string path)
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
            var bundle = LoadFormAssetBundle(path: Path.Combine(path, "resources.bundle"));
            Logger.LogDebug($"resources.bundle -> {bundle} -> {bundle.GetAllAssetNames().Join()}");

            var sprites = CustomAssetUtility
                .LoadComponentFromPath<tk2dSpriteCollectionData>(source: Path.Combine(path, "sprites.json"));
            Logger.LogDebug($"sprites.json -> {sprites}");

            var animation = CustomAssetUtility
                .LoadComponentFromPath<tk2dSpriteAnimation>(source: Path.Combine(path, "animation.json"));
            Logger.LogDebug($"animation.name -> {animation}");

            var asset = CustomAssetUtility
                .DeserializeAssetFromPath<HumanAsset>(source: Path.Combine(path, "asset.json"));
            Logger.LogDebug($"asset.json -> {asset}");

            var element = CustomAssetUtility
                .DeserializeAssetFromPath<LevelElement>(source: Path.Combine(path, "element.json"));
            Logger.LogDebug($"element.json -> {element}");
            AssetElementBinder.PushToIndex(element);

            return element;
        }

        private static AssetBundle LoadFormAssetBundle(string path)
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
                Logger.LogDebug($"[AssetBundle] load path: {name}");
                var request = bundle.LoadAssetAsync(name);
                while (!request.isDone)
                {
                    Thread.Sleep(100);
                }

                if (request.asset == null) continue;

                request.asset.hideFlags = HideFlags.HideAndDontSave;

                switch (request.asset)
                {
                    case null:
                        Logger.LogWarning("null !!!");
                        break;
                    case Sprite sprite:
                        Logger.LogDebug(sprite.name);
                        Logger.LogDebug(sprite.texture);
                        break;
                    case Material material:
                        Logger.LogDebug(material.name);
                        Logger.LogDebug(material.mainTexture);
                        Logger.LogDebug(material.shader.name);
                        break;
                    case Shader shader:
                        Logger.LogDebug(shader.name);
                        break;
                    case Rotorz.Tile.OrientedBrush brush:
                        Logger.LogDebug(brush.name);
                        break;
                    default:
                        Logger.LogDebug(request.asset.name);
                        break;
                }
            }

            return bundle;
        }
    }
}