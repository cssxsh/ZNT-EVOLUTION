using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

public class LazyRef : CustomAssetObject
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(LazyRef));
    
    [UsedImplicitly]
    internal CustomAssetObject Fetch()
    {
        return CustomAssetUtility.Cache.TryGetValue(HierarchyName, out var value)
            ? value as CustomAssetObject
            : null;
    }

    public override void LoadFromAsset(GameObject gameObject)
    {
        if (Fetch() is { } asset) asset.LoadFromAsset(gameObject);
        else Logger.LogError($"NotFound \"{HierarchyName}\"");
    }
}