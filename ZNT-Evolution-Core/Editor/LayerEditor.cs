using System.Linq;
using System.Reflection;
using UnityEngine;
using ZNT.LevelEditor;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Layer")]
[DisallowMultipleComponent]
public class LayerEditor : Editor, IEditorOverride
{
    [SerializeInEditor(name: "Main Layer")]
    public int Main
    {
        get => gameObject.layer;
        set => gameObject.layer = value;
    }

    private GameObject Child(string n) => gameObject.transform.Find(n)?.gameObject;

    private GameObject TopCollider => Child(nameof(TopCollider));

    [SerializeInEditor(name: "Top Layer")]
    public int Top
    {
        get => TopCollider?.layer ?? 0;
        set => TopCollider?.layer = value;
    }

    private GameObject BottomCollider => Child(nameof(BottomCollider));

    [SerializeInEditor(name: "Bottom Layer")]
    public int Bottom
    {
        get => BottomCollider?.layer ?? 0;
        set => BottomCollider?.layer = value;
    }

    private tk2dBaseSprite BaseSprite => GetComponentInChildren<tk2dBaseSprite>();

    [SerializeInEditor(name: "Sprite Layer")]
    public string Sprite
    {
        get => BaseSprite?.CachedRenderer.sortingLayerName ?? "";
        set => BaseSprite?.CachedRenderer.sortingLayerName = value;
    }

    [SerializeInEditor(name: "Sprite Order")]
    public int SpriteOrder
    {
        get => BaseSprite?.SortingOrder ?? 0;
        set => BaseSprite?.SortingOrder = value;
    }

    private static string[] _names;

    private static string[] _sorting;

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        _names ??= Enumerable.Range(0, 0x20)
            .Select(i => string.IsNullOrEmpty(LayerMask.LayerToName(i)) ? i.ToString() : LayerMask.LayerToName(i))
            .ToArray();
        _sorting ??= SortingLayer.layers
            .Select(l => l.name)
            .ToArray();
        switch (member.Name)
        {
            case nameof(Main):
                CustomBinder(menu).BindIndexListField(component, member, _names);
                return true;
            case nameof(Top):
                if (TopCollider) CustomBinder(menu).BindIndexListField(component, member, _names);
                return true;
            case nameof(Bottom):
                if (BottomCollider) CustomBinder(menu).BindIndexListField(component, member, _names);
                return true;
            case nameof(Sprite):
                if (BaseSprite) CustomBinder(menu).BindStringListField(component, member, _sorting);
                return true;
            default:
                return false;
        }
    }
}