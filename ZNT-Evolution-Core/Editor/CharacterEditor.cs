using System;
using System.Collections.Generic;
using UnityEngine;
using ZNT.Evolution.Core.Effect;

namespace ZNT.Evolution.Core.Editor;

[DisallowMultipleComponent]
public abstract class CharacterEditor : Editor
{
    [NonSerialized]
    protected Character Character;

    [NonSerialized]
    protected readonly Dictionary<string, CharacterBuff> Records = new();

    protected override void OnCreate()
    {
        Character ??= GetComponentInChildren<Character>();
    }

    public void ApplyBuff(Parameters parameters)
    {
        if (parameters == null || string.IsNullOrEmpty(parameters.Id)) return;
        var buff = ScriptableObject.CreateInstance<CharacterBuff>();
        buff.Load(parameters);
        if (Records.TryGetValue(buff.AssetId, out var prev))
        {
            if (prev.IsActive && prev.Diff >= buff.Diff) return;
            prev.Remove(Character);
        }

        buff.Apply(Character);
        Records[buff.AssetId] = buff;
    }

    protected virtual void Update()
    {
        foreach (var (_, buff) in Records)
        {
            if (buff.Repeat.IsComplete()) buff.Tick(Character);
            if (buff.Expire.IsComplete()) buff.Remove(Character);
        }
    }
}