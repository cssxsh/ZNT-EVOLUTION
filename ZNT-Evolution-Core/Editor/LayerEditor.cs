using UnityEngine;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Layer")]
    [DisallowMultipleComponent]
    public class LayerEditor : EditorComponent
    {
        [SerializeInEditor(name: "Main Layer")]
        public Layer Layer
        {
            get => (Layer)(0x01 << gameObject.layer);
            set => gameObject.layer = LayerMask.NameToLayer(value.ToString());
        }
    }
}