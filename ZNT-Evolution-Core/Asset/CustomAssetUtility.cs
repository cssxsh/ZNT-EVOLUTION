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
                new StringEnumConverter(),
                new ColorConverter()
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
                new StringEnumConverter(),
                new ColorConverter()
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

        private static void SaveObjectToPath(JsonSerializer serializer, object data, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                using (var json = new JsonTextWriter(writer))
                {
                    json.Formatting = Formatting.Indented;
                    serializer.Serialize(json, data);
                }
            }
        }

        public static T DeserializeAssetFromPath<T>(string source) where T : CustomAsset
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(NameConverterSettings(typeof(T)));
            var impl = LoadObjectFromPath<T>(jsonSerializer, source);
            switch (impl)
            {
                case CharacterAnimationAsset animations:
                    animations.CommonAnimations.AnimationLibrary = animations.AnimationLibrary;
                    animations.HitAnimations.AnimationLibrary = animations.AnimationLibrary;
                    animations.DeathAnimations.AnimationLibrary = animations.AnimationLibrary;
                    (animations.CommonAnimations as ISerializationCallbackReceiver).OnAfterDeserialize();
                    (animations.HitAnimations as ISerializationCallbackReceiver).OnAfterDeserialize();
                    (animations.DeathAnimations as ISerializationCallbackReceiver).OnAfterDeserialize();
                    break;
            }

            return impl;
        }

        public static T DeserializeAssetFromTextAsset<T>(TextAsset asset) where T : CustomAsset
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(NameConverterSettings(typeof(T)));
            var impl = LoadObjectFromTextAsset<T>(jsonSerializer, asset);
            switch (impl)
            {
                case CharacterAnimationAsset animations:
                    animations.CommonAnimations.AnimationLibrary = animations.AnimationLibrary;
                    animations.HitAnimations.AnimationLibrary = animations.AnimationLibrary;
                    animations.DeathAnimations.AnimationLibrary = animations.AnimationLibrary;
                    (animations.CommonAnimations as ISerializationCallbackReceiver).OnAfterDeserialize();
                    (animations.HitAnimations as ISerializationCallbackReceiver).OnAfterDeserialize();
                    (animations.DeathAnimations as ISerializationCallbackReceiver).OnAfterDeserialize();
                    break;
            }

            return impl;
        }

        public static T LoadComponentFromPath<T>(string source) where T : Component
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(ComponentSettings(typeof(T)));
            var impl = LoadObjectFromPath<T>(jsonSerializer, source);
            switch (impl)
            {
                case tk2dSpriteAnimation animation:
                    animation.InitializeClipCache();
                    break;
                case tk2dSpriteCollectionData sprites:
                    sprites.InitDictionary();
                    sprites.InitMaterialIds();
                    break;
                case ISerializationCallbackReceiver receiver:
                    receiver.OnAfterDeserialize();
                    break;
            }

            return impl;
        }
        
        public static T LoadComponentFromTextAsset<T>(TextAsset asset) where T : Component
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(ComponentSettings(typeof(T)));
            var impl = LoadObjectFromTextAsset<T>(jsonSerializer, asset);
            switch (impl)
            {
                case tk2dSpriteAnimation animation:
                    animation.InitializeClipCache();
                    break;
                case tk2dSpriteCollectionData sprites:
                    sprites.InitDictionary();
                    sprites.InitMaterialIds();
                    break;
                case ISerializationCallbackReceiver receiver:
                    receiver.OnAfterDeserialize();
                    break;
            }

            return impl;
        }

        private static T LoadObjectFromPath<T>(JsonSerializer jsonSerializer, string path)
        {
            using (var reader = new StreamReader(path))
            {
                using (var json = new JsonTextReader(reader))
                {
                    return jsonSerializer.Deserialize<T>(json);
                }
            }
        }
        
        private static T LoadObjectFromTextAsset<T>(JsonSerializer jsonSerializer, TextAsset asset)
        {
            using (var reader = new StringReader(asset.text))
            {
                using (var json = new JsonTextReader(reader))
                {
                    return jsonSerializer.Deserialize<T>(json);
                }
            }
        }
    }
}