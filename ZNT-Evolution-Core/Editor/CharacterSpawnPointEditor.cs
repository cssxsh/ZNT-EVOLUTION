using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Character")]
[DisallowMultipleComponent]
public class CharacterSpawnPointEditor : Editor
{
    protected CharacterSpawnPoint Spawn => field ??= GetComponent<CharacterSpawnPoint>();

    protected Parameters SendParams => Traverse.Create(Spawn).Field<Parameters>("sendParams").Value;

    [SerializeInEditor(name: "Dialogue Text")]
    public LocalizableString DialogueText = new() { Localize = false, Category = "Dialogues" };

    [SerializeInEditor(name: "Dialogue Duration")]
    public float DialogueDuration = 10;

    [SerializeInEditor(name: "Dialogue Voice")]
    public Voice DialogueVoice = Voice.None;

    protected virtual void Start()
    {
        // ReSharper disable once InvertIf
        if (!(DialogueDuration <= 0 || DialogueText.Content is null or ""))
        {
            SendParams.Update(
                "dialogue_text", DialogueText,
                "dialogue_duration", DialogueDuration,
                "dialogue_voice", DialogueVoice);
        }
    }
}