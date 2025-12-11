using HarmonyLib;
using UnityEngine.Events;

namespace ZNT.Evolution.Core.Effect;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CharacterBuff : CustomAsset
{
    public Parameters Origin;

    // ReSharper disable InconsistentNaming
    public Timer Repeat;
    public Timer Expire;
    public float Diff;
    public float DeltaTime;
    // ReSharper restore InconsistentNaming

    // ReSharper disable MemberCanBePrivate.Global
    public UnityAction<Character, CustomAsset> OnApply;
    public UnityAction<Character, CustomAsset> OnRemove;
    public UnityAction<Character, CustomAsset> OnTick;
    // ReSharper restore MemberCanBePrivate.Global

    public bool IsActive => Expire.Timespan > DeltaTime;

    public virtual void Load(Parameters parameters)
    {
        Traverse.Create(this).Field<string>("assetId").Value = parameters.Id;
        Repeat = parameters.GetValue<Timer>("repeat");
        Expire = parameters.GetValue<Timer>("expire");
        Diff = parameters.GetValue<float>("diff");
        DeltaTime = parameters.GetValue<float>("delta");
        OnApply = parameters.GetValue<UnityAction<Character, CustomAsset>>("apply");
        OnRemove = parameters.GetValue<UnityAction<Character, CustomAsset>>("remove");
        OnTick = parameters.GetValue<UnityAction<Character, CustomAsset>>("tick");
        Origin = parameters;
    }

    public void Apply(Character target)
    {
        OnApply?.Invoke(target, this);
        if (float.IsFinite(Repeat.Duration)) Repeat.Start();
        if (float.IsFinite(Expire.Duration)) Expire.Start();
    }

    public void Remove(Character target)
    {
        OnRemove?.Invoke(target, this);
        OnTick = null;
        OnRemove = null;
    }

    public void Tick(Character target)
    {
        OnTick?.Invoke(target, this);
    }
}