using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using ZNT.Evolution.Core.Asset;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Evolution")]
[DisallowMultipleComponent]
public class EvolutionSettings : Editor
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(EvolutionSettings));

    private static EvolutionSettings inst;

    private static LevelSettings LevelSettings => ComponentSingleton<LevelSettings>.Instance;

    public static EvolutionSettings Instance => inst ??= LevelSettings.gameObject.GetComponentSafe<EvolutionSettings>();

    internal static SpinnerFloat ExplosionProofSpinner;

    [SerializeInEditor(name: "Explosion Proof")]
    public float ExplosionProof = float.Epsilon;

    private void SetExplosionProof(float value) => ExplosionProof = value;

    internal static Button AssetOverrideImport;

    internal static Button AssetOverrideExport;

    [SerializeInEditor(name: "Asset Override Data")]
    public List<AssetOverrideRecord> AssetOverrideData { get; set; } = [];

    private void ImportAssetOverride()
    {
        try
        {
            AssetOverrideData =
                CustomAssetUtility.DeserializeObjectFromPath<List<AssetOverrideRecord>>("AssetOverride.json");
            Logger.LogInfo("ImportAssetOverride from AssetOverride.json");
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }
    }

    private void ExportAssetOverride()
    {
        try
        {
            if (AssetOverrideData is { Count: 0 })
            {
                AssetOverrideData.Add(new AssetOverrideRecord
                {
                    Type = nameof(HumanAsset),
                    Pattern = "This is pattern for name",
                    Handles = new JObject
                    {
                        [nameof(HumanAsset.RiseAsset)] = "SwordWomen : HumanAsset"
                    }
                });
            }

            CustomAssetUtility.SerializeObjectToPath("AssetOverride.json", AssetOverrideData);
            Logger.LogInfo("ExportAssetOverride to AssetOverride.json");
        }
        catch (System.Exception e)
        {
            Logger.LogError(e);
        }
    }

    [SerializeInEditor(name: "Unix Time Seconds")]
    private long UnixTimeSeconds
    {
        get => System.DateTimeOffset.Now.ToUnixTimeSeconds();
        set
        {
            _ = value;
            AssetOverrideUpdate();
            AssetOverrideSubmit();
        }
    }

    protected override void OnCreate()
    {
        SceneLoader.BeforeLoadScene += Reset;
    }

    public void Bind()
    {
        ExplosionProofSpinner.Min = 0;
        ExplosionProofSpinner.Max = 1748;
        ExplosionProofSpinner.onEndEditFloat = new SubmitEventFloat();
        ExplosionProofSpinner.onEndEditFloat.AddListener(SetExplosionProof);
        ExplosionProofSpinner.Value = ExplosionProof;
        AssetOverrideImport.onClick = new Button.ButtonClickedEvent();
        AssetOverrideImport.onClick.AddListener(ImportAssetOverride);
        AssetOverrideExport.onClick = new Button.ButtonClickedEvent();
        AssetOverrideExport.onClick.AddListener(ExportAssetOverride);
    }

    public void Reset()
    {
        AssetOverrideData.Clear();
        AssetOverrideUpdate();
    }

    public void OnDestroy()
    {
        Reset();
        SceneLoader.BeforeLoadScene -= Reset;
        inst = null;
        ExplosionProofSpinner = null;
    }

    #region AssetOverride

    [JsonObject]
    public class AssetOverrideRecord : IEnumerable<CustomAssetOverrider>
    {
        [JsonProperty]
        [UsedImplicitly]
        public string Type;

        [JsonProperty]
        [UsedImplicitly]
        public string Pattern;

        [JsonProperty]
        [UsedImplicitly]
        public JObject Handles;

        private static readonly Regex FieldRegex = new(@"(\w+)(?:\[(\w+)\])?([=|+|-|*|/])?", RegexOptions.Compiled);

        public IEnumerator<CustomAssetOverrider> GetEnumerator()
        {
            var type = AccessTools.TypeByName(Type);
            if (!typeof(CustomAsset).IsAssignableFrom(type)) yield break;
            foreach (var asset in CustomAssetUtility.Cache.Values.Where(type.IsInstanceOfType).Cast<CustomAsset>())
            {
                if (!Regex.IsMatch(asset.name, Pattern)) continue;
                foreach (var (path, token) in Handles)
                {
                    if (path is null or "") continue;
                    if (path[0] is '#' or '$') continue;
                    var match = FieldRegex.Match(path);
                    if (!match.Success) continue;
                    var field = type.GetField(
                        name: match.Groups[1].Value,
                        bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field is null) continue;
                    var index = match.Groups[2].Value;
                    var action = match.Groups[3].Value;
                    var id = $"{Type}[{asset.name}].{path}".ReplaceLast(action, "");
                    yield return new CustomAssetOverrider
                    {
                        Id = id,
                        Asset = asset,
                        Field = field,
                        Index = index,
                        Action = action,
                        Token = token
                    };
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private static readonly Dictionary<string, CustomAssetOverrider> AssetOverriderRegistered = new();

    public void AssetOverrideSubmit()
    {
        foreach (var (_, overrider) in AssetOverriderRegistered)
        {
            try
            {
                overrider.Submit();
            }
            catch (System.Exception e)
            {
                Logger.LogError(e);
            }
        }
    }

    public void AssetOverrideReset()
    {
        foreach (var (_, overrider) in AssetOverriderRegistered)
        {
            try
            {
                overrider.Reset();
            }
            catch (System.Exception e)
            {
                Logger.LogError(e);
            }
        }
    }

    public void AssetOverrideUpdate()
    {
        AssetOverrideReset();
        AssetOverriderRegistered.Clear();
        foreach (var overrider in
                 from record in AssetOverrideData
                 from overrider in record
                 select overrider)
        {
            if (AssetOverriderRegistered.TryGetValue(overrider.Id, out var registered))
            {
                Logger.LogWarning($"[AssetOverrideUpdate] {registered.Id} is already registered.");
            }

            AssetOverriderRegistered[overrider.Id] = overrider;
        }
    }

    #endregion
}