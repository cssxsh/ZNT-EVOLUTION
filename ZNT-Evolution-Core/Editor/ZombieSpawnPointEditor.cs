using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZNT.Evolution.Core.Asset;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Zombie")]
[DisallowMultipleComponent]
public class ZombieSpawnPointEditor : CharacterSpawnPointEditor, IEditorOverride
{
    private static SortedDictionary<string, CharacterMutation> MutationAssets = new();

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        switch (member.Name)
        {
            case nameof(SelectedMutation1):
            case nameof(SelectedMutation2):
            case nameof(SelectedMutation3):
            case nameof(SelectedMutation4):
            case nameof(SelectedMutation5):
            case nameof(SelectedMutation6):
            case nameof(SelectedMutation7):
            case nameof(SelectedMutation8):
            case nameof(SelectedMutation9):
            case nameof(SelectedMutationX):
            {
                var value = member.GetMemberValue<string>(component.Data);
                var list = new List<string> { MutationAssets.ContainsKey(value) ? "" : value };
                list.AddRange(MutationAssets.Keys);
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

    [SerializeInEditor(name: "Selected Mutation 1")]
    public string SelectedMutation1 = "";

    [SerializeInEditor(name: "Selected Mutation 2")]
    public string SelectedMutation2 = "";

    [SerializeInEditor(name: "Selected Mutation 3")]
    public string SelectedMutation3 = "";

    [SerializeInEditor(name: "Selected Mutation 4")]
    public string SelectedMutation4 = "";

    [SerializeInEditor(name: "Selected Mutation 5")]
    public string SelectedMutation5 = "";

    [SerializeInEditor(name: "Selected Mutation 6")]
    public string SelectedMutation6 = "";

    [SerializeInEditor(name: "Selected Mutation 7")]
    public string SelectedMutation7 = "";

    [SerializeInEditor(name: "Selected Mutation 8")]
    public string SelectedMutation8 = "";

    [SerializeInEditor(name: "Selected Mutation 9")]
    public string SelectedMutation9 = "";

    [SerializeInEditor(name: "Selected Mutation X")]
    public string SelectedMutationX = "";

    private IEnumerable<CharacterMutation> SelectedMutations()
    {
        if (MutationAssets.TryGetValue(SelectedMutation1, out var h1)) yield return h1;
        if (MutationAssets.TryGetValue(SelectedMutation2, out var h2)) yield return h2;
        if (MutationAssets.TryGetValue(SelectedMutation3, out var h3)) yield return h3;
        if (MutationAssets.TryGetValue(SelectedMutation4, out var h4)) yield return h4;
        if (MutationAssets.TryGetValue(SelectedMutation5, out var h5)) yield return h5;
        if (MutationAssets.TryGetValue(SelectedMutation6, out var h6)) yield return h6;
        if (MutationAssets.TryGetValue(SelectedMutation7, out var h7)) yield return h7;
        if (MutationAssets.TryGetValue(SelectedMutation8, out var h8)) yield return h8;
        if (MutationAssets.TryGetValue(SelectedMutation9, out var h9)) yield return h9;
        if (MutationAssets.TryGetValue(SelectedMutationX, out var hx)) yield return hx;
    }

    private CharacterMutation[] mutations;

    private void RandomMutation()
    {
        Spawn.SpawnMutation = mutations?.GetRandom();
        SendParams.Update("mutation", Spawn.SpawnMutation);
    }

    protected override void OnCreate()
    {
        SceneLoader.BeforeLoadScene += MutationAssets.Clear;
        if (MutationAssets.Count is not 0) return;
        foreach (var (_, asset) in CustomAssetUtility.Cache)
        {
            if (asset is not CharacterMutation { MutationTarget: MutationTarget.Zombie } mutation) continue;
            MutationAssets[mutation.name] = mutation;
        }
    }

    protected override void Start()
    {
        base.Start();
        // ReSharper disable once InvertIf
        if (SelectedMutations().Any())
        {
            mutations = SelectedMutations().ToArray();
            Spawn.Event<UnityEvent>("OnSpawn").AddListener(RandomMutation);
            RandomMutation();
        }
    }
}