using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "InvisibleOneWayWall")]
[DisallowMultipleComponent]
public class OneWayEditor : Editor, IActivable, IDeserializable
{
    private OneWayCollider _wall;

    private BoxCollider2D _collider;

    [SerializeInEditor(name: "Is Active")]
    public bool IsActive { get; private set; } = true;

    public void OnDeserialized()
    {
        // ...
    }

    public void OnGameObjectDeserialized()
    {
        SetActive(IsActive);
    }

    public void SetActive(bool state)
    {
        _collider ??= GetComponent<BoxCollider2D>();
        _collider.enabled = IsActive = state;
    }

    [SignalReceiver]
    public void SetActive() => SetActive(true);

    [SignalReceiver]
    public void SetInactive() => SetActive(false);

    [SignalReceiver]
    public void ToggleActivation() => SetActive(!IsActive);

    public void SetOrientation(Orientation orientation)
    {
        _wall ??= GetComponent<OneWayCollider>();
        Traverse.Create(_wall).Field<Orientation>("orientation").Value = orientation;
        _wall.SendMessage(methodName: "Start");
    }

    [SignalReceiver]
    public void TurnLeft() => SetOrientation(Orientation.Left);

    [SignalReceiver]
    public void TurnRight() => SetOrientation(Orientation.Right);

    [SignalReceiver]
    public void TurnUp() => SetOrientation(Orientation.Up);

    [SignalReceiver]
    public void TurnDown() => SetOrientation(Orientation.Down);
}