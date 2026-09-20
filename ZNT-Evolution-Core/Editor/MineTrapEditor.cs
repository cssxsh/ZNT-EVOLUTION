using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Mine")]
[DisallowMultipleComponent]
public class MineTrapEditor : Editor
{
    private MineBehaviour Behaviour => field ??= GetComponent<MineBehaviour>();

    private Tag DetectedTags
    {
        get => Behaviour.Trigger.WithTags;
        set => Behaviour.Trigger.WithTags = value;
    }

    [SerializeInEditor(name: "Detected Human")]
    public bool DetectedHuman
    {
        get => DetectedTags.HasFlag(Tag.Human);
        set => DetectedTags = value ? DetectedTags.Add(Tag.Human) : DetectedTags.Remove(Tag.Human);
    }

    [SerializeInEditor(name: "Detected Zombie")]
    public bool DetectedZombie
    {
        get => DetectedTags.HasFlag(Tag.Zombie);
        set => DetectedTags = value ? DetectedTags.Add(Tag.Zombie) : DetectedTags.Remove(Tag.Zombie);
    }

    [SerializeInEditor(name: "Detected World Enemy")]
    public bool DetectedWorldEnemy
    {
        get => DetectedTags.HasFlag(Tag.WorldEnemy);
        set => DetectedTags = value ? DetectedTags.Add(Tag.WorldEnemy) : DetectedTags.Remove(Tag.WorldEnemy);
    }

    [SerializeInEditor(name: "Delay")]
    public float Delay
    {
        get => Behaviour.Delay;
        set => Behaviour.Delay = value;
    }
}