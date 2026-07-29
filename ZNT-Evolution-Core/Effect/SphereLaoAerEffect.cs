using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Effect;

[SerializeInEditor(name: "LaoAer")]
[DisallowMultipleComponent]
public class SphereLaoAerEffect : TriggerEffect
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(SphereLaoAerEffect));

    [JsonIgnore]
    [SerializeInEditor(name: "Detection Radius")]
    public float DetectionRadius
    {
        get => ((SphereDetection)Trigger.Detection).Radius;
        set => ((SphereDetection)Trigger.Detection).Radius = value;
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Detection Frequency", devOnly: true)]
    public float DetectionFrequency
    {
        get => Trigger.Frequency;
        set => Trigger.Frequency = value;
    }

    [JsonIgnore]
    private HumanBehaviour _human;

    [JsonIgnore]
    private CorpseBehaviour _nearest;

    public override void OnApplyOnGameObject(GameObject target)
    {
        _human ??= GetComponentInParent<HumanBehaviour>();
        if (_human is null) return;
        var corpse = target.GetComponent<CorpseBehaviour>();
        if (corpse is null) return;
        // var parameters = Traverse.Create(corpse).Field<CorpseParameter>("parameters").Value;
        // if (parameters.Rise) return;
        var corpses = Traverse.Create<CorpseBehaviour>().Field<Queue<CorpseBehaviour>>("aliveCorpses").Value;
        if (corpses.Contains(corpse)) return;
        if (_human.Attacker.IsInAttackRange(target.transform))
        {
            // var dialogue = ComponentSingleton<GamePoolManager>.Instance
            //     .Spawn(nameof(Dialogue)).GetComponent<Dialogue>();
            // dialogue.SetText(new LocalizableString { Localize = false, Content = $"高达 {_nearest.transform.position}" }, 5.0f);
            // dialogue.Show(_human.Patroller, _human.Patroller.DialogueOffset, Voice.None);
            // TODO Handle Corpse
            corpse.StopAllCoroutines();
            corpse.Dissolve();
        }
        else
        {
            if (_nearest is null ||
                Vector3.Distance(_human.transform.position, _nearest.transform.position) >
                Vector3.Distance(_human.transform.position, corpse.transform.position))
            {
                _nearest = corpse;
            }
        }
    }

    public override void OnEffectAppliedOnGameObjects()
    {
        if (_nearest is null) return;
        _human ??= GetComponentInParent<HumanBehaviour>();
        if (_human.State != BehaviourState.Idle) return;
        _human.SendMessage(methodName: "MoveToTarget", value: _nearest.transform);
    }

    protected override bool OverrideExecutionMode(out Execution.Mode mode)
    {
        mode = Execution.Mode.Play | Execution.Mode.Preview;
        return true;
    }

    private void OnSpawned()
    {
        if (CanExecute) Trigger.StartManualRepeatTrigger();
        _human = null;
    }

    private void OnDespawned()
    {
        if (CanExecute) Trigger.StopRepeatTrigger();
    }

    private static PoolSettingsAsset.PoolPrefab _prefab;

    // ReSharper disable Unity.PerformanceAnalysis
    public static PoolSettingsAsset.PoolPrefab PoolPrefab()
    {
        if (_prefab != null) return _prefab;
        var prefab = new GameObject(name: nameof(SphereLaoAerEffect));
        DontDestroyOnLoad(prefab);
        prefab.SetActive(false);
        var effect = prefab.AddComponent<SphereLaoAerEffect>();
        var sphere = prefab.AddComponent<SphereDetection>();
        var trigger = prefab.AddComponent<Trigger>();
        effect.PoolIndex = 0x1748_0101;
        effect.EditorVisibility = true;
        sphere.PoolIndex = 0x1748_0102;
        sphere.ObtsaclesLayers = LayerMask.GetMask("Gameplay", "Stairs Top", "Spit", "Block Humans");
        sphere.ObtsaclesTags = Tag.Indestructible | Tag.Destructible;
        sphere.Radius = 1748;
        trigger.PoolIndex = 0x1748_0103;
        trigger.Frequency = 10;
        trigger.Layers = LayerMask.GetMask("Ignore Characters");
        trigger.WithTags = Tag.Corpse;
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