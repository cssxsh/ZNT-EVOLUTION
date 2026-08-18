using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZNT.Evolution.Live;

[BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.live", Name: "Evolution Live", Version: "0.1.0")]
public class EvolutionLivePlugin : BaseUnityPlugin
{
    [UsedImplicitly]
    internal static EvolutionLivePlugin Instance;

    [UsedImplicitly]
    internal static Harmony Harmony;

    [UsedImplicitly]
    internal static ConfigEntry<string> AccessKeyId;

    [UsedImplicitly]
    internal static ConfigEntry<string> AccessKeySecret;

    [UsedImplicitly]
    internal static ConfigEntry<long> AppId;

    [UsedImplicitly]
    internal static ConfigEntry<string> Code;

    public void Awake()
    {
        Instance = this;
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll(typeof(MainMenuPatch));
        Harmony.PatchAll(typeof(CharacterSpawnPointPatch));
    }

    public void Start()
    {
        AccessKeyId = Config.Bind("api", nameof(AccessKeyId), "", "https://open-live.bilibili.com/open-manage");
        AccessKeySecret = Config.Bind("api", nameof(AccessKeySecret), "", "https://open-live.bilibili.com/open-manage");
        AppId = Config.Bind("api", nameof(AppId), 0L, "https://open-live.bilibili.com/open-manage");
        Code = Config.Bind("api", nameof(Code), "", "https://play-live.bilibili.com");
    }
}