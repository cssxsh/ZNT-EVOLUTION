using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public static class CustomAssetUtility
    {
        private static JsonSerializerSettings NameConverterSettings(params Type[] exclude) => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new NameConverter(exclude: exclude),
                new ScriptableObjectConverter(),
                new StringEnumConverter()
            }
        };

        private static JsonSerializerSettings ComponentSettings(params Type[] include) => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new ComponentConverter(include: include),
                new NameConverter(exclude: include),
                new ScriptableObjectConverter(),
                new StringEnumConverter()
            }
        };

        public static void SerializeAssetToPath(string target, CustomAsset asset)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(NameConverterSettings(asset.GetType()));
            SaveObjectToPath(jsonSerializer, asset, target);
        }

        public static void SaveComponentToPath(string target, Component component)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(ComponentSettings(component.GetType()));
            SaveObjectToPath(jsonSerializer, component, target);
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

        public static T LoadComponentToPath<T>(string source) where T : Component
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(ComponentSettings(typeof(T)));
            var impl = LoadObjectToPath<T>(jsonSerializer, source);
            switch (impl)
            {
                case tk2dSpriteAnimation animation:
                    animation.InitializeClipCache();
                    break;
                case tk2dSpriteCollectionData sprites:
                    sprites.InitDictionary();
                    sprites.InitMaterialIds();
                    break;
            }

            return impl;
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
    }
}