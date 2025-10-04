using System;
using BepInEx.Logging;
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
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("LevelElementAddition");

        [JsonProperty("Assets")] public readonly CustomAsset[] Assets;

        [JsonConstructor]
        public LevelElementAddition(LevelElement[] targets, CustomAsset[] assets) : base(targets)
        {
            if (targets.Length != assets.Length) Logger.LogWarning("Targets.Length != Assets.Length");
            Assets = assets;
        }

        public override void Apply()
        {
            var length = Math.Min(Targets.Length, Assets.Length);
            for (var i = 0; i < length; i++)
            {
                var element = Targets[i];
                var asset = Assets[i];
                switch (element.CustomAsset)
                {
                    case null:
                        element.CustomAsset = asset as CustomAssetObject;
                        break;
                    case HumanAsset human when asset is PhysicObjectAsset physic:
                        human.ThrowableObjects = human.ThrowableObjects.AddToArray(physic);
                        break;
                    case HumanAsset human when asset is ExplosionAsset explosion:
                        human.ExplosionAssets = human.ExplosionAssets.AddToArray(explosion);
                        break;
                    case HumanAsset human when asset is CharacterAnimationAsset animations:
                        human.Animations = animations;
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