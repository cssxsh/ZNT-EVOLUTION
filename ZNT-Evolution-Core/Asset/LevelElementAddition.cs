using System;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Asset
{
    [JsonObject]
    [UsedImplicitly]
    internal class LevelElementAddition : EvolutionAddition<LevelElement>
    {
        [JsonProperty("Assets")] public readonly CustomAssetObject[] Assets;

        [JsonConstructor]
        public LevelElementAddition(LevelElement[] targets, CustomAssetObject[] assets) : base(targets)
        {
            if (targets.Length != assets.Length) throw new FormatException("Targets.Length != Assets.Length");
            Assets = assets;
        }

        public override void Apply()
        {
            for (var i = 0; i < Assets.Length; i++)
            {
                var element = Targets[i];
                var asset = Assets[i];
                switch (element.CustomAsset)
                {
                    case null:
                        element.CustomAsset = asset;
                        break;
                    case HumanAsset human when asset is PhysicObjectAsset physic:
                        human.ThrowableObjects = human.ThrowableObjects.AddToArray(physic);
                        break;
                    case HumanAsset human when asset is ExplosionAsset explosion:
                        human.ExplosionAssets = human.ExplosionAssets.AddToArray(explosion);
                        break;
                    case SentryGunAsset sentry when asset is PhysicObjectAsset physic:
                        sentry.ThrowableObjects = sentry.ThrowableObjects.AddToArray(physic);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported asset type {asset} for {element}");
                }
            }
        }
    }
}