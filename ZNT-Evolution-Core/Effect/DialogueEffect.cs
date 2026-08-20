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
                Signal.Event<GameObjectEvent>("OnDetected").RemoveListener(OnApplyOnGameObject);
                Signal.Event<GameObjectEvent>("OnDetectedExit").RemoveListener(OnApplyOnGameObject);
                break;
            case DetectionMode.SignalOnEnter:
                Signal.Event<GameObjectEvent>("OnDetected").AddListener(OnApplyOnGameObject);
                Signal.Event<GameObjectEvent>("OnDetectedExit").RemoveListener(OnApplyOnGameObject);
                break;
            case DetectionMode.SignalOnExit:
                Signal.Event<GameObjectEvent>("OnDetected").RemoveListener(OnApplyOnGameObject);
                Signal.Event<GameObjectEvent>("OnDetectedExit").AddListener(OnApplyOnGameObject);
                break;
        }
    }

    public override bool CheckConditions(GameObject target) => Mode is DetectionMode.Normal;

    public override void OnApplyOnGameObject(GameObject target)
    {
        if (Duration <= 0 || Text.Content is null or "") return;
        var behaviour = target?.GetComponent<CharacterBehaviour>();
        if (behaviour is null) return;
        if (behaviour.IsTalking() && !ForceShow) return;
        behaviour.Dialogue(Text, Duration, Voice);
    }

    public enum DetectionMode
    {
        Normal,
        SignalOnEnter,
        SignalOnExit
    }
}