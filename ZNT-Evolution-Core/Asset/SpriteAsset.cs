using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class SpriteAsset
    {
        [JsonProperty("Texture")] private readonly Texture _texture;

        [JsonProperty("OrthoSize")] private readonly float _orthoSize;

        [JsonProperty("TargetHeight")] private readonly float _targetHeight;
        
        [JsonProperty("Color")] private readonly Color _color;
        
        [JsonProperty("GlobalIlluminationFlags")] private readonly MaterialGlobalIlluminationFlags _flags;

        [JsonProperty("Shader")] private readonly Shader _shader;

        [JsonProperty("ShaderKeywords")] private readonly string[] _shaderKeywords;

        [JsonProperty("Floats")] private readonly Dictionary<string, float> _floats;

        [JsonProperty("Names")] private readonly string[] _names;

        [JsonProperty("Regions")] private readonly Rect[] _regions;
        
        [JsonProperty("Anchors")] private readonly Vector2[] _anchors;

        [JsonConstructor]
        public SpriteAsset(Texture texture, 
            float orthoSize, float targetHeight, string[] names, Rect[] regions, Vector2[] anchors, 
            Color color, Shader shader, string[] shaderKeywords, MaterialGlobalIlluminationFlags flags, 
            Dictionary<string, float> floats)
        {
            _texture = texture;
            _orthoSize = orthoSize;
            _targetHeight = targetHeight;
            _names = names;
            _regions = regions;
            _anchors = anchors;
            
            _color = color;
            _shader = shader;
            _shaderKeywords = shaderKeywords;
            _flags = flags;
            _floats = floats;
        }

        public tk2dSpriteCollectionData CreateSprites(string name)
        {
            var impl = tk2dSpriteCollectionData.CreateFromTexture(
                texture: _texture,
                size: tk2dSpriteCollectionSize.Explicit(orthoSize: _orthoSize, targetHeight: _targetHeight),
                names: _names,
                regions: _regions,
                anchors: _anchors
            );
            impl.material.color = _color;
            impl.material.globalIlluminationFlags = _flags;
            impl.material.shader = _shader;
            impl.material.shaderKeywords = _shaderKeywords;
            
            foreach (var (property, value) in _floats)
            {
                if (!impl.material.HasProperty(property)) continue;
                impl.material.SetFloat(property, value);
            }
            
            impl.name = name;
            impl.assetName = name;
            impl.hideFlags = HideFlags.HideAndDontSave;
            
            return impl;
        }
    }
}