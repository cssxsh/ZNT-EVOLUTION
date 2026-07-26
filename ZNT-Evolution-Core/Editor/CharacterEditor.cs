using System;
using System.Collections.Generic;
using UnityEngine;
using ZNT.Evolution.Core.Effect;

namespace ZNT.Evolution.Core.Editor;

[DisallowMultipleComponent]
public abstract class CharacterEditor : Editor
{
    [field: NonSerialized]
    protected Character Character => field ??= GetComponentInChildren<Character>();

    [NonSerialized]
    protected readonly Dictionary<string, CharacterBuff> Buffs = new();

    public void ApplyBuff(Parameters parameters)
    {
        if (parameters == null || string.IsNullOrEmpty(parameters.Id)) return;
        var buff = ScriptableObject.CreateInstance<CharacterBuff>();
        buff.Load(parameters);
        if (Buffs.TryGetValue(buff.AssetId, out var prev))
        {
            if (prev.IsActive && prev.Diff >= buff.Diff) return;
            prev.Remove(Character);
        }

        buff.Apply(Character);
        Buffs[buff.AssetId] = buff;
    }

    protected virtual void Update()
    {
        foreach (var (_, buff) in Buffs)
        {
            if (buff.Repeat.IsComplete()) buff.Tick(Character);
            if (buff.Expire.IsComplete()) buff.Remove(Character);
        }
    }
}