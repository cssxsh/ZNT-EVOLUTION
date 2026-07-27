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

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        var names =
            from index in Enumerable.Range(0x00, 0x20)
            let name = LayerMask.LayerToName(index)
            select string.IsNullOrEmpty(name) ? index.ToString() : name;
        switch (member.Name)
        {
            case nameof(Main):
                menu.ListBinder().BindIndexListField(component, member, names.ToArray());
                return true;
            case nameof(Top):
                if (TopCollider) menu.ListBinder().BindIndexListField(component, member, names.ToArray());
                return true;
            case nameof(Bottom):
                if (BottomCollider) menu.ListBinder().BindIndexListField(component, member, names.ToArray());
                return true;
            default:
                return false;
        }
    }
}