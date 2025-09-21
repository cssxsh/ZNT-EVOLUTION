using System;
using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class TriggerAssetWrapper : TriggerAsset
    {
        public TrapType TrapMode
        {
            get => Traverse.Create(this).Field<TrapType>("type").Value;
            set => Traverse.Create(this).Field<TrapType>("type").Value = value;
        }

        public ResizeAxis ResizeMode
        {
            get => Traverse.Create(this).Field<ResizeAxis>("resizeMode").Value;
            set => Traverse.Create(this).Field<ResizeAxis>("resizeMode").Value = value;
        }

        public ResizeHandles.ClampType ClampMethod
        {
            get => Traverse.Create(this).Field<ResizeHandles.ClampType>("clampMethod").Value;
            set => Traverse.Create(this).Field<ResizeHandles.ClampType>("clampMethod").Value = value;
        }

        public Vector2 MinSize
        {
            get => Traverse.Create(this).Field<Vector2>("minSize").Value;
            set => Traverse.Create(this).Field<Vector2>("minSize").Value = value;
        }

        public float RoundToNearest
        {
            get => Traverse.Create(this).Field<float>("roundToNeareset").Value;
            set => Traverse.Create(this).Field<float>("roundToNeareset").Value = value;
        }
    }
}