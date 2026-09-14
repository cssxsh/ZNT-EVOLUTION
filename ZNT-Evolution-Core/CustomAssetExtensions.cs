using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace ZNT.Evolution.Core;

public static class CustomAssetExtensions
{
    extension(CustomAsset asset)
    {
        [UsedImplicitly]
        public void SetAssetId(string value) => Traverse.Create(asset).Field<string>("assetId").Value = value;
    }

    extension(ExplosionAsset explosion)
    {
        [UsedImplicitly]
        public bool AutoExplode
        {
            get => Traverse.Create(explosion).Field<bool>("autoExplode").Value;
            set => Traverse.Create(explosion).Field<bool>("autoExplode").Value = value;
        }
    }

    extension(MovingObjectAsset moving)
    {
        [UsedImplicitly]
        public tk2dSpriteAnimation Animation
        {
            get => Traverse.Create(moving).Field<tk2dSpriteAnimation>("library").Value;
            set => Traverse.Create(moving).Field<tk2dSpriteAnimation>("library").Value = value;
        }
    }

    extension(PhysicObjectAsset physic)
    {
        [UsedImplicitly]
        public tk2dSpriteAnimation Animation
        {
            get => Traverse.Create(physic).Field<tk2dSpriteAnimation>("library").Value;
            set => Traverse.Create(physic).Field<tk2dSpriteAnimation>("library").Value = value;
        }
    }

    extension(TMPro.TMP_Settings)
    {
        [UsedImplicitly]
        public static void SetDefaultSpriteAsset(TMPro.TMP_SpriteAsset value)
        {
            Traverse.Create(TMPro.TMP_Settings.instance)
                .Field<TMPro.TMP_SpriteAsset>("m_defaultSpriteAsset").Value = value;
        }
    }

    extension(TMPro.MaterialReferenceManager)
    {
        [UsedImplicitly]
        public static void RemoveFontAsset(TMPro.TMP_FontAsset fontAsset)
        {
            Traverse.Create(TMPro.MaterialReferenceManager.instance)
                .Field<Dictionary<int, TMPro.TMP_FontAsset>>("m_FontAssetReferenceLookup").Value
                .Remove(fontAsset.hashCode);
            Traverse.Create(TMPro.MaterialReferenceManager.instance)
                .Field<Dictionary<int, Material>>("m_FontMaterialReferenceLookup").Value
                .Remove(fontAsset.materialHashCode);
        }

        [UsedImplicitly]
        public static void RemoveSpriteAsset(TMPro.TMP_SpriteAsset spriteAsset)
        {
            Traverse.Create(TMPro.MaterialReferenceManager.instance)
                .Field<Dictionary<int, TMPro.TMP_SpriteAsset>>("m_SpriteAssetReferenceLookup").Value
                .Remove(spriteAsset.hashCode);
            Traverse.Create(TMPro.MaterialReferenceManager.instance)
                .Field<Dictionary<int, Material>>("m_FontMaterialReferenceLookup").Value
                .Remove(spriteAsset.hashCode);
        }
    }
}