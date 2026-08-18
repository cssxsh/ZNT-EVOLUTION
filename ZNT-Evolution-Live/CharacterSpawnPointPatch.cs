using System;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Live;

public static class CharacterSpawnPointPatch
{
    [UsedImplicitly]
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(CharacterSpawnPoint));

    // ReSharper disable Unity.PerformanceAnalysis
    internal static Character Spawn(this SpawnPoint __instance, CharacterAsset asset)
    {
        var character = asset.CreateGameObject(position: __instance.transform.position).GetComponent<Character>();
        var parameters = Traverse.Create(__instance).Field<Parameters>("sendParams").Value;
        character.OnSpawn(parameters);
        return character;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    internal static void ShowMessage(this Character character, string text, float duration)
    {
        var patroller = character.Components.Patroller;
        if (patroller is null) return;
        if (patroller.Animator is HumanAnimationController controller &&
            controller.IconAnimator.Library.AnimationExists(text))
        {
            controller.PlayIcon(text);
            Timer.DelayedCall(duration, () => controller.IconAnimator.Renderer.enabled = false);
            return;
        }

        var dialogue = ComponentSingleton<GamePoolManager>.Instance
            .Spawn(nameof(Dialogue)).GetComponent<Dialogue>();
        dialogue.SetText(new LocalizableString { Localize = false, Content = text }, duration);
        dialogue.Show(patroller, patroller.DialogueOffset, Voice.None);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    internal static void SpawnCopy(this Character character, string id)
    {
        var asset = character.Components.Asset.Asset;
        var clone = asset.CreateGameObject(position: character.transform.position).GetComponent<Character>();
        clone.OnSpawn(new Parameters(id: id)
            .Update("move_on_start", false)
            .Update("orientation", character.transform.forward));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterSpawnPoint), "Start")]
    public static void Start(CharacterSpawnPoint __instance)
    {
        // if (__instance.Active) return;
        if (Traverse.Create(__instance).Field<Enum>("spawnType").Value.ToString() is not "Human") return;
        if (GameManager.Instance is null) return;
        var system = GameManager.Instance.DecorSystems[DecorSystemLayer.LayerType.Foreground];
        foreach (var (_, decor) in system.Decors)
        {
            if (__instance.transform.position - decor.Position != new Vector3(0.5f, -2.5f, 0.0f)) continue;
            switch (decor.Sprite.name)
            {
                case "Downtown Number 1":
                    LiveManager.SpawnPoints["1"] = __instance;
                    return;
                case "Downtown Number 2":
                    LiveManager.SpawnPoints["2"] = __instance;
                    return;
                case "Downtown Number 3":
                    LiveManager.SpawnPoints["3"] = __instance;
                    return;
                case "Downtown Number 4":
                    LiveManager.SpawnPoints["4"] = __instance;
                    return;
                case "Downtown Number 5":
                    LiveManager.SpawnPoints["5"] = __instance;
                    return;
                case "Downtown Number 6":
                    LiveManager.SpawnPoints["6"] = __instance;
                    return;
                case "Downtown Number 7":
                    LiveManager.SpawnPoints["7"] = __instance;
                    return;
                case "Downtown Number 8":
                    LiveManager.SpawnPoints["8"] = __instance;
                    return;
                case "Downtown Number 9":
                    LiveManager.SpawnPoints["9"] = __instance;
                    return;
                case "Downtown Number 10":
                    LiveManager.SpawnPoints["10"] = __instance;
                    return;
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Character), "OnDie")]
    public static void OnDie(Character __instance)
    {
        LiveManager.Users.Remove(__instance.name);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PatrolDialogue), "OnStart")]
    public static bool OnStart(PatrolDialogue __instance)
    {
        if (__instance.Patroller.Animator is not HumanAnimationController controller) return true;
        if (!controller.IconAnimator.Library.AnimationExists(__instance.Text.Content)) return true;
        controller.PlayIcon(__instance.Text.Content);
        Timer.DelayedCall(__instance.DialogueDuration, () => controller.IconAnimator.Renderer.enabled = false);
        // TODO: __instance.Offset;
        return false;
    }
}