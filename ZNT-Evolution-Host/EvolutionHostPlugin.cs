using BepInEx;
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

    public void Awake()
    {
        Instance = this;
        EvolutionHost.Logger = Logger;
        EvolutionHost.Uri = Config.Bind("http", "uri", "http://localhost:8080/");
        EvolutionHost.RootFolder = $"{Paths.PluginPath}/ZNT-Evolution-Host/www";
        Harmony ??= new Harmony(Info.Metadata.GUID);
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