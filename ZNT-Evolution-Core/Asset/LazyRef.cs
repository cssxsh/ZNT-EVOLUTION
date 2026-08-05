using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

public class LazyRef : CustomAssetObject
{
    [UsedImplicitly]
    internal CustomAssetObject Fetch()
    {
        return CustomAssetUtility.DeserializeObject<CustomAssetObject>(new JValue(HierarchyName));
    }

    public override void LoadFromAsset(GameObject gameObject)
    {
        if (Fetch() is not { } asset) throw new AssetException($"NotFound \"{HierarchyName}\"");
        asset.LoadFromAsset(gameObject);
    }
}