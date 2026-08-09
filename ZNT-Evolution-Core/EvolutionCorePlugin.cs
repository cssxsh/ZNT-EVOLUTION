using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZNT.Evolution.Core;

[BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.core", Name: "Evolution Core", Version: "0.7.3")]
public class EvolutionCorePlugin : BaseUnityPlugin
{
    [UsedImplicitly]
    internal static EvolutionCorePlugin Instance;

    [UsedImplicitly]
    internal static Harmony Harmony;

    public void Awake()
    {
        Instance = this;
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll(typeof(DebugPatch));
        Harmony.PatchAll(typeof(GlobalSettingsPatch));
        Harmony.PatchAll(typeof(StartManagerPatch));
        Harmony.PatchAll(typeof(CustomAssetObjectPatch));
        Harmony.PatchAll(typeof(AnimationEventHandlerPatch));
        Harmony.PatchAll(typeof(SceneLoaderPatch));
        Harmony.PatchAll(typeof(SnakeFeetPatch));
        Harmony.PatchAll(typeof(TextAssetPatch));
    }
}