using HarmonyLib;

namespace ZNT.Evolution.Core.Asset
{
    internal class ExplosionAssetWrap : ExplosionAsset
    {
        public bool AutoExplode
        {
            get => Traverse.Create(this).Field<bool>("autoExplode").Value;
            set => Traverse.Create(this).Field<bool>("autoExplode").Value = value;
        }
    }
}