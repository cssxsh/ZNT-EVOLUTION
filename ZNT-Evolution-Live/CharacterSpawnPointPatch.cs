using HarmonyLib;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Live;

public static class CharacterSpawnPointPatch
{
    // ReSharper disable Unity.PerformanceAnalysis
    internal static Character Spawn(this SpawnPoint __instance, CharacterAsset asset)
    {
        var character = asset.CreateGameObject(position: __instance.transform.position).GetComponent<Character>();
        var parameters = Traverse.Create(__instance).Field<Parameters>("sendParams").Value;
        character.OnSpawn(parameters);
        return character;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    internal static void ShowDialogue(this Character character, string text, float duration)
    {
        var patroller = character.Components.Patroller;
        if (patroller is null) return;
        var dialogue = ComponentSingleton<GamePoolManager>.Instance
            .Spawn(nameof(Dialogue)).GetComponent<Dialogue>();
        dialogue.SetText(new LocalizableString { Localize = false, Content = text }, duration);
        dialogue.Show(patroller, patroller.DialogueOffset, Voice.None);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    internal static void ShowIcon(this Character character, string name)
    {
        if (character.AnimationController is not HumanAnimationController controller) return;
        if (!controller.IconAnimator.Library.AnimationExists(name)) return;
        controller.PlayIcon(name);
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
        if (__instance.Active) return;
        var type = Traverse.Create(__instance).Field<System.Enum>("spawnType").Value;
        if (type.ToString() != "Human") return;
        LiveManager.SpawnPoints.Add(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SpawnPoint), "StopSpawn")]
    public static void StopSpawn(SpawnPoint __instance)
    {
        LiveManager.SpawnPoints.Remove(__instance);
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
        // if (!__instance.Text.Content.StartsWith("[") || !__instance.Text.Content.EndsWith("[")) return true;
        if (!controller.IconAnimator.Library.AnimationExists(__instance.Text.Content)) return true;
        controller.PlayIcon(__instance.Text.Content);
        Timer.DelayedCall(__instance.DialogueDuration, () => controller.IconAnimator.Renderer.enabled = false);
        // TODO: __instance.Offset;
        return false;
    }
}