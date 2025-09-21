using System;
using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class UnityEngineObjectConverter : CustomCreationConverter<UnityEngine.Object>
    {
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer.WriteState == WriteState.Start)
            {
                value = value switch
                {
                    MovingObjectAsset moving => moving.Wrap(),
                    PhysicObjectAsset physic => physic.Wrap(),
                    TriggerAsset trigger => trigger.Wrap(),
                    ExplosionAsset explosion => explosion.Wrap(),
                    tk2dSpriteAnimation animation => new AnimationWrapper(animation),
                    tk2dSpriteCollectionData sprites => new SpritesWrapper(sprites),
                    _ => value
                };
                Traverse.Create(serializer)
                    .Field("_serializerWriter")
                    .Method("SerializeObject", new[]
                    {
                        typeof(JsonWriter),
                        typeof(object),
                        typeof(JsonObjectContract),
                        typeof(JsonProperty),
                        typeof(JsonContract)
                    })
                    .GetValue(writer, value, serializer.ContractResolver.ResolveContract(value.GetType()), null, null);
                return;
            }

            switch (value)
            {
                case FMODAsset fmod:
                    writer.WriteValue(fmod.path);
                    break;
                case Shader shader:
                    writer.WriteValue(shader.name);
                    break;
                case UnityEngine.Object impl:
                    writer.WriteValue($"{impl.name} : {value.GetType()}");
                    break;
                default:
                    throw new NotSupportedException($"write ${value.GetType()}");
            }
        }

        public override bool CanRead => true;

        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public override UnityEngine.Object Create(Type objectType)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(objectType))
            {
                if (objectType == typeof(MovingObjectAsset)) objectType = typeof(MovingObjectAssetWrapper);
                else if (objectType == typeof(PhysicObjectAsset)) objectType = typeof(PhysicObjectAssetWrapper);
                else if (objectType == typeof(TriggerAsset)) objectType = typeof(TriggerAssetWrapper);
                else if (objectType == typeof(ExplosionAsset)) objectType = typeof(ExplosionAssetWrapper);
                return ScriptableObject.CreateInstance(objectType);
            }

            if (typeof(Component).IsAssignableFrom(objectType))
            {
                return new GameObject(objectType.Name) { hideFlags = HideFlags.HideAndDontSave }
                    .AddComponent(objectType);
            }

            return objectType.GetConstructor(Array.Empty<Type>())?.Invoke(null) as UnityEngine.Object;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object _, JsonSerializer serializer)
        {
            if (reader.Depth == 0) return base.ReadJson(reader, objectType, null, serializer);
            var key = serializer.Deserialize<string>(reader);
            if (key == null) return null;
            if (objectType == typeof(Shader)) return Shader.Find(key);
            if (objectType == typeof(FMODAsset)) return FmodAssetIndex.PathIndex[key];

            var name = key.Split(':')[0].Trim();
            _cache.TryGetValue(name, out var impl);

            if (objectType.IsInstanceOfType(impl)) return impl;

            if (objectType == typeof(GameObject) && GameObject.Find(name) is { } body)
            {
                _cache[name] = body;
                return body;
            }

            foreach (var asset in Resources.FindObjectsOfTypeAll(objectType))
            {
                if (asset.name != name) continue;
                _cache[name] = asset;
                return asset;
            }

            throw new KeyNotFoundException(message: $"{objectType.FullName}(name: {name})");
        }

        [Serializable]
        internal class AnimationWrapper
        {
            [JsonIgnore] private readonly tk2dSpriteAnimation _data;

            public AnimationWrapper(tk2dSpriteAnimation data) => _data = data;

            [JsonProperty("name")] public string Name => _data.name;

            [JsonProperty("clips")] public tk2dSpriteAnimationClip[] Clips => _data.clips;
        }

        [Serializable]
        internal class SpritesWrapper
        {
            [JsonIgnore] private readonly tk2dSpriteCollectionData _data;

            public SpritesWrapper(tk2dSpriteCollectionData data) => _data = data;

            [JsonProperty("name")] public string Name => _data.name;

            [JsonProperty("version")] public int Version => _data.version;

            [JsonProperty("material")] public Material Material => _data.material;

            [JsonProperty("materials")] public Material[] Materials => _data.materials;

            [JsonProperty("textures")] public Texture[] Textures => _data.textures;

            [JsonProperty("needMaterialInstance")] public bool NeedMaterialInstance => _data.needMaterialInstance;

            [JsonProperty("premultipliedAlpha")] public bool PremultipliedAlpha => _data.premultipliedAlpha;

            [JsonProperty("pngTextures")] public TextAsset[] PngTextures => _data.pngTextures;

            [JsonProperty("textureFilterMode")] public FilterMode TextureFilterMode => _data.textureFilterMode;

            [JsonProperty("textureMipMaps")] public bool TextureMipMaps => _data.textureMipMaps;

            [JsonProperty("allowMultipleAtlases")] public bool AllowMultipleAtlases => _data.allowMultipleAtlases;

            [JsonProperty("spriteCollectionGUID")] public string SpriteCollectionGuid => _data.spriteCollectionGUID;

            [JsonProperty("spriteCollectionName")] public string SpriteCollectionName => _data.spriteCollectionName;

            [JsonProperty("assetName")] public string AssetName => _data.assetName;

            [JsonProperty("loadable")] public bool Loadable => _data.loadable;

            [JsonProperty("invOrthoSize")] public float InvOrthoSize => _data.invOrthoSize;

            [JsonProperty("halfTargetHeight")] public float HalfTargetHeight => _data.halfTargetHeight;

            [JsonProperty("spriteDefinitions")] public tk2dSpriteDefinition[] Definitions => _data.spriteDefinitions;
        }
    }
}