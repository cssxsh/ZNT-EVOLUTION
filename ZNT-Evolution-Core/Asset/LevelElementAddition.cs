using System;
using HarmonyLib;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class LevelElementAddition : EvolutionInfo
    {
        [JsonProperty("Targets")] public readonly LevelElement[] Targets;

        [JsonProperty("Assets")] public readonly CustomAssetObject[] Assets;

        [JsonConstructor]
        public LevelElementAddition(LevelElement[] targets, CustomAssetObject[] assets)
        {
            if (targets.Length != assets.Length) throw new FormatException("Targets.Length != Assets.Length");
            Targets = targets;
            Assets = assets;
        }

        public void Apply()
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