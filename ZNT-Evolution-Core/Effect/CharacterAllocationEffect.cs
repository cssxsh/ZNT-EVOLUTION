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

    private class Context : Dictionary<GameObject, CharacterAllocationEffect>
    {
        public void Remove(CharacterAllocationEffect effect)
        {
            foreach (var target in effect._cache) Remove(target);
        }
    }

    private static Dictionary<CharacterType, Context> _stopper;

    private static Dictionary<CharacterType, Context> _attacker;

    private Context FetchContext()
    {
        switch (name)
        {
            case nameof(Character.Components.Stopper):
                character ??= GetComponentInParent<Character>();
                _stopper ??= new Dictionary<CharacterType, Context>();
                if (_stopper.TryGetValue(character.CharacterType, out var stop)) return stop;
                return _stopper[character.CharacterType] = new Context();
            case nameof(Character.Components.Attacker):
                character ??= GetComponentInParent<Character>();
                _attacker ??= new Dictionary<CharacterType, Context>();
                if (_attacker.TryGetValue(character.CharacterType, out var attack)) return attack;
                return _attacker[character.CharacterType] = new Context();
            default:
                return null;
        }
    }

    private Context _allocated;

    private readonly C5.HashedArrayList<GameObject> _cache = new();

    public int capacity = 114514;

    private int Spare => capacity - _cache.Count;

    protected override void OnCreate()
    {
        var trigger = GetComponent<Trigger>();
        if (trigger.Effects.Contains(this)) return;
        Traverse.Create(trigger).Field<TriggerEffect[]>("effects").Value = null;
        _ = trigger.Effects;
    }

    public override void OnStartEffect()
    {
        _allocated = FetchContext();
        _cache.Clear();
    }

    public override void OnApplyEffect()
    {
        if (_allocated == null) return;
        _allocated.Remove(this);
        _cache.Clear();
        _cache.AddAll(DetectedGameObjects);
    }

    public override void OnApplyOnGameObject(GameObject target)
    {
        if (_allocated == null) return;
        if (_allocated.TryGetValue(target, out var other))
        {
            if (other.Spare >= Spare)
            {
                _cache.Remove(target);
                return;
            }

            other.DetectedGameObjects.Remove(target);
            other._cache.Remove(target);
        }

        _allocated[target] = this;
    }

    public override void OnEffectApplied()
    {
        if (_allocated == null) return;
        DetectedGameObjects.Clear();
        DetectedGameObjects.AddAll(_cache);
    }

    public override void OnEffectDone()
    {
        if (_allocated == null) return;
        _allocated.Remove(this);
        _cache.Clear();
        _allocated = null;
    }
}