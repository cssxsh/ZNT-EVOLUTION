using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [JsonObject]
    [UsedImplicitly]
    internal class SpriteMerge : EvolutionInfo
    {
        [JsonProperty("Source")] public readonly tk2dSpriteCollectionData Source;

        [JsonProperty("Name")] public readonly string Name;

        [JsonProperty("AttachPoints")] public readonly Dictionary<int, tk2dSpriteDefinition.AttachPoint[]> AttachPoints;

        [JsonConstructor]
        public SpriteMerge(
            tk2dSpriteCollectionData source,
            string name = null,
            Dictionary<int, tk2dSpriteDefinition.AttachPoint[]> points = null)
        {
            Source = source;
            Name = name;
            AttachPoints = points ?? new Dictionary<int, tk2dSpriteDefinition.AttachPoint[]>();
        }
    }
}