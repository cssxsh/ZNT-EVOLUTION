using System;
using HarmonyLib;

namespace ZNT.Evolution.Core.Asset
{
    [Serializable]
    internal class ExplosionAssetWrapper : ExplosionAsset
    {
        public bool AutoExplode
        {
            get => Traverse.Create(this).Field<bool>("autoExplode").Value;
            set => Traverse.Create(this).Field<bool>("autoExplode").Value = value;
        }
    }
}