using UnityEngine;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Human")]
    [DisallowMultipleComponent]
    public class HumanEditor : EditorComponent
    {
        private HumanBehaviour Behaviour => GetComponentInChildren<HumanBehaviour>();
        
        [SerializeInEditor(name: "Voice")]
        public Voice Voice
        {
            get => Behaviour.Patroller.Voice;
            set => Behaviour.Patroller.Voice = value;
        }
    }
}