using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ZNT.Evolution.Core
{
    [BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.core", Name: "Evolution Core", Version: "0.6.7")]
    public class EvolutionCorePlugin : BaseUnityPlugin
    {
        internal static ConfigEntry<int> CorpsesCountMax;

        internal static ConfigEntry<bool> RayConeFindNearest;

        internal static ConfigEntry<bool> ShowAllElement;

        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(DebugPatch));
            Harmony.CreateAndPatchAll(typeof(GlobalSettingsPatch));
            Harmony.CreateAndPatchAll(typeof(StartManagerPatch));
            Harmony.CreateAndPatchAll(typeof(CustomAssetObjectPatch));
            Harmony.CreateAndPatchAll(typeof(SceneManagerPatch));
        }

        public void Start()
        {
            CorpsesCountMax = Config.Bind("config", "CorpsesCountMax", 20, "尸体数量上限");
            RayConeFindNearest = Config.Bind("config", "RayConeFindNearest", false, "视锥邻近查找");
            ShowAllElement = Config.Bind("config", "ShowAllElement", false, "显示所有组件");
        }
    }
}