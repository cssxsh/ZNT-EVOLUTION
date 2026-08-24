using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Sentry")]
[DisallowMultipleComponent]
public class SentryGunEditor : Editor
{
    private SentryGunBehaviour Behaviour => field ??= GetComponent<SentryGunBehaviour>();

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
}