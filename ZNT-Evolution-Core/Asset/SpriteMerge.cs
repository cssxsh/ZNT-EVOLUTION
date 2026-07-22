using System.Collections.Generic;
using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class SpriteMerge : EvolutionMerge<tk2dSpriteCollectionData>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(SpriteMerge));

    [JsonProperty("AttachPoints")]
    public readonly Dictionary<int, tk2dSpriteDefinition.AttachPoint[]> AttachPoints;

    [JsonProperty("Material")]
    public readonly Material Material;

    [JsonConstructor]
    public SpriteMerge(
        tk2dSpriteCollectionData source,
        string name = null,
        Dictionary<int, tk2dSpriteDefinition.AttachPoint[]> points = null,
        Material material = null) : base(name, source)
    {
        AttachPoints = points ?? new Dictionary<int, tk2dSpriteDefinition.AttachPoint[]>();
        Material = material;
        if (Source is null) Logger.LogWarning("Source is null");
    }

    public override tk2dSpriteCollectionData Create()
    {
        var clone = Object.Instantiate(Source);

        clone.name = Name ?? Material.name.Replace("_mat", "");
        clone.material = Material;
        clone.materials[0] = Material;
        clone.textures[0] = Material.mainTexture;
        foreach (var definition in clone.spriteDefinitions) definition.material = Material;
        foreach (var (index, points) in AttachPoints) clone.spriteDefinitions[index].attachPoints = points;

        Object.DontDestroyOnLoad(clone);
        return clone;
    }

    public SpriteMerge WithMaterial(Material material)
    {
        if (Material) return this;
        return new SpriteMerge(
            source: Source,
            name: Name,
            points: AttachPoints,
            material: material
        );
    }
}