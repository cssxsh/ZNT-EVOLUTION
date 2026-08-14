using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class BrushMerge : EvolutionMerge<Rotorz.Tile.OrientedBrush>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(BrushMerge));

    [JsonProperty("Prefab")]
    public readonly GameObject Prefab;

    [JsonConstructor]
    public BrushMerge(
        Rotorz.Tile.OrientedBrush source,
        string name,
        GameObject prefab) : base(name, source)
    {
        Prefab = prefab;
        if (Name is null or "") Logger.LogWarning("Name is null");
        if (Source is null) Logger.LogWarning("Source is null");
        if (Prefab is null) Logger.LogWarning("Prefab is null");
    }

    public override Rotorz.Tile.OrientedBrush Create()
    {
        var clone = Object.Instantiate(Source);

        clone.name = Name;
        clone.DefaultOrientation.SetVariation(0, Prefab);

        Object.DontDestroyOnLoad(clone);
        return clone;
    }
}