using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Effect;

[DisallowMultipleComponent]
public class CharacterAllocationEffect : TriggerEffect
{
    [SerializeField]
    private Character character;

    private static Dictionary<CharacterType, Dictionary<GameObject, CharacterAllocationEffect>> _stopper;

    private Dictionary<GameObject, CharacterAllocationEffect> Context()
    {
        switch (name)
        {
            case nameof(Character.Components.Stopper):
                _stopper ??= new Dictionary<CharacterType, Dictionary<GameObject, CharacterAllocationEffect>>();
                if (_stopper.TryGetValue(character.CharacterType, out var result)) return result;
                return _stopper[character.CharacterType] = new Dictionary<GameObject, CharacterAllocationEffect>();
            default:
                return new Dictionary<GameObject, CharacterAllocationEffect>();
        }
    }

    private Dictionary<GameObject, CharacterAllocationEffect> _allocated;

    private readonly C5.HashedArrayList<GameObject> _cache = new();

    public int capacity = 114514;

    private int Spare => capacity - DetectedGameObjects.Count;

    protected override void OnCreate()
    {
        var trigger = GetComponent<Trigger>();
        if (trigger.Effects.Contains(this)) return;
        Traverse.Create(trigger).Field<TriggerEffect[]>("effects").Value = null;
        _ = trigger.Effects;
    }

    public override void OnStartEffect()
    {
        character ??= GetComponentInParent<Character>();
        _allocated = Context();
        _cache.Clear();
    }

    public override void OnApplyEffect()
    {
        if (_allocated == null) return;
        foreach (var target in _cache) _allocated.Remove(target);
        _cache.Clear();
    }

    public override void OnApplyOnGameObject(GameObject target)
    {
        if (_allocated == null) return;
        if (_allocated.TryGetValue(target, out var other))
        {
            if (other.Spare >= Spare + _cache.Count)
            {
                _cache.Add(target);
                return;
            }

            other.DetectedGameObjects.Remove(target);
            other._cache.Remove(target);
        }

        _allocated[target] = this;
    }

    public override void OnEffectApplied()
    {
        if (DetectedGameObjects == null) return;
        DetectedGameObjects.RemoveAll(_cache);
        _cache.Clear();
        _cache.AddAll(DetectedGameObjects);
    }

    public override void OnEffectDone()
    {
        if (_allocated == null) return;
        foreach (var target in _cache) _allocated.Remove(target);
        _cache.Clear();
        _allocated = null;
    }
}