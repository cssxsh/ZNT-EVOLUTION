using System.IO;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public static class CustomAssetUtility
    {
        private static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new UnityEngineObjectConverter(),
                new StringEnumConverter(),
                new LayerMaskConverter(),
                new ColorConverter(),
                new Vector2Converter(),
                new Vector3Converter(),
                new Vector4Converter(),
                new Matrix4x4Converter()
            }
        };

        public static void SerializeObjectToPath(string target, object data)
        {
            switch (data)
            {
                case LevelElement element:
                    element.SpriteDefinition = null;
                    break;
            }

            var serializer = JsonSerializer.Create(SerializerSettings);
            SaveObjectToPath(serializer, data, target);
        }

        private static void SaveObjectToPath(JsonSerializer serializer, object data, string path)
        {
            using var writer = new StreamWriter(path);
            using var json = new JsonTextWriter(writer);
            json.Formatting = Formatting.Indented;
            serializer.Serialize(json, data);
        }

        public static T DeserializeObjectFromPath<T>(string source)
        {
            var serializer = JsonSerializer.Create(SerializerSettings);
            var impl = LoadObjectFromPath<T>(serializer, source);
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

        public static T DeserializeObjectFromTextAsset<T>(TextAsset asset)
        {
            var serializer = JsonSerializer.Create(SerializerSettings);
            var impl = LoadObjectFromTextAsset<T>(serializer, asset);
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
            using var reader = new StreamReader(path);
            using var json = new JsonTextReader(reader);
            return jsonSerializer.Deserialize<T>(json);
        }

        private static T LoadObjectFromTextAsset<T>(JsonSerializer jsonSerializer, TextAsset asset)
        {
            using var reader = new StringReader(asset.text);
            using var json = new JsonTextReader(reader);
            return jsonSerializer.Deserialize<T>(json);
        }

        internal static MovingObjectAssetWrapper Wrap(this MovingObjectAsset moving)
        {
            if (moving is MovingObjectAssetWrapper wrapped) return wrapped;
            var wrap = ScriptableObject.CreateInstance<MovingObjectAssetWrapper>();
            Traverse.IterateFields(moving, wrap, Traverse.CopyFields);
            return wrap;
        }

        internal static PhysicObjectAssetWrapper Wrap(this PhysicObjectAsset physic)
        {
            if (physic is PhysicObjectAssetWrapper wrapped) return wrapped;
            var wrap = ScriptableObject.CreateInstance<PhysicObjectAssetWrapper>();
            Traverse.IterateFields(physic, wrap, Traverse.CopyFields);
            return wrap;
        }

        internal static TriggerAssetWrapper Wrap(this TriggerAsset trigger)
        {
            if (trigger is TriggerAssetWrapper wrapped) return wrapped;
            var wrap = ScriptableObject.CreateInstance<TriggerAssetWrapper>();
            Traverse.IterateFields(trigger, wrap, Traverse.CopyFields);
            return wrap;
        }

        internal static ExplosionAssetWrapper Wrap(this ExplosionAsset explosion)
        {
            if (explosion is ExplosionAssetWrapper wrapped) return wrapped;
            var wrap = ScriptableObject.CreateInstance<ExplosionAssetWrapper>();
            Traverse.IterateFields(explosion, wrap, Traverse.CopyFields);
            return wrap;
        }
    }
}