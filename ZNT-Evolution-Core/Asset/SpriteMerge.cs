using System;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class SpriteMerge : EvolutionInfo
    {
        [JsonProperty("Source")] public readonly tk2dSpriteCollectionData Source;

        [JsonProperty("Name")] public readonly string Name;

        [JsonConstructor]
        public SpriteMerge(tk2dSpriteCollectionData source, string name = null)
        {
            Source = source;
            Name = name;
        }
    }
}