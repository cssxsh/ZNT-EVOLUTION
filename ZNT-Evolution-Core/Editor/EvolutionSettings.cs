using BepInEx.Logging;
using UIWidgets;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Evolution")]
[DisallowMultipleComponent]
public class EvolutionSettings : Editor
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(EvolutionSettings));

    private static EvolutionSettings instance;

    public static EvolutionSettings Instance =>
        instance ??= ComponentSingleton<LevelSettings>.Instance.gameObject.GetComponentSafe<EvolutionSettings>();

    internal static SpinnerFloat ExplosionProofSpinner;

    [SerializeInEditor(name: "Explosion Proof")]
    public float ExplosionProof = 50f;

    protected override void OnCreate()
    {
        SceneLoader.BeforeLoadScene += Reset;
    }

    public void Bind()
    {
        ExplosionProofSpinner.Min = 0;
        ExplosionProofSpinner.Max = 1748;
        ExplosionProofSpinner.onEndEditFloat.AddListener(v => ExplosionProof = v);
        ExplosionProofSpinner.Value = ExplosionProof;
    }

    public void Reset()
    {
        Logger.LogDebug("[Reset] ...");
    }

    public void Start()
    {
        Logger.LogDebug("[Start] ...");
    }

    public void OnDestroy()
    {
        SceneLoader.BeforeLoadScene -= Reset;
        instance = null;
    }
}