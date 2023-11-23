using UnityEngine;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Layer")]
    [DisallowMultipleComponent]
    public class LayerEditor : EditorComponent
    {
        [SerializeInEditor(name: "Main Layer")]
        public LayerType Layer
        {
            get => (LayerType)gameObject.layer;
            set => gameObject.layer = (int)value;
        }
    }
}