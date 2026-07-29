using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Invisible One Way Wall")]
[DisallowMultipleComponent]
public class OneWayEditor : Editor, IActivable, IDeserializable
{
    private static readonly Dictionary<Collider2D, OneWayCollider> Cache = new();

    [JsonIgnore]
    private OneWayCollider Wall => field ??= GetComponent<OneWayCollider>();

    [JsonIgnore]
    private BoxCollider2D Collider => field ??= Traverse.Create(Wall).Field<BoxCollider2D>("collider").Value;

    [JsonIgnore]
    private PlatformEffector2D Effector => field ??= Traverse.Create(Wall).Field<PlatformEffector2D>("effector").Value;

    [JsonIgnore]
    [SerializeInEditor(name: "Type")]
    public WallType Type
    {
        get => Traverse.Create(Wall).Property<WallType>("Type").Value;
        set => Traverse.Create(Wall).Property<WallType>("Type").Value = value;
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Block From")]
    public Orientation Orientation
    {
        get => Traverse.Create(Wall).Field<Orientation>("orientation").Value;
        set
        {
            switch (value)
            {
                case Orientation.Left:
                    Effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                    Effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, Vector2.left);
                    Traverse.Create(Wall).Field<Vector2>("direction").Value = Vector2.left;
                    break;
                case Orientation.Right:
                    Effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                    Effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, Vector2.right);
                    Traverse.Create(Wall).Field<Vector2>("direction").Value = Vector2.right;
                    break;
                case Orientation.Up:
                    Effector.gameObject.layer = LayerMask.NameToLayer("Stairs Top");
                    Effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, Vector2.up);
                    Traverse.Create(Wall).Field<Vector2>("direction").Value = Vector2.up;
                    break;
                case Orientation.Down:
                    Effector.gameObject.layer = LayerMask.NameToLayer("One Way");
                    Effector.rotationalOffset = Vector2.SignedAngle(Vector2.up, Vector2.down);
                    Traverse.Create(Wall).Field<Vector2>("direction").Value = Vector2.down;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Orientation), value, null);
            }

            Traverse.Create(Wall).Field<Orientation>("orientation").Value = value;
        }
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Is Active")]
    public bool IsActive { get; private set; } = true;

    protected override void OnCreate()
    {
        Wall.SetVisible(false);
    }

    private void OnEnable()
    {
        Cache[Collider] = Wall;
    }

    private void OnDisable()
    {
        Cache.Remove(Collider);
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
        Collider.enabled = IsActive = state;
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