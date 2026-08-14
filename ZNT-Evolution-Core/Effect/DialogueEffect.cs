using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using ZNT.Evolution.Core.Editor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Effect;

[SerializeInEditor(name: "Dialogue")]
[DisallowMultipleComponent]
public class DialogueEffect : TriggerEffect
{
    [JsonIgnore]
    private SignalEffect Signal => field ??= GetComponent<SignalEffect>();

    [JsonIgnore]
    private GameObjectEvent SignalOnDetected =>
        Traverse.Create(Signal).Field("events").Field<GameObjectEvent>("OnDetected").Value;

    [JsonIgnore]
    private GameObjectEvent SignalOnOnDetectedExit =>
        Traverse.Create(Signal).Field("events").Field<GameObjectEvent>("OnDetectedExit").Value;

    [SerializeInEditor(name: "Mode")]
    public DetectionMode Mode = DetectionMode.Normal;

    [SerializeInEditor(name: "Force Show")]
    public bool ForceShow;

    [SerializeInEditor(name: "Text")]
    public LocalizableString Text = new() { Localize = false, Category = "Dialogues" };

    [SerializeInEditor(name: "Duration")]
    public float Duration = 10;

    [SerializeInEditor(name: "Voice")]
    public Voice Voice = Voice.None;

    public override void OnStartEffect()
    {
        if (Signal is null) return;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (Mode)
        {
            case DetectionMode.Normal:
                SignalOnDetected.RemoveListener(OnApplyOnGameObject);
                SignalOnOnDetectedExit.RemoveListener(OnApplyOnGameObject);
                break;
            case DetectionMode.SignalOnEnter:
                SignalOnDetected.AddListener(OnApplyOnGameObject);
                SignalOnOnDetectedExit.RemoveListener(OnApplyOnGameObject);
                break;
            case DetectionMode.SignalOnExit:
                SignalOnDetected.RemoveListener(OnApplyOnGameObject);
                SignalOnOnDetectedExit.AddListener(OnApplyOnGameObject);
                break;
        }
    }

    public override bool CheckConditions(GameObject target) => Mode is DetectionMode.Normal;

    public override void OnApplyOnGameObject(GameObject target)
    {
        if (Text.Content is null or "" || Duration <= 0) return;
        var human = target?.GetComponent<HumanBehaviour>();
        if (human is null) return;
        var patroller = human.Patroller;
        if (patroller.IsTalking() && !ForceShow) return;
        var dialogue = ComponentSingleton<GamePoolManager>.Instance
            .Spawn(nameof(Dialogue)).GetComponent<Dialogue>();
        dialogue.SetText(Text, Duration);
        dialogue.Show(patroller, patroller.DialogueOffset, Voice);
    }
    
    public enum DetectionMode
    {
        Normal,
        SignalOnEnter,
        SignalOnExit
    }
}