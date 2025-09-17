using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Barricade Layer")]
    [DisallowMultipleComponent]
    public class BarricadeLayerEditor : LayerEditor
    {
        private GameObject Body(string child) => gameObject.transform.Find(child)?.gameObject;

        private GameObject TopCollider => Body("TopCollider") ?? gameObject;

        [SerializeInEditor(name: "Top Layer")]
        public Layer Top
        {
            get => (Layer)(0x01 << TopCollider.layer);
            set => TopCollider.layer = LayerMask.NameToLayer(value.ToString());
        }

        private GameObject BottomCollider => Body("BottomCollider") ?? gameObject;

        [SerializeInEditor(name: "Bottom Layer")]
        public Layer Bottom
        {
            get => (Layer)(0x01 << BottomCollider.layer);
            set => BottomCollider.layer = LayerMask.NameToLayer(value.ToString());
        }
    }
}