using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Explosion")]
[DisallowMultipleComponent]
public class ExplosionEditor : Editor
{
    private ExplosionEffect Effect => field ??= GetComponent<ExplosionEffect>();

    private SphereDetection Detection => field ??= Effect.GetComponent<SphereDetection>();

    private ExplosionEffect TilesEffect => field ??= transform.Find("DetectTiles").GetComponent<ExplosionEffect>();

    private SphereDetection TilesDetection => field ??= TilesEffect.GetComponent<SphereDetection>();

    [SerializeInEditor(name: "Detected Radius")]
    public float DetectedRadius
    {
        get => Detection.Radius;
        set => Detection.Radius = value;
    }

    private LayerMask Layers
    {
        get => Effect.Trigger.Layers;
        set => Effect.Trigger.Layers = value;
    }

    [SerializeInEditor(name: "Detected Human")]
    public bool DetectedHuman
    {
        get => Layers.ContainsLayer("Human");
        set => Layers = value ? Layers.AddLayer("Human") : Layers.RemoveLayer("Human");
    }

    [SerializeInEditor(name: "Detected Zombie")]
    public bool DetectedZombie
    {
        get => Layers.ContainsLayer("Zombie");
        set => Layers = value ? Layers.AddLayer("Human") : Layers.RemoveLayer("Zombie");
    }

    [SerializeInEditor(name: "Detected World Enemy")]
    public bool DetectedWorldEnemy
    {
        get => Layers.ContainsLayer("World Enemy");
        set => Layers = value ? Layers.AddLayer("World Enemy") : Layers.RemoveLayer("World Enemy");
    }

    [SerializeInEditor(name: "Damage")]
    public float Damage
    {
        get => Effect.Damage;
        set => Effect.Damage = value;
    }

    [SerializeInEditor(name: "Damage Type")]
    public DamageType DamageType
    {
        get => Effect.DamageType;
        set => Effect.DamageType = value;
    }

    private Tag ApplyDamageOn
    {
        get => Effect.ApplyDamageOn;
        set => Effect.ApplyDamageOn = value;
    }

    [SerializeInEditor(name: "Damage Human")]
    public bool DamageHuman
    {
        get => ApplyDamageOn.HasFlag(Tag.Human);
        set => ApplyDamageOn = value ? ApplyDamageOn.Add(Tag.Human) : ApplyDamageOn.Remove(Tag.Human);
    }

    [SerializeInEditor(name: "Damage Zombie")]
    public bool DamageZombie
    {
        get => ApplyDamageOn.HasFlag(Tag.Zombie);
        set => ApplyDamageOn = value ? ApplyDamageOn.Add(Tag.Zombie) : ApplyDamageOn.Remove(Tag.Zombie);
    }

    [SerializeInEditor(name: "Damage World Enemy")]
    public bool DamageWorldEnemy
    {
        get => ApplyDamageOn.HasFlag(Tag.WorldEnemy);
        set => ApplyDamageOn = value ? ApplyDamageOn.Add(Tag.WorldEnemy) : ApplyDamageOn.Remove(Tag.WorldEnemy);
    }

    [SerializeInEditor(name: "Force")]
    public float Force
    {
        get => Effect.Force;
        set => Effect.Force = value;
    }

    [SerializeInEditor(name: "Force Mode")]
    public ForceMode2D ForceMode
    {
        get => Effect.ForceMode;
        set => Effect.ForceMode = value;
    }

    private Tag ApplyForceOn
    {
        get => Effect.ApplyForceOn;
        set => Effect.ApplyForceOn = value;
    }

    [SerializeInEditor(name: "Force Human")]
    public bool ForceHuman
    {
        get => ApplyForceOn.HasFlag(Tag.Human);
        set => ApplyForceOn = value ? ApplyForceOn.Add(Tag.Human) : ApplyForceOn.Remove(Tag.Human);
    }

    [SerializeInEditor(name: "Force Zombie")]
    public bool ForceZombie
    {
        get => ApplyForceOn.HasFlag(Tag.Zombie);
        set => ApplyForceOn = value ? ApplyForceOn.Add(Tag.Zombie) : ApplyForceOn.Remove(Tag.Zombie);
    }

    [SerializeInEditor(name: "Force World Enemy")]
    public bool ForceWorldEnemy
    {
        get => ApplyForceOn.HasFlag(Tag.WorldEnemy);
        set => ApplyForceOn = value ? ApplyForceOn.Add(Tag.WorldEnemy) : ApplyForceOn.Remove(Tag.WorldEnemy);
    }

    [SerializeInEditor(name: "Shake Camera")]
    public bool ShakeCamera
    {
        get => Effect.ShakeCamera;
        set => Effect.ShakeCamera = value;
    }

    [SerializeInEditor(name: "Delay")]
    public float Delay;

    [SignalReceiver]
    public void StartExplosion()
    {
        if (Effect.Started) return;
        TilesEffect?.StartExplosion(Delay);
        Effect.StartExplosion(Delay);
    }
}