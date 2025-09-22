using System.Reflection;
using UnityEngine;
using ZNT.LevelEditor;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Layer")]
    [DisallowMultipleComponent]
    public class LayerEditor : EditorComponent, IEditorOverride
    {
        [SerializeInEditor(name: "Main Layer")]
        public int Main
        {
            get => gameObject.layer;
            set => gameObject.layer = value;
        }

        private GameObject Child(string n) => gameObject.transform.Find(n)?.gameObject;

        private GameObject TopCollider => Child("TopCollider") ?? gameObject;

        [SerializeInEditor(name: "Top Layer")]
        public int Top
        {
            get => TopCollider.layer;
            set => TopCollider.layer = value;
        }

        private GameObject BottomCollider => Child("BottomCollider") ?? gameObject;

        [SerializeInEditor(name: "Bottom Layer")]
        public int Bottom
        {
            get => BottomCollider.layer;
            set => BottomCollider.layer = value;
        }
        
        private static string[] _names;

        private string[] LayerNames()
        {
            if (_names != null) return _names;
            _names = new string[0x20];
            for (var i = 0; i < 0x20; i++)
            {
                _names[i] = LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(_names[i])) _names[i] = i.ToString();
            }
            return _names;
        }

        public bool OverrideMemberUi(SelectionMenu selectionMenu, global::EditorComponent component, MemberInfo member)
        {
            var binder = selectionMenu.InstantiateCustomBinder(selectionMenu.CustomBinders.IntStringList);
            switch (member.Name)
            {
                case nameof(Main):
                    binder.BindIndexListField(component, member, LayerNames());
                    return true;
                case nameof(Top):
                    binder.BindIndexListField(component, member, LayerNames());
                    if (Child("TopCollider") is null) binder.EditorVisibility = false;
                    return true;
                case nameof(Bottom):
                    binder.BindIndexListField(component, member, LayerNames());
                    if (Child("BottomCollider") is null) binder.EditorVisibility = false;
                    return true;
                default:
                    return false;
            }
        }
    }
}