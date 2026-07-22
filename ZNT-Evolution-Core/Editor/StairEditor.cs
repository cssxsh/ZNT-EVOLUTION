using System;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Stair")]
[DisallowMultipleComponent]
public class StairEditor : Editor, IActivable
{
    [NonSerialized]
    protected StairBehaviour Behaviour;

    public bool IsActive => (Behaviour ??= GetComponent<StairBehaviour>()).UseStairs;

    public void SetActive(bool state)
    {
        if (IsActive == state) return;
        (Behaviour ??= GetComponent<StairBehaviour>()).SendMessage(methodName: "OnMouseUpAsButton");
    }

    [SignalReceiver(name: "Set Stair Active")]
    public void SetActive() => SetActive(true);

    [SignalReceiver(name: "Set Stair Inactive")]
    public void SetInactive() => SetActive(false);

    [SignalReceiver(name: "Toggle Stair Activation")]
    public void ToggleActivation() => SetActive(!IsActive);
}