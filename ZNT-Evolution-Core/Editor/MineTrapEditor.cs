using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Mine")]
[DisallowMultipleComponent]
public class MineTrapEditor : Editor
{
    private Trigger Trigger => GetComponent<Trigger>();

    [SerializeInEditor(name: "Detected Human")]
    public bool DetectedHuman
    {
        get => Trigger.WithTags.HasFlag(Tag.Human);
        set => Trigger.WithTags = value ? Trigger.WithTags.Add(Tag.Human) : Trigger.WithTags.Remove(Tag.Human);
    }

    [SerializeInEditor(name: "Detected Zombie")]
    public bool DetectedZombie
    {
        get => Trigger.WithTags.HasFlag(Tag.Zombie);
        set => Trigger.WithTags = value ? Trigger.WithTags.Add(Tag.Zombie) : Trigger.WithTags.Remove(Tag.Zombie);
    }

    private MineBehaviour Behaviour => GetComponent<MineBehaviour>();

    [SerializeInEditor(name: "Delay")]
    public float Delay
    {
        get => Traverse.Create(Behaviour).Field<float>("Delay").Value;
        set => Traverse.Create(Behaviour).Field<float>("Delay").Value = value;
    }
}