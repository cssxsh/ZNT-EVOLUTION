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
}