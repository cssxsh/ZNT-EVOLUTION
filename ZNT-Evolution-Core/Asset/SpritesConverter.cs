using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class SpritesConverter : CustomCreationConverter<tk2dSpriteCollectionData>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new Wrapper((tk2dSpriteCollectionData)value));
        }

        public override bool CanWrite => true;

        public override tk2dSpriteCollectionData Create(Type objectType)
        {
            var impl = new GameObject("SpriteCollection")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            
            return impl.AddComponent<tk2dSpriteCollectionData>();
        }
        
        
        [Serializable]
        internal class Wrapper
        {
            [JsonIgnore] private tk2dSpriteCollectionData _data;

            public Wrapper(tk2dSpriteCollectionData data) => _data = data;
            
            [JsonProperty("name")] public string Name => _data.name;
            
            [JsonProperty("version")] public int Version => _data.version;
            
            [JsonProperty("needMaterialInstance")] public bool NeedMaterialInstance => _data.needMaterialInstance;

            [JsonProperty("spriteDefinitions")] public tk2dSpriteDefinition[] Definitions => _data.spriteDefinitions;
            
            [JsonProperty("premultipliedAlpha")] public bool PremultipliedAlpha => _data.premultipliedAlpha;
            
            [JsonProperty("material")] public Material Material => _data.material;
            
            [JsonProperty("materials")] public Material[] Materials => _data.materials;
            
            [JsonProperty("textures")] public Texture[] Textures => _data.textures;
            
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
        }
    }
}