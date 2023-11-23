using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Barricade Layer")]
    [DisallowMultipleComponent]
    public class BarricadeLayerEditor : EditorComponent
    {
        [SerializeInEditor(name: "Main Layer")]
        public LayerType Layer
        {
            get => (LayerType)gameObject.layer;
            set => gameObject.layer = (int)value;
        }

        private GameObject Body(string n) => gameObject.GetChildren().Find(body => body.name == n);

        private GameObject TopCollider => Body("TopCollider") ?? gameObject;

        [SerializeInEditor(name: "Top Layer")]
        public LayerType Top
        {
            get => (LayerType)TopCollider.layer;
            set => TopCollider.layer = (int)value;
        }

        private GameObject BottomCollider => Body("BottomCollider") ?? gameObject;

        [SerializeInEditor(name: "Bottom Layer")]
        public LayerType Bottom
        {
            get => (LayerType)BottomCollider.layer;
            set => BottomCollider.layer = (int)value;
        }
    }
}