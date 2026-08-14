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
internal class MaterialMerge : EvolutionMerge<Material>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(MaterialMerge));

    [JsonProperty("Shader")]
    public readonly Shader Shader;

    [JsonProperty("Textures")]
    public readonly Dictionary<string, Texture> Textures;

    [JsonProperty("Floats")]
    public readonly Dictionary<string, float> Floats;

    [JsonProperty("Colors")]
    public readonly Dictionary<string, Color> Colors;

    [JsonConstructor]
    public MaterialMerge(
        Material source,
        string name,
        Shader shader = null,
        Dictionary<string, Texture> textures = null,
        Dictionary<string, float> floats = null,
        Dictionary<string, Color> colors = null) : base(name, source)
    {
        Shader = shader;
        Textures = textures ?? new Dictionary<string, Texture>();
        Floats = floats ?? new Dictionary<string, float>();
        Colors = colors ?? new Dictionary<string, Color>();
        if (Name is null or "") Logger.LogWarning("Name is null");
        if (Shader is null) Logger.LogWarning("Shader is null");
        if (Source is null) Logger.LogWarning("Source is null");
    }

    public override Material Create()
    {
        var clone = new Material(Source)
        {
            name = Name,
            shader = Shader ?? Source.shader
        };
        foreach (var (name, texture) in Textures) clone.SetTexture(name, texture);
        foreach (var (name, value) in Floats) clone.SetFloat(name, value);
        foreach (var (name, color) in Colors) clone.SetColor(name, color);

        Object.DontDestroyOnLoad(clone);
        return clone;
    }
}