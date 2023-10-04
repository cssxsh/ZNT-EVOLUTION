using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public static class CustomAssetUtility
    {
        private static JsonSerializerSettings NameConverterSettings(params Type[] exclude) => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters = { new NameConverter(exclude: exclude) }
        };

        private static readonly JsonSerializerSettings AnimationSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters = { new AnimationConverter(), new NameConverter() }
        };

        public static void SerializeAssetToPath(string target, CustomAsset asset)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(NameConverterSettings(asset.GetType()));
            SaveObjectToPath(jsonSerializer, asset, target);
        }

        public static void SaveAnimationToPath(string target, tk2dSpriteAnimation asset)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(AnimationSettings);
            SaveObjectToPath(jsonSerializer, asset, target);
        }

        private static void SaveObjectToPath(JsonSerializer jsonSerializer, object data, string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
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
            JsonSerializer jsonSerializer = JsonSerializer.Create(NameConverterSettings(typeof(T)));
            var impl = LoadObjectToPath<T>(jsonSerializer, source);
            switch (impl)
            {
                case CharacterAnimationAsset animations:
                    animations.HitAnimations.AnimationLibrary = animations.AnimationLibrary;
                    animations.DeathAnimations.AnimationLibrary = animations.AnimationLibrary;
                    break;
            }
            return impl;
        }

        public static tk2dSpriteAnimation LoadAnimationFromPath(string source)
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