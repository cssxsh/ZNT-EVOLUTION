using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class SpriteInfo : EvolutionInfo<tk2dSpriteCollectionData>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(SpriteInfo));

    [JsonProperty("OrthoSize")]
    public readonly float OrthoSize;

    [JsonProperty("TargetHeight")]
    public readonly float TargetHeight;

    [JsonProperty("Names")]
    public readonly string[] Names;

    [JsonProperty("Regions")]
    public readonly Rect[] Regions;

    [JsonProperty("Anchors")]
    public readonly Vector2[] Anchors;

    [JsonProperty("AttachPoints")]
    public readonly Dictionary<int, tk2dSpriteDefinition.AttachPoint[]> AttachPoints;

    [JsonProperty("Material")]
    public readonly Material Material;

    [JsonProperty("Transformed")]
    public readonly bool Transformed;

    [JsonConstructor]
    public SpriteInfo(
        float orthoSize,
        float targetHeight,
        string[] names,
        Rect[] regions,
        Vector2[] anchors = null, Vector2? anchor = null,
        Dictionary<int, tk2dSpriteDefinition.AttachPoint[]> points = null,
        string name = null,
        Material material = null,
        bool transformed = false) : base(name)
    {
        OrthoSize = orthoSize;
        TargetHeight = targetHeight;
        Names = names;
        Regions = regions;
        Anchors = anchors ?? Regions.Select(region => anchor ?? (region.size / 2)).ToArray();
        AttachPoints = points ?? new Dictionary<int, tk2dSpriteDefinition.AttachPoint[]>();
        Material = material;
        Transformed = transformed;
        if (Names.Length != Regions.Length) Logger.LogWarning("Names.Length != Regions.Length");
        if (Names.Length != Anchors.Length) Logger.LogWarning("Names.Length != Anchors.Length");
    }

    public override tk2dSpriteCollectionData Create()
    {
        var names = new string[Regions.Length];
        var regions = new Rect[Regions.Length];
        var anchors = new Vector2[Regions.Length];
        for (var i = 0; i < Regions.Length; i++)
        {
            names[i] = Names.IsIndexValid(i) ? Names[i] : $"region_{i}";
            regions[i] = Regions[i];
            anchors[i] = Anchors.IsIndexValid(i) ? Anchors[i] : Regions[i].size / 2;
            if (!Transformed) continue;
            anchors[i].y = regions[i].height - anchors[i].y;
            regions[i].y = Material.mainTexture.height - regions[i].height - regions[i].y;
        }

        var impl = tk2dSpriteCollectionData.CreateFromTexture(
            texture: Material.mainTexture,
            size: tk2dSpriteCollectionSize.Explicit(orthoSize: OrthoSize, targetHeight: TargetHeight),
            names: names,
            regions: regions,
            anchors: anchors
        );

        impl.name = Name ?? Material.name.Replace("_mat", "");
        impl.material = Material;
        impl.materials[0] = Material;
        foreach (var definition in impl.spriteDefinitions) definition.material = Material;
        foreach (var (index, points) in AttachPoints) impl.spriteDefinitions[index].attachPoints = points;

        Object.DontDestroyOnLoad(impl);
        return impl;
    }

    public SpriteInfo WithMaterial(Material material)
    {
        if (Material) return this;
        return new SpriteInfo(
            orthoSize: OrthoSize,
            targetHeight: TargetHeight,
            names: Names,
            regions: Regions,
            anchors: Anchors,
            points: AttachPoints,
            name: Name,
            material: material,
            transformed: Transformed
        );
    }
}