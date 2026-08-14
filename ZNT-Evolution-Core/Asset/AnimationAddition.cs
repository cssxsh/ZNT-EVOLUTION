using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class AnimationAddition : EvolutionAddition<tk2dSpriteAnimation, tk2dSpriteAnimationClip>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(AnimationAddition));

    [JsonProperty("Targets")]
    private List<tk2dSpriteAnimation> Targets = [];

    [JsonProperty("Clips")]
    private List<tk2dSpriteAnimationClip> Clips = [];

    public override void Push(tk2dSpriteAnimation target, tk2dSpriteAnimationClip source)
    {
        Targets.Add(target);
        Clips.Add(source);
    }

    public override void Apply()
    {
        var changed = new C5.HashSet<tk2dSpriteAnimation>();
        foreach (var (animation, clip) in this)
        {
            if (animation.clips.Contains(clip)) continue;
            var id = animation.GetClipIdByName(clip.name);
            if (id != -1) Logger.LogWarning($"{animation.name} already exists clip {clip.name} at {id}");
            animation.clips = animation.clips.AddToArray(clip);
            changed.Add(animation);
        }

        foreach (var animation in changed)
        {
            Traverse.Create(animation)
                .Field<Dictionary<string, tk2dSpriteAnimationClip>>("clipNameCache").Value = null;
            Traverse.Create(animation)
                .Field<Dictionary<string, int>>("idNameCache").Value = null;
            animation.InitializeClipCache();
        }
    }

    protected override void OnDestroy()
    {
        var changed = new C5.HashSet<tk2dSpriteAnimation>();
        foreach (var (animation, clip) in this)
        {
            if (!animation.clips.Contains(clip)) continue;
            animation.clips = animation.clips.Where(item => item != clip).ToArray();
            changed.Add(animation);
        }

        foreach (var animation in changed)
        {
            Traverse.Create(animation)
                .Field<Dictionary<string, tk2dSpriteAnimationClip>>("clipNameCache").Value = null;
            Traverse.Create(animation)
                .Field<Dictionary<string, int>>("idNameCache").Value = null;
            animation.InitializeClipCache();
        }
    }

    public override IEnumerator<KeyValuePair<tk2dSpriteAnimation, tk2dSpriteAnimationClip>> GetEnumerator()
    {
        var length = Count;
        for (var i = 0; i < length; i++)
        {
            var animation = Targets[i];
            var clip = Clips[i];
            if (animation is null || clip is null) continue;
            yield return new KeyValuePair<tk2dSpriteAnimation, tk2dSpriteAnimationClip>(animation, clip);
        }
    }

    public override int Count => Math.Min(Targets.Count, Clips.Count);

    public override void OnAfterDeserialize()
    {
        Targets ??= [];
        Clips ??= [];
        if (Targets.Count != Clips.Count) Logger.LogWarning("Targets.Count != Clips.Count");
    }
}