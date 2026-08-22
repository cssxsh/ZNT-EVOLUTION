using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZNT.Evolution.Host;

[BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.host", Name: "Evolution Host", Version: "0.1.0")]
public class EvolutionHostPlugin : BaseUnityPlugin
{
    [UsedImplicitly]
    internal static EvolutionHostPlugin Instance;

    [UsedImplicitly]
    internal static Harmony Harmony;

    public void Awake()
    {
        Instance = this;
        Harmony ??= new Harmony(Info.Metadata.GUID);
    }
}