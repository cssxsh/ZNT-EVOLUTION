using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ZNT.Evolution.Core;

[BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.core", Name: "Evolution Core", Version: "0.7.3")]
public class EvolutionCorePlugin : BaseUnityPlugin
{
    internal static EvolutionCorePlugin Instance;
    
    internal static Harmony Harmony;

    internal static ConfigEntry<int> CorpsesCountMax;

    internal static ConfigEntry<bool> VisionMaterialization;

    internal static ConfigEntry<bool> ShowAllElement;

    internal static ConfigEntry<bool> ShowAllAnimationClip;

    internal static ConfigEntry<bool> ShowDevComponent;

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

    public void Start()
    {
        CorpsesCountMax = Config.Bind("config", nameof(CorpsesCountMax), GameConf.MaxAliveCorpses, "尸体数量上限");
        VisionMaterialization = Config.Bind("config", nameof(VisionMaterialization), false, "视觉射线渲染");
        ShowAllElement = Config.Bind("config", nameof(ShowAllElement), false, "显示所有元件");
        ShowAllAnimationClip = Config.Bind("config", nameof(ShowAllAnimationClip), false, "显示所有动画");
        ShowDevComponent = Config.Bind("config", nameof(ShowDevComponent), false, "显示实验组件");
    }
}