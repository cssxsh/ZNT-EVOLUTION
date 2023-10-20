using System;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class TagInfo : EvolutionInfo
    {
        [JsonProperty("Tag")] public readonly Tag Tag;

        [JsonConstructor]
        public TagInfo(Tag tag) => Tag = tag;
    }
}