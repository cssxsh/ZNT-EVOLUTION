using System;
using HarmonyLib;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class MovingObjectAssetWrapper : MovingObjectAsset
    {
        public tk2dSpriteAnimation Animation
        {
            get => Traverse.Create(this).Field<tk2dSpriteAnimation>("library").Value;
            set => Traverse.Create(this).Field<tk2dSpriteAnimation>("library").Value = value;
        }
    }
}