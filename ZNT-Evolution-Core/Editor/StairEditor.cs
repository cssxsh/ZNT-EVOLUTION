using System;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Stair")]
[DisallowMultipleComponent]
public class StairEditor : Editor, IActivable
{
    [field: NonSerialized]
    protected StairBehaviour Behaviour => field ??= GetComponent<StairBehaviour>();

    public bool IsActive => Behaviour.UseStairs;

    public void SetActive(bool state)
    {
        if (IsActive == state) return;
        Behaviour.SendMessage(methodName: "OnMouseUpAsButton");
    }

    [SignalReceiver(name: "Set Stair Active")]
    public void SetActive() => SetActive(true);

    [SignalReceiver(name: "Set Stair Inactive")]
    public void SetInactive() => SetActive(false);

    [SignalReceiver(name: "Toggle Stair Activation")]
    public void ToggleActivation() => SetActive(!IsActive);
}