using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Human")]
[DisallowMultipleComponent]
public class HumanEditor : Editor
{
    private HumanBehaviour Behaviour => GetComponent<HumanBehaviour>();

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

    [SerializeInEditor(name: "Resist Scream")]
    public bool ResistScream
    {
        get => Behaviour.ResistScream;
        set => Behaviour.ResistScream = value;
    }

    [SerializeInEditor(name: "Grabbed On Attacked")]
    public bool GrabbedOnAttacked
    {
        get => Behaviour.GrabbedOnAttacked;
        set => Behaviour.GrabbedOnAttacked = value;
    }

    [SerializeInEditor(name: "Weapon Magazine Size")]
    public int WeaponMagazineSize
    {
        get => Behaviour.Weapon.DefaultMag.Size;
        set => Behaviour.Weapon.DefaultMag = new Magazine(value);
    }

    [SerializeInEditor(name: "Weapon Reload Time")]
    public float WeaponReloadTime
    {
        get => Behaviour.Weapon.ReloadTimer.Duration;
        set => Behaviour.Weapon.ReloadTimer.Duration = value;
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

    [SerializeInEditor(name: "Attack Frequency")]
    public float AttackFrequency
    {
        get => Behaviour.Attacker.AttackFrequency;
        set => Behaviour.Attacker.AttackFrequency = value;
    }

    [SerializeInEditor(name: "Attack Range")]
    public float AttackRange
    {
        get => Behaviour.Attacker.AttackRange;
        set => Behaviour.Attacker.AttackRange = value;
    }

    [SerializeInEditor(name: "Moving Attack Range")]
    public float MovingAttackRange
    {
        get => Behaviour.Attacker.MovingAttackRange;
        set => Behaviour.Attacker.MovingAttackRange = value;
    }

    [SerializeInEditor(name: "Damage")]
    public float Damage
    {
        get => Behaviour.Attacker.Damage;
        set => Behaviour.Attacker.Damage = value;
    }

    [SerializeInEditor(name: "Damage Type")]
    public DamageType DamageType
    {
        get => Behaviour.Attacker.DamageType;
        set => Behaviour.Attacker.DamageType = value;
    }

    [SerializeInEditor(name: "Damage Range")]
    public float DamageRange
    {
        get => Behaviour.Attacker.DamageRange;
        set => Behaviour.Attacker.DamageRange = value;
    }

    [SerializeInEditor(name: "Hit Multiple Targets")]
    public bool HitMultipleTargets
    {
        get => Behaviour.Attacker.HitMultipleTargets;
        set => Behaviour.Attacker.HitMultipleTargets = value;
    }

    [SerializeInEditor(name: "Hit Max Targets")]
    public int MaxTargets
    {
        get => Behaviour.Attacker.MaxTargets;
        set => Behaviour.Attacker.MaxTargets = value;
    }

    [SerializeInEditor(name: "Hit Targets Damage Multiplier")]
    public float NextTargetsDamageMultiplier
    {
        get => Behaviour.Attacker.NextTargetsDamageMultiplier;
        set => Behaviour.Attacker.NextTargetsDamageMultiplier = value;
    }

    [SerializeInEditor(name: "Vision Cast All", devOnly: true)]
    public bool VisionCastAll
    {
        get => ((RayConeDetection)Behaviour.Vision.Detection).CastAll;
        set => ((RayConeDetection)Behaviour.Vision.Detection).CastAll = value;
    }

    [SerializeInEditor(name: "Vision Keep Lost Track", devOnly: true)]
    public bool VisionKeepLostTrack
    {
        get => ((SignalEffect)Behaviour.Vision.Effects[0]).KeepLostTrack;
        set => ((SignalEffect)Behaviour.Vision.Effects[0]).KeepLostTrack = value;
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

    private void OnDespawned()
    {
        var prefab = Behaviour.SharedAsset?.Prefab.GetComponent<HumanEditor>();
        if (prefab is null) return;
        VisionCastAll = prefab.VisionCastAll;
        VisionKeepLostTrack = prefab.VisionKeepLostTrack;
    }
}