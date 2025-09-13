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

        [SerializeInEditor(name: "Alert Duration")]
        public float AlertDuration
        {
            get => Behaviour.AlertedTimer.Duration;
            set => Behaviour.AlertedTimer.Duration = value;
        }

        [SerializeInEditor(name: "Alert Radius")]
        public float AlertRadius
        {
            get => ((SphereDetection)Behaviour.Character.Components.AlertZone.Detection).Radius;
            set => ((SphereDetection)Behaviour.Character.Components.AlertZone.Detection).Radius = value;
        }

        [SerializeInEditor(name: "Alert Relay Radius")]
        public float AlertRelayRadius
        {
            get => Behaviour.AlertRelayRadius;
            set => Behaviour.AlertRelayRadius = value;
        }

        [SerializeInEditor(name: "Alert Relay Ratio")]
        public float AlertRelayRatio
        {
            get => Behaviour.AlertRelayRatio;
            set => Behaviour.AlertRelayRatio = value;
        }

        [SerializeInEditor(name: "Alert Relay Over Time")]
        public bool AlertRelayOverTime
        {
            get => Behaviour.RelayAlertOverTime;
            set => Behaviour.RelayAlertOverTime = value;
        }

        [SerializeInEditor(name: "Vision All")]
        public bool VisionAll
        {
            get => ((RayConeDetection)Behaviour.Vision.Detection).CastAll;
            set => ((RayConeDetection)Behaviour.Vision.Detection).CastAll = value;
        }

        [SerializeInEditor(name: "Voice")]
        public Voice Voice
        {
            get => Behaviour.Patroller.Voice;
            set => Behaviour.Patroller.Voice = value;
        }
    }
}