using System;
using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Asset
{
    [JsonObject]
    [UsedImplicitly]
    internal class AnimationAddition : EvolutionAddition<tk2dSpriteAnimation>
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("AnimationAddition");

        [JsonProperty("Clips")] public readonly tk2dSpriteAnimationClip[] Clips;

        [JsonConstructor]
        public AnimationAddition(tk2dSpriteAnimation[] targets, tk2dSpriteAnimationClip[] clips) : base(targets)
        {
            if (targets.Length != clips.Length) Logger.LogWarning("Targets.Length != Clips.Length");
            Clips = clips;
        }

        public override void Apply()
        {
            var length = Math.Min(Targets.Length, Clips.Length);
            for (var i = 0; i < length; i++)
            {
                var animation = Targets[i];
                var clip = Clips[i];
                animation.clips = animation.clips.AddToArray(clip);
                Traverse.Create(animation)
                    .Field<Dictionary<string, tk2dSpriteAnimationClip>>("clipNameCache").Value = null;
                Traverse.Create(animation)
                    .Field<Dictionary<string, int>>("idNameCache").Value = null;
                animation.InitializeClipCache();
            }
        }
    }
}