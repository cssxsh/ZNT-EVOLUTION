using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Stair")]
[DisallowMultipleComponent]
public class StairEditor : Editor, IActivable
{
    private StairBehaviour Behaviour => GetComponentInParent<StairBehaviour>();

    public bool IsActive => Behaviour.UseStairs;

    public void SetActive(bool state)
    {
        if (Behaviour.UseStairs == state) return;
        ToggleActivation();
    }

    [SignalReceiver(name: "Set Stair Active")]
    public void SetActive() => SetActive(true);

    [SignalReceiver(name: "Set Stair Inactive")]
    public void SetInactive() => SetActive(false);

    [SignalReceiver(name: "Toggle Stair Activation")]
    public void ToggleActivation() => Behaviour.SendMessage(methodName: "OnMouseUpAsButton");
}