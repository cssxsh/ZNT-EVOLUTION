using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZNT.LevelEditor;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Layer")]
    [DisallowMultipleComponent]
    public class LayerEditor : EditorComponent, IEditorOverride
    {
        private static List<string> _names;

        private void Awake()
        {
            if (_names != null) return;
            _names = new List<string>(0x20);
            for (var i = 0; i < 0x20; i++)
            {
                _names[i] = LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(_names[i])) _names[i] = i.ToString();
            }
        }

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

        public bool OverrideMemberUi(SelectionMenu selectionMenu, global::EditorComponent component, MemberInfo member)
        {
            var binder = selectionMenu.InstantiateCustomBinder(selectionMenu.CustomBinders.IntStringList);
            switch (member.Name)
            {
                case "Main":
                    binder.BindIndexListField(component, member, _names);
                    return true;
                case "Top":
                    binder.BindIndexListField(component, member, _names);
                    binder.EditorVisibility = !(Child("TopCollider") is null);
                    return true;
                case "Bottom":
                    binder.BindIndexListField(component, member, _names);
                    binder.EditorVisibility = !(Child("BottomCollider") is null);
                    return true;
                default:
                    return false;
            }
        }
    }
}