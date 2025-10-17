using UnityEngine;
using ZNT.Evolution.Core.Editor;

namespace ZNT.Evolution.Core.Asset;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
internal class RageAsset : CustomAssetObject
{
    public DamageDictionary<ExplosionAsset> ExplosionAssets = new();

    public override void LoadFromAsset(GameObject gameObject)
    {
        base.LoadFromAsset(gameObject);
        if (ExplosionAssets == null) return;
        gameObject.GetComponentSafe<RageEditor>().ExplosionAssets = ExplosionAssets;
    }
}