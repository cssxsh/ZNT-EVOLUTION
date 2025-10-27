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
        set => Traverse.Create(_wall).Field<Orientation>("orientation").Value = value;
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
        SetOrientation(Orientation);
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

    public void SetOrientation(Orientation orientation)
    {
        _wall ??= GetComponent<OneWayCollider>();
        _collider ??= Traverse.Create(_wall).Field<BoxCollider2D>("collider").Value;
        _effector ??= Traverse.Create(_wall).Field<PlatformEffector2D>("effector").Value;
        var size = Orientation switch
        {
            Orientation.Left or Orientation.Right => new Vector2(_collider.size.y, _collider.size.x),
            Orientation.Up or Orientation.Down => _collider.size,
            _ => throw new ArgumentOutOfRangeException(nameof(Orientation), Orientation, null)
        };
        switch (Orientation = orientation)
        {
            case Orientation.Left:
                _effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                _effector.transform.up = Vector3.left;
                _collider.size = new Vector2(size.y, size.x);
                Traverse.Create(_wall).Field<Vector2>("direction").Value = Vector2.left;
                break;
            case Orientation.Right:
                _effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                _effector.transform.up = Vector3.right;
                _collider.size = new Vector2(size.y, size.x);
                Traverse.Create(_wall).Field<Vector2>("direction").Value = Vector2.right;
                break;
            case Orientation.Up:
                _effector.gameObject.layer = LayerMask.NameToLayer(IsWalkable ? "Stairs Top" : "One Way");
                _effector.transform.up = Vector3.up;
                _collider.size = size;
                Traverse.Create(_wall).Field<Vector2>("direction").Value = Vector2.up;
                break;
            case Orientation.Down:
                _effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                _effector.transform.up = Vector3.down;
                _collider.size = size;
                Traverse.Create(_wall).Field<Vector2>("direction").Value = Vector2.down;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
        }
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