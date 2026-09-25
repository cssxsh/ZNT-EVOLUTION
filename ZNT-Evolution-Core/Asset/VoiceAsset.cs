using System.Collections.Generic;

namespace ZNT.Evolution.Core.Asset;

public class VoiceAsset : AssetElement
{
    internal static readonly SortedDictionary<int, VoiceAsset> Elements = new();

    public int index;

    public string path;
}