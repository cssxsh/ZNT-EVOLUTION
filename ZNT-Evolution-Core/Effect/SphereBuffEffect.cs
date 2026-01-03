using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using ZNT.Evolution.Core.Editor;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Effect;

[SerializeInEditor(name: "Buff")]
[DisallowMultipleComponent]
public class SphereBuffEffect : TriggerEffect
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(SphereBuffEffect));

    [SerializeInEditor(name: "Detection Radius")]
    public float DetectionRadius
    {
        get => ((SphereDetection)Trigger.Detection).Radius;
        set => ((SphereDetection)Trigger.Detection).Radius = value;
    }

    [SerializeInEditor(name: "Detection Frequency", devOnly: true)]
    public float DetectionFrequency
    {
        get => Trigger.Frequency;
        set => Trigger.Frequency = value;
    }

    // ReSharper disable InconsistentNaming

    [SerializeInEditor(name: "Detection Type")]
    public CharacterType DetectionType = CharacterType.ArmedCivilian;

    [SerializeInEditor(name: "Max Hp Buff")]
    public bool MaxHp;

    [SerializeInEditor(name: "Max Hp Duration")]
    public float MaxHpDuration = 60.0f;

    [SerializeInEditor(name: "Max Hp Diff")]
    public float MaxHpDiff = 100.0f;

    private readonly Parameters MaxHpParameters = new(id: nameof(Health.MaxHp));

    [SerializeInEditor(name: "Hp Buff")]
    public bool Hp;

    [SerializeInEditor(name: "Hp Duration")]
    public float HpDuration = 600.0f;

    [SerializeInEditor(name: "Hp Diff")]
    public float HpDiff = 10.0f;

    private readonly Parameters HpParameters = new(id: nameof(Health.Hp));

    [SerializeInEditor(name: "Damage Buff")]
    public bool Damage;

    [SerializeInEditor(name: "Damage Duration")]
    public float DamageDuration = 60.0f;

    [SerializeInEditor(name: "Damage Diff")]
    public float DamageDiff = 100.0f;

    private readonly Parameters DamageParameters = new(id: nameof(Attacker.Damage));

    // ReSharper restore InconsistentNaming

    public override bool CheckConditions(GameObject target)
    {
        return target.TryGetComponent(out Character character) && character.CharacterType == DetectionType;
    }

    public override void OnApplyOnGameObject(GameObject target)
    {
        if (MaxHp)
        {
            target.SendMessage(
                methodName: nameof(HumanEditor.ApplyBuff),
                value: MaxHpParameters
                    .Update("repeat", new Timer { Duration = float.NaN, Loop = true })
                    .Update("expire", new Timer { Duration = MaxHpDuration, Loop = false })
                    .Update("diff", MaxHpDiff)
                    .Update("delta", 1.0f / Trigger.Frequency)
                    .Update("apply", new UnityAction<Character, CustomAsset>(MaxHpApply))
                    .Update("remove", new UnityAction<Character, CustomAsset>(MaxHpRemove))
                    .Update("tick", null),
                options: SendMessageOptions.DontRequireReceiver);
        }

        if (Hp)
        {
            target.SendMessage(
                methodName: nameof(HumanEditor.ApplyBuff),
                value: HpParameters
                    .Update("repeat", new Timer { Duration = 1.0f, Loop = true })
                    .Update("expire", new Timer { Duration = HpDuration, Loop = false })
                    .Update("diff", HpDiff)
                    .Update("delta", 1.0f / Trigger.Frequency)
                    .Update("apply", null)
                    .Update("remove", null)
                    .Update("tick", new UnityAction<Character, CustomAsset>(HpTick)),
                options: SendMessageOptions.DontRequireReceiver);
        }

        if (Damage)
        {
            target.SendMessage(
                methodName: nameof(HumanEditor.ApplyBuff),
                value: DamageParameters
                    .Update("repeat", new Timer { Duration = float.NaN, Loop = true })
                    .Update("expire", new Timer { Duration = DamageDuration, Loop = false })
                    .Update("diff", DamageDiff)
                    .Update("delta", 1.0f / Trigger.Frequency)
                    .Update("apply", new UnityAction<Character, CustomAsset>(DamageApply))
                    .Update("remove", new UnityAction<Character, CustomAsset>(DamageRemove))
                    .Update("tick", null),
                options: SendMessageOptions.DontRequireReceiver);
        }
    }

    protected override bool OverrideExecutionMode(out Execution.Mode mode)
    {
        mode = Execution.Mode.Play | Execution.Mode.Preview;
        return true;
    }

    private void OnSpawned()
    {
        if (CanExecute) Trigger.StartManualRepeatTrigger();
    }

    private void OnDespawned()
    {
        if (CanExecute) Trigger.StopRepeatTrigger();
    }

    private static void MaxHpApply(Character target, CustomAsset asset)
    {
        if (asset is not CharacterBuff buff) return;
        if (buff.Diff == 0.0f) return;
        Logger.LogDebug($"{target} MaxHp += {buff.Diff}");
        target.Behaviour.Health.MaxHp += buff.Diff;
    }

    private static void MaxHpRemove(Character target, CustomAsset asset)
    {
        if (asset is not CharacterBuff buff) return;
        if (buff.Diff == 0.0f) return;
        Logger.LogDebug($"{target} MaxHp -= {buff.Diff}");
        target.Behaviour.Health.MaxHp -= buff.Diff;
    }

    private static void HpTick(Character target, CustomAsset asset)
    {
        if (asset is not CharacterBuff buff) return;
        if (buff.Diff == 0.0f || target.Behaviour.Health.Hp >= target.Behaviour.Health.MaxHp) return;
        Logger.LogDebug($"{target} Hp += {buff.Diff}");
        target.Behaviour.Health.Hp += buff.Diff;
    }

    private static void DamageApply(Character target, CustomAsset asset)
    {
        if (asset is not CharacterBuff buff) return;
        if (buff.Diff == 0.0f) return;
        Logger.LogDebug($"{target} Damage += {buff.Diff}");
        target.Behaviour.Attacker.Damage += buff.Diff;
    }

    private static void DamageRemove(Character target, CustomAsset asset)
    {
        if (asset is not CharacterBuff buff) return;
        if (buff.Diff == 0.0f) return;
        Logger.LogDebug($"{target} Damage -= {buff.Diff}");
        target.Behaviour.Attacker.Damage -= buff.Diff;
    }

    private static PoolSettingsAsset.PoolPrefab _prefab;

    // ReSharper disable Unity.PerformanceAnalysis
    public static PoolSettingsAsset.PoolPrefab PoolPrefab()
    {
        if (_prefab != null) return _prefab;
        var prefab = new GameObject(name: nameof(SphereBuffEffect));
        DontDestroyOnLoad(prefab);
        prefab.SetActive(false);
        var buff = prefab.AddComponent<SphereBuffEffect>();
        var sphere = prefab.AddComponent<SphereDetection>();
        var trigger = prefab.AddComponent<Trigger>();
        buff.PoolIndex = 0x1748_0001;
        buff.EditorVisibility = true;
        sphere.PoolIndex = 0x1748_0002;
        sphere.ObtsaclesLayers = LayerMask.GetMask("Gameplay", "Stairs Top", "Spit", "Block Humans");
        sphere.ObtsaclesTags = Tag.Indestructible | Tag.Destructible;
        sphere.Radius = 1748;
        trigger.PoolIndex = 0x1748_0003;
        trigger.Frequency = 10;
        trigger.Layers = LayerMask.GetMask("Human");
        Traverse.Create(trigger).Field<TriggerType>("Type").Value = TriggerType.ManualActivation;
        prefab.SetActive(true);
        // ReSharper disable once Unity.UnknownResource
        var pool = Resources.Load<PoolSettingsAsset>("Assets/GamePoolSettings");
        pool.Prefabs.Add(_prefab = new PoolSettingsAsset.PoolPrefab
        {
            Prefab = prefab.transform,
            Amount = 9
        });
        return _prefab;
    }
}