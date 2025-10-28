using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "InvisibleOneWayWall")]
[DisallowMultipleComponent]
public class OneWayEditor : Editor, IActivable, IDeserializable
{
    private OneWayCollider _wall;

    private BoxCollider2D _collider;

    private PlatformEffector2D _effector;

    [SerializeInEditor(name: "Type")]
    public WallType Type
    {
        get => Traverse.Create(_wall).Property<WallType>("Type").Value;
        set => Traverse.Create(_wall).Property<WallType>("Type").Value = value;
    }

    [SerializeInEditor(name: "Block From")]
    public Orientation Orientation
    {
        get => Traverse.Create(_wall).Field<Orientation>("orientation").Value;
        set
        {
            _wall ??= GetComponent<OneWayCollider>();
            _effector ??= Traverse.Create(_wall).Field<PlatformEffector2D>("effector").Value;
            switch (value)
            {
                case Orientation.Left:
                    _effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                    _effector.rotationalOffset = Vector2.Angle(Vector2.up, Vector2.left);
                    Traverse.Create(_wall).Field<Vector2>("direction").Value = Vector2.left;
                    break;
                case Orientation.Right:
                    _effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                    _effector.rotationalOffset = Vector2.Angle(Vector2.up, Vector2.right);
                    Traverse.Create(_wall).Field<Vector2>("direction").Value = Vector2.right;
                    break;
                case Orientation.Up:
                    _effector.gameObject.layer = LayerMask.NameToLayer(IsWalkable ? "Stairs Top" : "One Way");
                    _effector.rotationalOffset = Vector2.Angle(Vector2.up, Vector2.up);
                    Traverse.Create(_wall).Field<Vector2>("direction").Value = Vector2.up;
                    break;
                case Orientation.Down:
                    _effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                    _effector.rotationalOffset = Vector2.Angle(Vector2.up, Vector2.down);
                    Traverse.Create(_wall).Field<Vector2>("direction").Value = Vector2.down;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Orientation), value, null);
            }

            Traverse.Create(_wall).Field<Orientation>("orientation").Value = value;
        }
    }

    [SerializeInEditor(name: "Is Active")]
    public bool IsActive { get; private set; } = true;

    [SerializeInEditor(name: "Is Walkable")]
    public bool IsWalkable { get; private set; } = true;

    protected override void OnCreate()
    {
        _wall ??= GetComponent<OneWayCollider>();
        _wall.EditorVisibility = false;
    }

    private IEnumerator Start()
    {
        yield return Wait.ForEndOfFrame;
        Orientation = Orientation;
    }

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
        _wall ??= GetComponent<OneWayCollider>();
        _collider ??= Traverse.Create(_wall).Field<BoxCollider2D>("collider").Value;
        _collider.enabled = IsActive = state;
    }

    [SignalReceiver]
    public void SetActive() => SetActive(true);

    [SignalReceiver]
    public void SetInactive() => SetActive(false);

    [SignalReceiver]
    public void ToggleActivation() => SetActive(!IsActive);

    [SignalReceiver]
    public void TurnLeft() => Orientation = Orientation.Left;

    [SignalReceiver]
    public void TurnRight() => Orientation = Orientation.Right;

    [SignalReceiver]
    public void TurnUp() => Orientation = Orientation.Up;

    [SignalReceiver]
    public void TurnDown() => Orientation = Orientation.Down;
}