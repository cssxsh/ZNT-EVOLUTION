using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Live;

internal static class MainMenuPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public static void Start(MainMenu __instance)
    {
        var canvas = __instance.transform.Find("Canvas");
        var live = Object.Instantiate(original: canvas.Find("Stats Button"), parent: canvas);
        {
            live.name = "Live Button";
            live.transform.localPosition = new Vector3(-77.0f, -208.0f, 0.0f);
            var tooltip = live.GetComponent<TooltipReceiver>();
            Traverse.Create(tooltip).Field<string>("text").Value = LiveManager.Instance.LiveState.Term;
            var button = live.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(LiveManager.Instance.ToggleActivation);
            var image = live.GetComponent<Image>();
            image.enabled = false;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenu), "Quit")]
    public static void Quit(MainMenu __instance)
    {
        LiveManager.Instance.SetInactive();
    }

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