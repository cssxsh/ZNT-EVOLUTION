using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Invisible One Way Wall")]
[DisallowMultipleComponent]
public class OneWayEditor : Editor, IActivable, IDeserializable
{
    private static readonly Dictionary<Collider2D, OneWayCollider> Cache = new();

    private OneWayCollider Wall => field ??= GetComponent<OneWayCollider>();

    [SerializeInEditor(name: "Type")]
    public WallType Type
    {
        get => Wall.Type;
        set => Wall.Type = value;
    }

    [SerializeInEditor(name: "Block From")]
    public Orientation Orientation
    {
        get => Wall.Orientation;
        set
        {
            switch (value)
            {
                case Orientation.Left:
                    Wall.Effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                    Wall.Effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, Vector2.left);
                    Wall.SetDirection(Vector2.left);
                    break;
                case Orientation.Right:
                    Wall.Effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                    Wall.Effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, Vector2.right);
                    Wall.SetDirection(Vector2.right);
                    break;
                case Orientation.Up:
                    Wall.Effector.gameObject.layer = LayerMask.NameToLayer("Stairs Top");
                    Wall.Effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, Vector2.up);
                    Wall.SetDirection(Vector2.up);
                    break;
                case Orientation.Down:
                    Wall.Effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                    Wall.Effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, Vector2.down);
                    Wall.SetDirection(Vector2.down);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Orientation), value, null);
            }

            Wall.Orientation = value;
        }
    }

    [SerializeInEditor(name: "Is Active")]
    public bool IsActive { get; private set; } = true;

    protected override void OnCreate()
    {
        Wall.SetVisible(false);
    }

    private void OnEnable()
    {
        Cache[Wall.Collider] = Wall;
    }

    private void OnDisable()
    {
        Cache.Remove(Wall.Collider);
    }

    private IEnumerator Start()
    {
        yield return Wait.ForEndOfFrame;
        Orientation = Orientation;
    }

    public static bool TryGetOneWay(Collider2D key, out OneWayCollider value) => Cache.TryGetValue(key, out value);

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
        Wall.Collider.enabled = IsActive = state;
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