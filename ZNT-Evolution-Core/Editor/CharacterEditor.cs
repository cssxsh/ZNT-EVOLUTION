using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using ZNT.Evolution.Core.Effect;

namespace ZNT.Evolution.Core.Editor;

[DisallowMultipleComponent]
public abstract class CharacterEditor : Editor
{
    protected Character Character => field ??= GetComponentInChildren<Character>();

    [UsedImplicitly]
    protected readonly Dictionary<string, CharacterBuff> Buffs = new();

    public virtual void ApplyBuff(Parameters parameters)
    {
        if (parameters?.Id is null or "") return;
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

    public virtual void ResetBuff()
    {
        foreach (var (_, buff) in Buffs) buff.Remove(Character);
        Buffs.Clear();
    }

    public virtual void ApplyEnemy()
    {
        // ...
    }

    public virtual void ResetEnemy()
    {
        // ...
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