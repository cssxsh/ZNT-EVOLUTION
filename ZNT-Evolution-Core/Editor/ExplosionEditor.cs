using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Explosion")]
[DisallowMultipleComponent]
public class ExplosionEditor : Editor
{
    [JsonIgnore]
    private ExplosionEffect Effect => field ??= GetComponent<ExplosionEffect>();

    [JsonIgnore]
    private SphereDetection Detection => field ??= GetComponent<SphereDetection>();

    [JsonIgnore]
    private ExplosionAsset Asset => field ??= GetComponent<AssetComponent>().Asset as ExplosionAsset;

    [JsonIgnore]
    [SerializeInEditor(name: "Damage")]
    public float Damage
    {
        get => Effect.Damage;
        set => Effect.Damage = value;
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Damage Type")]
    public DamageType DamageType
    {
        get => Effect.DamageType;
        set => Effect.DamageType = value;
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Damage Radius")]
    public float DamageRadius
    {
        get => Detection.Radius;
        set => Detection.Radius = value;
    }

    [JsonIgnore]
    private Tag ApplyDamageOn
    {
        get => Effect.ApplyDamageOn;
        set => Effect.ApplyDamageOn = value;
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Damage Breakable")]
    public bool DamageBreakable
    {
        get => ApplyDamageOn.HasFlag(Tag.Breakable);
        set => ApplyDamageOn = value ? ApplyDamageOn.Add(Tag.Breakable) : ApplyDamageOn.Remove(Tag.Breakable);
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Damage Human")]
    public bool DamageHuman
    {
        get => ApplyDamageOn.HasFlag(Tag.Human);
        set => ApplyDamageOn = value ? ApplyDamageOn.Add(Tag.Human) : ApplyDamageOn.Remove(Tag.Human);
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Damage Zombie")]
    public bool DamageZombie
    {
        get => ApplyDamageOn.HasFlag(Tag.Zombie);
        set => ApplyDamageOn = value ? ApplyDamageOn.Add(Tag.Zombie) : ApplyDamageOn.Remove(Tag.Zombie);
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Force")]
    public float Force
    {
        get => Effect.Force;
        set => Effect.Force = value;
    }

    [JsonIgnore]
    private Tag ApplyForceOn
    {
        get => Effect.ApplyForceOn;
        set => Effect.ApplyForceOn = value;
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Force Human")]
    public bool ForceHuman
    {
        get => ApplyForceOn.HasFlag(Tag.Human);
        set => ApplyForceOn = value ? ApplyForceOn.Add(Tag.Human) : ApplyForceOn.Remove(Tag.Human);
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Force Zombie")]
    public bool ForceZombie
    {
        get => ApplyForceOn.HasFlag(Tag.Zombie);
        set => ApplyForceOn = value ? ApplyForceOn.Add(Tag.Zombie) : ApplyForceOn.Remove(Tag.Zombie);
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Shake Camera")]
    public bool ShakeCamera
    {
        get => Effect.ShakeCamera;
        set => Effect.ShakeCamera = value;
    }

    [SignalReceiver]
    public void StartExplosion()
    {
        if (Effect.Started) return;
        foreach (var effect in GetComponentsInChildren<ExplosionEffect>().Reverse()) effect.StartExplosion(Asset.Delay);
    }
}