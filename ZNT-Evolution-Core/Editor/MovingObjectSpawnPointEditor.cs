using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZNT.Evolution.Core.Asset;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Movement")]
[DisallowMultipleComponent]
public class MovingObjectSpawnPointEditor : Editor, IEditorOverride
{
    private SpawnPoint Spawn => field ??= GetComponent<SpawnPoint>();

    private Parameters SendParams => Traverse.Create(Spawn).Field<Parameters>("sendParams").Value;

    private static SortedDictionary<string, MovingObjectAsset> MovementAssets = new();

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        switch (member.Name)
        {
            case nameof(SelectedMovement1):
            case nameof(SelectedMovement2):
            case nameof(SelectedMovement3):
            case nameof(SelectedMovement4):
            case nameof(SelectedMovement5):
            case nameof(SelectedMovement6):
            case nameof(SelectedMovement7):
            case nameof(SelectedMovement8):
            case nameof(SelectedMovement9):
            case nameof(SelectedMovementX):
            {
                var value = member.GetMemberValue<string>(component.Data);
                var list = new List<string> { MovementAssets.ContainsKey(value) ? "" : value };
                list.AddRange(MovementAssets.Keys);
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

    [SerializeInEditor(name: "Speed Ease")]
    public Ease SpeedEase = Ease.InOutQuad;

    [SerializeInEditor(name: "Speed Ease Duration")]
    public float SpeedEaseDuration;

    [SerializeInEditor(name: "Selected Movement 1")]
    public string SelectedMovement1 = "";

    [SerializeInEditor(name: "Selected Movement 2")]
    public string SelectedMovement2 = "";

    [SerializeInEditor(name: "Selected Movement 3")]
    public string SelectedMovement3 = "";

    [SerializeInEditor(name: "Selected Movement 4")]
    public string SelectedMovement4 = "";

    [SerializeInEditor(name: "Selected Movement 5")]
    public string SelectedMovement5 = "";

    [SerializeInEditor(name: "Selected Movement 6")]
    public string SelectedMovement6 = "";

    [SerializeInEditor(name: "Selected Movement 7")]
    public string SelectedMovement7 = "";

    [SerializeInEditor(name: "Selected Movement 8")]
    public string SelectedMovement8 = "";

    [SerializeInEditor(name: "Selected Movement 9")]
    public string SelectedMovement9 = "";

    [SerializeInEditor(name: "Selected Movement X")]
    public string SelectedMovementX = "";

    private IEnumerable<MovingObjectAsset> SelectedMovements()
    {
        if (MovementAssets.TryGetValue(SelectedMovement1, out var h1)) yield return h1;
        if (MovementAssets.TryGetValue(SelectedMovement2, out var h2)) yield return h2;
        if (MovementAssets.TryGetValue(SelectedMovement3, out var h3)) yield return h3;
        if (MovementAssets.TryGetValue(SelectedMovement4, out var h4)) yield return h4;
        if (MovementAssets.TryGetValue(SelectedMovement5, out var h5)) yield return h5;
        if (MovementAssets.TryGetValue(SelectedMovement6, out var h6)) yield return h6;
        if (MovementAssets.TryGetValue(SelectedMovement7, out var h7)) yield return h7;
        if (MovementAssets.TryGetValue(SelectedMovement8, out var h8)) yield return h8;
        if (MovementAssets.TryGetValue(SelectedMovement9, out var h9)) yield return h9;
        if (MovementAssets.TryGetValue(SelectedMovementX, out var hx)) yield return hx;
    }

    protected override void OnCreate()
    {
        SceneLoader.BeforeLoadScene += MovementAssets.Clear;
        if (MovementAssets.Count is not 0) return;
        foreach (var (_, asset) in CustomAssetUtility.Cache)
        {
            if (asset is not MovingObjectAsset { HierarchyName: not (null or "") } movement) continue;
            MovementAssets[movement.HierarchyName] = movement;
        }
    }

    protected void Start()
    {
        // ReSharper disable once InvertIf
        if (!(SpeedEaseDuration <= 0 || SpeedEase is Ease.Unset))
        {
            SendParams.Update(
                "speed_ease", SpeedEase,
                "speed_ease_duration", SpeedEaseDuration);
        }

        // ReSharper disable once InvertIf
        if (SelectedMovements().Any())
        {
            Spawn.SpawnableObjects.Clear();
            Spawn.SpawnableObjects.AddRange(SelectedMovements());
        }
    }
}