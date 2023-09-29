using BepInEx;
using HarmonyLib;

namespace ZNT.Evolution.Core
{
    [BepInPlugin("xyz.cssxsh.znt.evolution.core", "Evolution Core", "0.1.0")]
    public class EvolutionCorePlugin : BaseUnityPlugin
    {
        public void Start()
        {
            new Harmony(Info.Metadata.GUID).PatchAll();
        }
    }
}