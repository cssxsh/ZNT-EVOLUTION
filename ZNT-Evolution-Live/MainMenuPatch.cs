using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Live;

internal static class MainMenuPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ZNT.LevelEditor.MainMenu), "OnCreate")]
    public static void OnCreate(ZNT.LevelEditor.MainMenu __instance)
    {
        var name = Traverse.Create(__instance).Field<Text>("levelName").Value;
        {
            var tooltip = name.gameObject.AddComponent<TooltipReceiver>();
            Traverse.Create(tooltip).Field<Vector2>("offset").Value = new Vector2(-50.0f, -50.0f);
            Traverse.Create(tooltip).Field<string>("text").Value = LiveManager.Instance.LiveState.Term;
            var button = name.gameObject.AddComponent<Button>();
            button.onClick.AddListener(LiveManager.Instance.ToggleActivation);
        }
    }
}