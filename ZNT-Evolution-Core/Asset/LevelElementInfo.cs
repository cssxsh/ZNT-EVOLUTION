using System;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    public class LevelElementInfo : EvolutionInfo
    {
        [JsonProperty("CustomAsset")] public readonly string CustomAsset;

        [JsonConstructor]
        public LevelElementInfo(string asset) => CustomAsset = asset;
    }
}