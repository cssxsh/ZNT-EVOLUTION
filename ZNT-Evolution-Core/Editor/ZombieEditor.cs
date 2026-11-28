using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Zombie")]
[DisallowMultipleComponent]
public class ZombieEditor : CharacterEditor
{
    private Tag Tags
    {
        get => gameObject.GetTags();
        set => gameObject.SetTags(value);
    }

    [SerializeInEditor(name: "Human Ignore")]
    public bool IgnoreAttack
    {
        get => Tags.HasFlag(Tag.IgnoreAttack);
        set => Tags = value ? Tags.Add(Tag.IgnoreAttack) : Tags.Remove(Tag.IgnoreAttack);
    }

    private void OnDespawned()
    {
        ResetBuff();
        ResetEnemy();
    }

    [SignalReceiver]
    public override void ApplyEnemy()
    {
        Character.AnimationController.SetSpriteColor(Color.red);
        Character.Behaviour.Vision.Layers = LayerMask.GetMask("Zombie");
        Character.Behaviour.Vision.WithTags = Tag.Zombie;
        Character.Behaviour.Vision.Update();
        Character.Behaviour.Attacker.AttackTrigger.Layers = LayerMask.GetMask("Zombie");
        Character.Behaviour.Attacker.AttackTrigger.WithTags = Tag.Zombie;
        Character.Behaviour.Attacker.RemoveTarget();
        Character.Behaviour.Attacker.RemoveVisibleTarget();
        Character.Behaviour.Attacker.Idle();
        Character.Behaviour.EnemyTag = Tag.Zombie;
        Character.Behaviour.SetIdle();
        Character.Behaviour.InvertOrientation();
        Character.gameObject.layer = LayerMask.NameToLayer("Human");
        Character.gameObject.SetTags(Tag.Character | Tag.Human);
    }

    [SignalReceiver]
    public override void ResetEnemy()
    {
        Character.AnimationController.SetSpriteColor(Color.white);
        Character.Behaviour.Vision.Layers = LayerMask.GetMask("Human", "Prop", "World Enemy");
        Character.Behaviour.Vision.WithTags = Tag.Door | Tag.Human | Tag.Breakable | Tag.WorldEnemy;
        Character.Behaviour.Attacker.AttackTrigger.Layers = LayerMask.GetMask("Human", "Prop", "World Enemy");
        Character.Behaviour.Attacker.AttackTrigger.WithTags = Tag.Human | Tag.Breakable | Tag.WorldEnemy;
        Character.Behaviour.EnemyTag = Tag.Human | Tag.WorldEnemy;
        Character.gameObject.layer = LayerMask.NameToLayer("Zombie");
        Character.gameObject.SetTags(Tag.Character | Tag.Zombie);
    }
}