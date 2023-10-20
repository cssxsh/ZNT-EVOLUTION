using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class SpriteInfo : EvolutionInfo
    {
        [JsonProperty("OrthoSize")] public readonly float OrthoSize;

        [JsonProperty("TargetHeight")] public readonly float TargetHeight;

        [JsonProperty("Names")] public readonly string[] Names;

        [JsonProperty("Regions")] public readonly Rect[] Regions;

        [JsonProperty("Anchors")] public readonly Vector2[] Anchors;

        [JsonConstructor]
        public SpriteInfo(
            float orthoSize, float targetHeight,
            string[] names, Rect[] regions, Vector2[] anchors = null,
            Vector2? anchor = null)
        {
            OrthoSize = orthoSize;
            TargetHeight = targetHeight;
            Names = names;
            Regions = regions;
            Anchors = anchors ?? Regions.Select(_ => anchor ?? Vector2.zero).ToArray();
        }
    }
}