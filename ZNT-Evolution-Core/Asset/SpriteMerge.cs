using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    public readonly Dictionary<string, tk2dSpriteDefinition.AttachPoint[]> AttachPoints;

    [JsonProperty("Material")]
    public readonly Material Material;

    [JsonProperty("Transformed")]
    public readonly bool Transformed;

    [JsonConstructor]
    public SpriteMerge(
        tk2dSpriteCollectionData source,
        string name = null,
        Dictionary<string, tk2dSpriteDefinition.AttachPoint[]> points = null,
        Material material = null,
        bool transformed = false) : base(name, source)
    {
        AttachPoints = points ?? new Dictionary<string, tk2dSpriteDefinition.AttachPoint[]>();
        Material = material;
        Transformed = transformed;
        if (Source is null) Logger.LogWarning("Source is null");
    }

    public override tk2dSpriteCollectionData Create()
    {
        var clone = Object.Instantiate(Source);

        clone.name = Name ?? Regex.Replace(Material.name, "_mat$", "");
        clone.material = Material;
        clone.materials[0] = Material;
        clone.textures[0] = Material.mainTexture;
        for (var index = 0; index < clone.spriteDefinitions.Length; index++)
        {
            var definition = clone.spriteDefinitions[index];
            definition.material = Material;
            definition.attachPoints = Attach(definition.name, index);
        }

        Object.DontDestroyOnLoad(clone);
        return clone;
    }

    private tk2dSpriteDefinition.AttachPoint[] Attach(string name, int index)
    {
        _ = AttachPoints.TryGetValue(name, out var points) ||
            AttachPoints.TryGetValue(index.ToString(), out points);
        return System.Array.ConvertAll(points ?? [], point => new tk2dSpriteDefinition.AttachPoint
        {
            name = point.name,
            position = Transformed ? point.position / 2 / Source.halfTargetHeight : point.position,
            angle = point.angle
        });
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