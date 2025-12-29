using BepInEx.Logging;
using HarmonyLib;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Live;

internal static class MainMenuPatch
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(MainMenu));

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public static void Start(MainMenu __instance)
    {
        LiveManager.Instance.SetActive();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenu), "Quit")]
    public static void Quit(MainMenu __instance)
    {
        LiveManager.Instance.SetInactive();
    }
}