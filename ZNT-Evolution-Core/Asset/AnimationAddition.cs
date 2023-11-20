using System;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class AnimationAddition : EvolutionInfo
    {
        [JsonProperty("Targets")] public readonly string[] Targets;

        [JsonProperty("Clips")] public readonly tk2dSpriteAnimationClip[] Clips;

        [JsonConstructor]
        public AnimationAddition(string[] targets, tk2dSpriteAnimationClip[] clips)
        {
            Targets = targets;
            Clips = clips;
        }
    }
}