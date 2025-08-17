using UnityEngine;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Human")]
    [DisallowMultipleComponent]
    public class HumanEditor : EditorComponent
    {
        private HumanBehaviour Behaviour => GetComponentInChildren<HumanBehaviour>();

        [SerializeInEditor(name: "Flee Before Zombie Explode")]
        public bool FleeBeforeZombieExplode
        {
            get => Behaviour.FleeBeforeZombieExplode;
            set => Behaviour.FleeBeforeZombieExplode = value;
        }

        [SerializeInEditor(name: "Flee Duration")]
        public float FleeDuration
        {
            get => Behaviour.FleeingTimer.Duration;
            set => Behaviour.FleeingTimer.Duration = value;
        }

        [SerializeInEditor(name: "Direct Aim")]
        public bool DirectAim
        {
            get => Behaviour.Attacker.DirectAim;
            set => Behaviour.Attacker.DirectAim = value;
        }

        [SerializeInEditor(name: "Aim Stop Time")]
        public float AimStopTime
        {
            get => Behaviour.Attacker.StopAimingTime;
            set => Behaviour.Attacker.StopAimingTime = value;
        }

        [SerializeInEditor(name: "Aim Range")]
        public float AimRange
        {
            get => Behaviour.Attacker.AimRange;
            set => Behaviour.Attacker.AimRange = value;
        }

        [SerializeInEditor(name: "Voice")]
        public Voice Voice
        {
            get => Behaviour.Patroller.Voice;
            set => Behaviour.Patroller.Voice = value;
        }
    }
}