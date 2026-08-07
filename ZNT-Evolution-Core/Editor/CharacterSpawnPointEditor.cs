using System;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Character")]
[DisallowMultipleComponent]
public class CharacterSpawnPointEditor : Editor, IEditorOverride
{
    [JsonIgnore]
    private CharacterSpawnPoint Spawn => field ?? GetComponent<CharacterSpawnPoint>();

    [JsonIgnore]
    private Parameters SendParams => Traverse.Create(Spawn).Field<Parameters>("sendParams").Value;

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        return Traverse.Create(Spawn).Field<Enum>("spawnType").Value.ToString() is not "Human";
    }

    [SerializeInEditor(name: "Dialogue Text")]
    public string DialogueText = "";

    [SerializeInEditor(name: "Dialogue Duration")]
    public float DialogueDuration = 10;

    [SerializeInEditor(name: "Dialogue Voice")]
    public Voice DialogueVoice = Voice.None;

    private void Start()
    {
        // ReSharper disable once InvertIf
        if (!string.IsNullOrEmpty(DialogueText) && DialogueDuration > 0)
        {
            SendParams.Update(
                "dialogue_text", new LocalizableString { Localize = false, Content = DialogueText },
                "dialogue_duration", DialogueDuration,
                "dialogue_voice", DialogueVoice);
        }
    }
}