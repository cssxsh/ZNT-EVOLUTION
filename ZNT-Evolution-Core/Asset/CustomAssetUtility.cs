using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public static class CustomAssetUtility
    {
        private static readonly JsonSerializerSettings AssetSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new NameConverter(typeof(tk2dSpriteAnimation)),
                new NameConverter(typeof(tk2dSpriteCollectionData)),
                new NameConverter(typeof(FMODAsset)),
                new NameConverter(typeof(Transform))
            }
        };

        private static readonly JsonSerializerSettings AnimationSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new AnimationConverter(),
                new NameConverter(typeof(tk2dSpriteCollectionData)),
                new NameConverter(typeof(FMODAsset)),
                new NameConverter(typeof(Transform))
            }
        };

        public static void SerializeAssetToPath(string target, CustomAsset asset)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(AssetSettings);
            SaveObjectToPath(jsonSerializer, asset, target);
        }

        public static void SerializeAnimationToPath(string target, tk2dSpriteAnimation asset)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(AnimationSettings);
            SaveObjectToPath(jsonSerializer, asset, target);
        }

        private static void SaveObjectToPath(JsonSerializer jsonSerializer, object data, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var writer = new StreamWriter(stream))
                {
                    using (var json = new JsonTextWriter(writer))
                    {
                        json.Formatting = Formatting.Indented;
                        jsonSerializer.Serialize(json, data);
                    }
                }
            }
        }

        public static T DeserializeAssetFromPath<T>(string source) where T : CustomAsset
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(AssetSettings);
            return LoadObjectToPath<T>(jsonSerializer, source);
        }

        public static tk2dSpriteAnimation DeserializeAnimationFromPath(string source)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(AnimationSettings);
            return LoadObjectToPath<tk2dSpriteAnimation>(jsonSerializer, source);
        }

        private static T LoadObjectToPath<T>(JsonSerializer jsonSerializer, string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var writer = new StreamReader(stream))
                {
                    using (var json = new JsonTextReader(writer))
                    {
                        return jsonSerializer.Deserialize<T>(json);
                    }
                }
            }
        }

        // public static tk2dSpriteCollectionData CreateSprite()
        // {
        //     var texture = new Texture2D(0, 0);
        //     texture.LoadImage(File.ReadAllBytes(""));
        //     tk2dSpriteCollectionData.CreateFromTexture(texture, )
        //     return null;
        // }
    }
}