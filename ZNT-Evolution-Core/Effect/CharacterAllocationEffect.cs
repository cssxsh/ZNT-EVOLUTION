using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Effect;

[DisallowMultipleComponent]
public class CharacterAllocationEffect : TriggerEffect
{
    [JsonIgnore]
    private Character Character => field ??= GetComponentInParent<Character>();

    private class Context : Dictionary<GameObject, CharacterAllocationEffect>
    {
        public void Remove(CharacterAllocationEffect effect)
        {
            foreach (var target in effect._cache) Remove(target);
        }
    }

    private static readonly Dictionary<CharacterType, Context> Stopper = new();

    private static readonly Dictionary<CharacterType, Context> Vision = new();

    private Context FetchContext()
    {
        switch (name)
        {
            case nameof(Character.Components.Stopper):
                if (Stopper.TryGetValue(Character.CharacterType, out var stop)) return stop;
                return Stopper[Character.CharacterType] = new Context();
            case nameof(Character.Components.Vision):
                if (Vision.TryGetValue(Character.CharacterType, out var vision)) return vision;
                return Vision[Character.CharacterType] = new Context();
            default:
                return null;
        }
    }

    [JsonIgnore]
    private Context _allocated;

    [JsonIgnore]
    private readonly C5.HashedArrayList<GameObject> _cache = new();

    [JsonIgnore]
    public int capacity = 114514;

    private int Spare => capacity - _cache.Count;

    public override void OnStartEffect()
    {
        _allocated = FetchContext();
        _cache.Clear();
    }

    public override void OnApplyEffect()
    {
        if (_allocated is null) return;
        _allocated.Remove(this);
        _cache.Clear();
        _cache.AddAll(DetectedGameObjects);
    }

    public override void OnApplyOnGameObject(GameObject target)
    {
        if (_allocated is null) return;
        if (_allocated.TryGetValue(target, out var other))
        {
            if (other.Spare >= Spare)
            {
                _cache.Remove(target);
            }
            else
            {
                other.DetectedGameObjects.Remove(target);
                other._cache.Remove(target);
            }
        }

        _allocated[target] = this;
    }

    public override void OnEffectApplied()
    {
        if (_allocated is null) return;
        DetectedGameObjects.Clear();
        DetectedGameObjects.AddAll(_cache);
    }

    public override void OnEffectDone()
    {
        if (_allocated is null) return;
        _allocated.Remove(this);
        _cache.Clear();
        _allocated = null;
    }

    private void OnDisable() => OnEffectDone();
}