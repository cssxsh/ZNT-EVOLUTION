using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ZNT.Evolution.Live;

[BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.live", Name: "Evolution Live", Version: "0.1.0")]
public class EvolutionLivePlugin : BaseUnityPlugin
{
    internal static EvolutionLivePlugin Instance;

    internal static ConfigEntry<string> AccessKeyId;

    internal static ConfigEntry<string> AccessKeySecret;

    internal static ConfigEntry<long> AppId;

    internal static ConfigEntry<string> Code;

    public void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(typeof(MainMenuPatch));
    }

    public void Start()
    {
        AccessKeyId = Config.Bind("api", nameof(AccessKeyId), "", "https://open-live.bilibili.com/open-manage");
        AccessKeySecret = Config.Bind("api", nameof(AccessKeySecret), "", "https://open-live.bilibili.com/open-manage");
        AppId = Config.Bind("api", nameof(AppId), 0L, "https://open-live.bilibili.com/open-manage");
        Code = Config.Bind("api", nameof(Code), "", "https://play-live.bilibili.com");
    }
}