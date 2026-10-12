using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZNT.Evolution.Host;

[BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.host", Name: "Evolution Host", Version: "0.1.0")]
[BepInDependency(DependencyGUID: "xyz.cssxsh.znt.evolution.core")]
public class EvolutionHostPlugin : BaseUnityPlugin
{
    [UsedImplicitly]
    internal static EvolutionHostPlugin Instance;

    [UsedImplicitly]
    internal static Harmony Harmony;

    [UsedImplicitly]
    internal static ConfigEntry<string> Uri;

    public void Awake()
    {
        Instance = this;
        Uri = Config.Bind("http", "uri", "http://localhost:8080/");
        Harmony ??= new Harmony(Info.Metadata.GUID);
        EvolutionHost.Init();
    }

    public void Start()
    {
        EvolutionHost.Start();
    }

    public void OnDestroy()
    {
        EvolutionHost.Stop();
    }
}