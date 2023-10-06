using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class SpriteAsset
    {
        [JsonProperty("texture")] public readonly string Texture;

        [JsonProperty("OrthoSize")] public readonly float OrthoSize;

        [JsonProperty("TargetHeight")] public readonly float TargetHeight;

        [JsonProperty("Definitions")] public readonly Dictionary<string, Definition> Definitions;

        [JsonConstructor]
        public SpriteAsset(string texture, float orthoSize, float targetHeight,
            Dictionary<string, Definition> definitions)
        {
            Texture = texture;
            OrthoSize = orthoSize;
            TargetHeight = targetHeight;
            Definitions = definitions;
        }

        [Serializable]
        internal class Definition
        {
            [JsonProperty("region")] public readonly Rect Region;

            [JsonProperty("anchor")] public readonly Vector2 Anchor;

            [JsonConstructor]
            public Definition(Rect region, Vector2 anchor)
            {
                Region = region;
                Anchor = anchor;
            }
        }
    }
}