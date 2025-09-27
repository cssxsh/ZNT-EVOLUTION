using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    [JsonObject]
    [UsedImplicitly]
    internal class ComponentMerge : EvolutionInfo
    {
        [JsonProperty("Source")] public readonly Component Source;

        [JsonProperty("Name")] public readonly string Name;

        [JsonProperty("Fields")] public readonly Dictionary<string, string> Fields;

        [JsonConstructor]
        public ComponentMerge(Component source, string name, Dictionary<string, string> fields)
        {
            Source = source;
            Name = name;
            Fields = fields;
        }
    }
}