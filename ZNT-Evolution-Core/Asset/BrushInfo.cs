using System;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class BrushInfo : EvolutionInfo
    {
        [JsonProperty("Name")] public readonly string Name;

        [JsonProperty("ForceLegacySideways")] public readonly bool ForceLegacySideways;

        [JsonProperty("ApplyPrefabTransform")] public readonly bool ApplyPrefabTransform;

        [JsonProperty("UserFlags")] public readonly int UserFlags;

        [JsonProperty("ForceOverrideFlags")] public readonly bool ForceOverrideFlags;

        [JsonProperty("Coalesce")] public readonly Rotorz.Tile.Coalesce Coalesce;

        [JsonProperty("Variation")] public readonly UnityEngine.Object Variation;

        [JsonConstructor]
        public BrushInfo(
            string name, 
            bool forceLegacySideways, 
            bool applyPrefabTransform, 
            int userFlags,
            bool forceOverrideFlags,
            Rotorz.Tile.Coalesce coalesce,
            UnityEngine.Object variation)
        {
            Name = name;
            ForceLegacySideways = forceLegacySideways;
            ApplyPrefabTransform = applyPrefabTransform;
            UserFlags = userFlags;
            ForceOverrideFlags = forceOverrideFlags;
            Coalesce = coalesce;
            Variation = variation;
        }
    }
}