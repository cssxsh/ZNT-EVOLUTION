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

    private GameObject TopCollider => Child(nameof(TopCollider)) ?? gameObject;

    [SerializeInEditor(name: "Top Layer")]
    public int Top
    {
        get => TopCollider.layer;
        set => TopCollider.layer = value;
    }

    private GameObject BottomCollider => Child(nameof(BottomCollider)) ?? gameObject;

    [SerializeInEditor(name: "Bottom Layer")]
    public int Bottom
    {
        get => BottomCollider.layer;
        set => BottomCollider.layer = value;
    }

    private static string[] _names;

    protected override void OnCreate()
    {
        if (_names != null) return;
        _names = new string[0x20];
        for (var i = 0; i < 0x20; i++)
        {
            _names[i] = LayerMask.LayerToName(i);
            if (string.IsNullOrEmpty(_names[i])) _names[i] = i.ToString();
        }
    }

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        switch (member.Name)
        {
            case nameof(Main):
                CustomBinder(menu).BindIndexListField(component, member, _names);
                return true;
            case nameof(Top):
                if (Child(nameof(TopCollider))) CustomBinder(menu).BindIndexListField(component, member, _names);
                return true;
            case nameof(Bottom):
                if (Child(nameof(BottomCollider))) CustomBinder(menu).BindIndexListField(component, member, _names);
                return true;
            default:
                return false;
        }
    }
}