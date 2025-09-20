using System;
using HarmonyLib;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class LevelElementAddition : EvolutionInfo
    {
        [JsonProperty("Targets")] public readonly LevelElement[] Targets;

        [JsonProperty("CustomAsset")] public readonly CustomAssetObject CustomAsset;

        [JsonConstructor]
        public LevelElementAddition(LevelElement[] targets, CustomAssetObject asset)
        {
            Targets = targets;
            CustomAsset = asset;
        }

        public void Apply()
        {
            foreach (var element in Targets)
            {
                switch (element.CustomAsset)
                {
                    case HumanAsset human:
                        Apply(human);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported asset type {element}");
                }
            }
        }

        private void Apply(HumanAsset human)
        {
            switch (CustomAsset)
            {
                case PhysicObjectAsset physic:
                    human.ThrowableObjects = human.ThrowableObjects.AddToArray(physic);
                    break;
                case ExplosionAsset explosion:
                    human.ExplosionAssets = human.ExplosionAssets.AddToArray(explosion);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported asset type {CustomAsset}");
            }
        }
    }
}