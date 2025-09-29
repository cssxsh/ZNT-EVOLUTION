using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Human")]
    [DisallowMultipleComponent]
    public class HumanEditor : Editor
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

        [SerializeInEditor(name: "Invincible On Attack")]
        public bool InvincibleOnAttack
        {
            get => Behaviour.InvincibleOnAttack;
            set => Behaviour.InvincibleOnAttack = value;
        }

        [SerializeInEditor(name: "Ignore Damages")]
        public bool IgnoreDamages
        {
            get => Behaviour.IgnoreDamages;
            set => Behaviour.IgnoreDamages = value;
        }

        [SerializeInEditor(name: "Weapon Magazine Size")]
        public int WeaponMagazineSize
        {
            get => Behaviour.Weapon.DefaultMag.Size;
            set => Behaviour.Weapon.DefaultMag = new Magazine(value);
        }

        [SerializeInEditor(name: "Weapon Reload Type")]
        public ReloadType WeaponReloadType
        {
            get => Behaviour.Weapon.ReloadType;
            set => Behaviour.Weapon.ReloadType = value;
        }

        [SerializeInEditor(name: "Weapon Reload Time")]
        public float WeaponReloadTime
        {
            get => Behaviour.Weapon.ReloadTimer.Duration;
            set => Behaviour.Weapon.ReloadTimer.Duration = value;
        }

        [SerializeInEditor(name: "Weapon Stamina Refill Time")]
        public float WeaponStaminaRefillTimer
        {
            get => Behaviour.Weapon.StaminaRefillTimer.Duration;
            set => Behaviour.Weapon.StaminaRefillTimer.Duration = value;
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

        [SerializeInEditor(name: "Vision Cast All")]
        public bool VisionCastAll
        {
            get => ((RayConeDetection)Behaviour.Vision.Detection).CastAll;
            set => ((RayConeDetection)Behaviour.Vision.Detection).CastAll = value;
        }

        [SerializeInEditor(name: "Block Opponents")]
        public bool BlockOpponents
        {
            get => Traverse.Create(Behaviour.Stopper).Field<bool>("blockOpponents").Value;
            set => Traverse.Create(Behaviour.Stopper).Field<bool>("blockOpponents").Value = value;
        }

        [SerializeInEditor(name: "Max Opponents Block")]
        public int MaxOpponentsBlock
        {
            get => Traverse.Create(Behaviour.Stopper).Field<int>("MaxOpponents").Value;
            set => Traverse.Create(Behaviour.Stopper).Field<int>("MaxOpponents").Value = value;
        }

        [SerializeInEditor(name: "Voice")]
        public Voice Voice
        {
            get => Behaviour.Patroller.Voice;
            set => Behaviour.Patroller.Voice = value;
        }
    }
}