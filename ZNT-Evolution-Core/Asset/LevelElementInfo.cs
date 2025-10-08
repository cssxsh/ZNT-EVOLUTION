using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class LevelElementInfo
{
    [JsonProperty("CustomAsset")]
    public readonly string CustomAsset;

    [JsonConstructor]
    public LevelElementInfo(string asset) => CustomAsset = asset;
}