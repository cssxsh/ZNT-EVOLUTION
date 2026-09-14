using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Human")]
[DisallowMultipleComponent]
public class HumanEditor : CharacterEditor
{
    private HumanBehaviour Behaviour => field ??= GetComponent<HumanBehaviour>();

    private Tag Tags
    {
        get => gameObject.GetTags();
        set => gameObject.SetTags(value);
    }

    [SerializeInEditor(name: "Zombie Ignore")]
    public bool CannotAttack
    {
        get => Tags.HasFlag(Tag.CannotAttack);
        set => Tags = value ? Tags.Add(Tag.CannotAttack) : Tags.Remove(Tag.CannotAttack);
    }

    [SerializeInEditor(name: "Weapon Magazine Size")]
    public int WeaponMagazineSize
    {
        get => Behaviour.Weapon.DefaultMag.Size;
        set => Behaviour.Weapon.SetMagazine(value);
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

    [SerializeInEditor(name: "Block Opponents")]
    public bool BlockOpponents
    {
        get => Behaviour.Stopper.BlockOpponents;
        set => Behaviour.Stopper.Initialize(value, Behaviour.Stopper.MaxOpponents, Behaviour.OnStopperBreak);
    }

    [SerializeInEditor(name: "Max Opponents Block")]
    public int MaxOpponentsBlock
    {
        get => Behaviour.Stopper.MaxOpponents;
        set => Behaviour.Stopper.Initialize(Behaviour.Stopper.BlockOpponents, value, Behaviour.OnStopperBreak);
    }

    private void OnDespawned()
    {
        foreach (var (_, buff) in Buffs) buff.Remove(Character);
        Buffs.Clear();
    }
}