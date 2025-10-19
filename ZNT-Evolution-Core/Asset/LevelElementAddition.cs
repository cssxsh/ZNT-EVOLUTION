using System;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class LevelElementAddition : EvolutionAddition<LevelElement>
{
    private static readonly ManualLogSource LogSource = Logger.CreateLogSource(nameof(LevelElementAddition));

    [JsonProperty("Assets")]
    public readonly CustomAsset[] Assets;

    [JsonConstructor]
    public LevelElementAddition(LevelElement[] targets, CustomAsset[] assets) : base(targets)
    {
        if (targets.Length != assets.Length) LogSource.LogWarning("Targets.Length != Assets.Length");
        Assets = assets;
    }

    public override void Apply()
    {
        var length = Math.Min(Targets.Length, Assets.Length);
        for (var i = 0; i < length; i++)
        {
            var element = Targets[i];
            if (element is null) continue;
            var asset = Assets[i];
            switch (element.CustomAsset, asset)
            {
                case (null, CustomAssetObject cao):
                    element.CustomAsset = cao;
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
                case (HumanAsset human, CustomAssetObject cao):
                    human.RiseAsset = cao;
                    break;
                case (SentryGunAsset sentry, PhysicObjectAsset physic):
                    sentry.ThrowableObjects = sentry.ThrowableObjects.AddToArray(physic);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported asset type {asset} for {element}");
            }
        }
    }
}