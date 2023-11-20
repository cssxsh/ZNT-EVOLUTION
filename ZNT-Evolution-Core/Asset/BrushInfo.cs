using System;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class BrushInfo : EvolutionInfo
    {
        [JsonProperty("Name")] public readonly string Name;

        [JsonProperty("Variation")] public readonly UnityEngine.Object Variation;

        [JsonConstructor]
        public BrushInfo(string name, UnityEngine.Object variation)
        {
            Name = name;
            Variation = variation;
        }
    }
}