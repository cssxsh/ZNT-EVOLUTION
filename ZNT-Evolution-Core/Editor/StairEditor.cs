using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Stair")]
[DisallowMultipleComponent]
public class StairEditor : Editor, IActivable
{
    [JsonIgnore]
    protected StairBehaviour Behaviour => field ??= GetComponentInChildren<StairBehaviour>();

    [JsonIgnore]
    public bool IsActive => Behaviour.UseStairs;

    public void SetActive(bool state)
    {
        if (IsActive == state) return;
        Behaviour.Invoke(methodName: "OnMouseUpAsButton", time: 0);
    }

    [SignalReceiver(name: "Set Stair Active")]
    public void SetActive() => SetActive(true);

    [SignalReceiver(name: "Set Stair Inactive")]
    public void SetInactive() => SetActive(false);

    [SignalReceiver(name: "Toggle Stair Activation")]
    public void ToggleActivation() => SetActive(!IsActive);
}