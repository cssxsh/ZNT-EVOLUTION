using JetBrains.Annotations;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

public class LazyRef : CustomAssetObject
{
    [UsedImplicitly]
    internal CustomAssetObject Fetch()
    {
        CustomAssetUtility.Cache.TryGetValue(HierarchyName, out var value);
        return value as CustomAssetObject;
    }

    public override void LoadFromAsset(GameObject gameObject)
    {
        if (Fetch() is { } asset) asset.LoadFromAsset(gameObject);
        throw new AssetException($"NotFound \"{HierarchyName}\"");
    }
}