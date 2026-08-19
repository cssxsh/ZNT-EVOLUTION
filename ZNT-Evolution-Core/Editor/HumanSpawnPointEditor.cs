using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZNT.Evolution.Core.Asset;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Human")]
[DisallowMultipleComponent]
public class HumanSpawnPointEditor : Editor, IEditorOverride
{
    private static SortedDictionary<string, HumanAsset> HumanAssets = new();

    private CharacterSpawnPoint Spawn => field ??= GetComponent<CharacterSpawnPoint>();

    private Parameters SendParams => Traverse.Create(Spawn).Field<Parameters>("sendParams").Value;

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        switch (member.Name)
        {
            case nameof(SelectedHuman1):
            case nameof(SelectedHuman2):
            case nameof(SelectedHuman3):
            case nameof(SelectedHuman4):
            case nameof(SelectedHuman5):
            case nameof(SelectedHuman6):
            case nameof(SelectedHuman7):
            case nameof(SelectedHuman8):
            case nameof(SelectedHuman9):
            case nameof(SelectedHumanX):
            {
                var value = member.GetMemberValue<string>(component.Data);
                var list = new List<string> { HumanAssets.Keys.Contains(value) ? "" : value };
                list.AddRange(HumanAssets.Keys);
                var binder = menu.ListBinder();
                binder.BindStringListField(component, member, list);
                if (list[0] is "") return true;
                var components = Traverse.Create(binder).Field<UIBehaviour[]>("uiComponents").Value;
                var dropdown = (Dropdown)components[0];
                var normal = dropdown.colors.normalColor;
                dropdown.onValueChanged.AddListener(index =>
                {
                    dropdown.colors = index is 0
                        ? dropdown.colors with { normalColor = Color.red }
                        : dropdown.colors with { normalColor = normal };
                    dropdown.RefreshShownValue();
                });
                dropdown.colors = dropdown.colors with { normalColor = Color.red };
                dropdown.RefreshShownValue();
            }
                return true;
            default:
                return false;
        }
    }

    [SerializeInEditor(name: "Dialogue Text")]
    public LocalizableString DialogueText = new() { Localize = false, Category = "Dialogues" };

    [SerializeInEditor(name: "Dialogue Duration")]
    public float DialogueDuration = 10;

    [SerializeInEditor(name: "Dialogue Voice")]
    public Voice DialogueVoice = Voice.None;

    [SerializeInEditor(name: "Selected Human 1")]
    public string SelectedHuman1 = "";

    [SerializeInEditor(name: "Selected Human 2")]
    public string SelectedHuman2 = "";

    [SerializeInEditor(name: "Selected Human 3")]
    public string SelectedHuman3 = "";

    [SerializeInEditor(name: "Selected Human 4")]
    public string SelectedHuman4 = "";

    [SerializeInEditor(name: "Selected Human 5")]
    public string SelectedHuman5 = "";

    [SerializeInEditor(name: "Selected Human 6")]
    public string SelectedHuman6 = "";

    [SerializeInEditor(name: "Selected Human 7")]
    public string SelectedHuman7 = "";

    [SerializeInEditor(name: "Selected Human 8")]
    public string SelectedHuman8 = "";

    [SerializeInEditor(name: "Selected Human 9")]
    public string SelectedHuman9 = "";

    [SerializeInEditor(name: "Selected Human X")]
    public string SelectedHumanX = "";

    private IEnumerable<HumanAsset> SelectedHumans()
    {
        if (HumanAssets.TryGetValue(SelectedHuman1, out var h1)) yield return h1;
        if (HumanAssets.TryGetValue(SelectedHuman2, out var h2)) yield return h2;
        if (HumanAssets.TryGetValue(SelectedHuman3, out var h3)) yield return h3;
        if (HumanAssets.TryGetValue(SelectedHuman4, out var h4)) yield return h4;
        if (HumanAssets.TryGetValue(SelectedHuman5, out var h5)) yield return h5;
        if (HumanAssets.TryGetValue(SelectedHuman6, out var h6)) yield return h6;
        if (HumanAssets.TryGetValue(SelectedHuman7, out var h7)) yield return h7;
        if (HumanAssets.TryGetValue(SelectedHuman8, out var h8)) yield return h8;
        if (HumanAssets.TryGetValue(SelectedHuman9, out var h9)) yield return h9;
        if (HumanAssets.TryGetValue(SelectedHumanX, out var hx)) yield return hx;
    }

    protected override void OnCreate()
    {
        SceneLoader.BeforeLoadScene += HumanAssets.Clear;
        if (HumanAssets.Count is not 0) return;
        foreach (var (_, asset) in CustomAssetUtility.Cache)
        {
            if (asset is not HumanAsset { HierarchyName: not (null or "") } human) continue;
            HumanAssets[human.HierarchyName] = human;
        }
    }

    private void Start()
    {
        // ReSharper disable once InvertIf
        if (!(DialogueText.Content is null or "" || DialogueDuration <= 0))
        {
            SendParams.Update(
                "dialogue_text", DialogueText,
                "dialogue_duration", DialogueDuration,
                "dialogue_voice", DialogueVoice);
        }

        // ReSharper disable once InvertIf
        if (SelectedHumans().Any())
        {
            Spawn.SpawnableObjects.Clear();
            Spawn.SpawnableObjects.AddRange(SelectedHumans());
        }
    }
}