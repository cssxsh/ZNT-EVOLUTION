using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    [JsonObject]
    [UsedImplicitly]
    internal class SpriteInfo : EvolutionInfo
    {
        [JsonProperty("OrthoSize")] public readonly float OrthoSize;

        [JsonProperty("TargetHeight")] public readonly float TargetHeight;

        [JsonProperty("Names")] public readonly string[] Names;

        [JsonProperty("Regions")] public readonly Rect[] Regions;

        [JsonProperty("Anchors")] public readonly Vector2[] Anchors;

        [JsonProperty("AttachPoints")] public readonly Dictionary<int, tk2dSpriteDefinition.AttachPoint[]> AttachPoints;

        [JsonProperty("Name")] public readonly string Name;

        [JsonConstructor]
        public SpriteInfo(
            float orthoSize,
            float targetHeight,
            string[] names,
            Rect[] regions,
            Vector2[] anchors = null, Vector2? anchor = null,
            Dictionary<int, tk2dSpriteDefinition.AttachPoint[]> points = null,
            string name = null)
        {
            OrthoSize = orthoSize;
            TargetHeight = targetHeight;
            Names = names;
            Regions = regions;
            Anchors = anchors ?? Regions.Select(_ => anchor ?? Vector2.zero).ToArray();
            AttachPoints = points ?? new Dictionary<int, tk2dSpriteDefinition.AttachPoint[]>();
            Name = name;
        }
    }
}