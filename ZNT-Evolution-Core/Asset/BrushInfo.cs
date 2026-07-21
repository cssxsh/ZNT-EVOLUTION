using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class BrushInfo : EvolutionInfo<Rotorz.Tile.OrientedBrush>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(BrushInfo));

    [JsonProperty("ForceLegacySideways")]
    public readonly bool ForceLegacySideways;

    [JsonProperty("ApplyPrefabTransform")]
    public readonly bool ApplyPrefabTransform;

    [JsonProperty("UserFlags")]
    public readonly int UserFlags;

    [JsonProperty("ForceOverrideFlags")]
    public readonly bool ForceOverrideFlags;

    [JsonProperty("Coalesce")]
    public readonly Rotorz.Tile.Coalesce Coalesce;

    [JsonProperty("Prefab")]
    public readonly GameObject Prefab;

    [JsonConstructor]
    public BrushInfo(
        string name,
        bool forceLegacySideways,
        bool applyPrefabTransform,
        int userFlags,
        bool forceOverrideFlags,
        Rotorz.Tile.Coalesce coalesce,
        GameObject prefab = null, GameObject variation = null) : base(name)
    {
        ForceLegacySideways = forceLegacySideways;
        ApplyPrefabTransform = applyPrefabTransform;
        UserFlags = userFlags;
        ForceOverrideFlags = forceOverrideFlags;
        Coalesce = coalesce;
        Prefab = prefab ?? variation;
        if (string.IsNullOrEmpty(Name)) Logger.LogWarning("Name is null");
        if (Prefab is null) Logger.LogWarning("Prefab is null");
    }

    public override Rotorz.Tile.OrientedBrush Create()
    {
        var impl = ScriptableObject.CreateInstance<Rotorz.Tile.OrientedBrush>();

        impl.hideFlags = HideFlags.HideAndDontSave;
        impl.name = Name;
        impl.group = 1;
        impl.forceLegacySideways = ForceLegacySideways;
        impl.applyPrefabTransform = ApplyPrefabTransform;
        Traverse.Create(impl).Field<int>("_userFlags").Value = UserFlags;
        impl.forceOverrideFlags = ForceOverrideFlags;
        impl.Coalesce = Coalesce;
        impl.AddOrientation(mask: 0).AddVariation(variation: Prefab, weight: 50);

        Object.DontDestroyOnLoad(impl);
        return impl;
    }
}