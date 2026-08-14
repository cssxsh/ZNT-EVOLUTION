using System;
using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class AssetAddition : EvolutionAddition<CustomAsset, UnityEngine.Object>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(AssetAddition));

    [JsonProperty("Targets")]
    private List<CustomAsset> Targets = [];

    [JsonProperty("Assets")]
    private List<UnityEngine.Object> Assets = [];

    public override void Push(CustomAsset target, UnityEngine.Object source)
    {
        Targets.Add(target);
        Assets.Add(source);
    }

    public override void Apply()
    {
        foreach (var (target, source) in this)
        {
            switch (target, source)
            {
                case (MovingObjectAsset moving, tk2dSpriteAnimation animation):
                    Traverse.Create(moving).Field<tk2dSpriteAnimation>("library").Value = animation;
                    break;
                case (PhysicObjectAsset physic, tk2dSpriteAnimation animation):
                    Traverse.Create(physic).Field<tk2dSpriteAnimation>("library").Value = animation;
                    break;
                case (PhysicObjectAsset physic, ExplosionAsset explosion):
                    physic.Explosion = explosion;
                    break;
                case (SentryGunAsset sentry, tk2dSpriteCollectionData sprites):
                    sentry.SpriteCollection = sprites;
                    break;
                case (SentryGunAsset sentry, tk2dSpriteAnimation animation):
                    sentry.Animation = animation;
                    break;
                case (SentryGunAsset sentry, PhysicObjectAsset physic):
                    sentry.ThrowableObjects = sentry.ThrowableObjects.AddToArray(physic);
                    break;
                case (HumanAsset human, tk2dSpriteCollectionData sprites):
                    human.SpriteCollection = sprites;
                    break;
                case (HumanAsset human, tk2dSpriteAnimation animation):
                    human.AnimationLibrary = animation;
                    break;
                case (HumanAsset human, PhysicObjectAsset physic):
                    human.ThrowableObjects = human.ThrowableObjects.AddToArray(physic);
                    break;
                case (HumanAsset human, ExplosionAsset explosion):
                    human.ExplosionAssets = human.ExplosionAssets.AddToArray(explosion);
                    break;
                case (HumanAsset human, CharacterAnimationAsset animations):
                    human.Animations = animations;
                    break;
                case (HumanAsset human, CharacterAsset rise):
                    human.RiseAsset = rise;
                    break;
                case (LevelElement element, CustomAssetObject asset):
                    element.CustomAsset = asset;
                    break;
                case (LevelElement element, tk2dSpriteCollectionData sprites):
                    element.SpriteCollection = sprites;
                    break;
                case (LevelElement element, tk2dSpriteAnimation animation):
                    element.AnimationLibrary = animation;
                    break;
                case (LevelElement element, UnityEngine.Sprite preview):
                    element.Preview = preview;
                    break;
                default:
                    throw new NotSupportedException($"Unsupported asset type {source} for {target}");
            }
        }
    }

    public override void Clear()
    {
        // TODO Reset AssetAddition
    }

    public override IEnumerator<KeyValuePair<CustomAsset, UnityEngine.Object>> GetEnumerator()
    {
        var length = Count;
        for (var i = 0; i < length; i++)
        {
            var element = Targets[i];
            var asset = Assets[i];
            if (element is null || asset is null) continue;
            yield return new KeyValuePair<CustomAsset, UnityEngine.Object>(element, asset);
        }
    }

    public override int Count => Math.Min(Targets.Count, Assets.Count);

    public override void OnAfterDeserialize()
    {
        Targets ??= [];
        Assets ??= [];
        if (Targets.Count != Assets.Count) Logger.LogWarning("Targets.Count != Assets.Count");
    }
}