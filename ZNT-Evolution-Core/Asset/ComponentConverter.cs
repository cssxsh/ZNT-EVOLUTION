using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class ComponentConverter : CustomCreationConverter<Component>
    {
        private readonly Type[] _include;

        public ComponentConverter(params Type[] include) => _include = include;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, Wrap(value));
        }

        public override bool CanWrite => true;

        public override Component Create(Type objectType)
        {
            return new GameObject(objectType.Name) { hideFlags = HideFlags.HideAndDontSave }
                .AddComponent(objectType);
        }

        public override bool CanConvert(Type objectType) => _include.Contains(objectType);

        private static object Wrap(object value) => value switch
        {
            tk2dSpriteAnimation animation => new AnimationWrapper(animation),
            tk2dSpriteCollectionData sprites => new SpritesWrapper(sprites),
            _ => throw new NotSupportedException("wrap " + value.GetType())
        };

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