using System;
using System.Linq;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class SpriteMerge : EvolutionInfo
    {
        [JsonProperty("Source")] public readonly tk2dSpriteCollectionData Source;

        [JsonConstructor]
        public SpriteMerge(tk2dSpriteCollectionData source) => Source = source;
    }
}