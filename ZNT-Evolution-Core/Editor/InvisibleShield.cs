using System;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "InvisibleShield")]
[RequireComponent(typeof(BoxCollider2D))]
[DisallowMultipleComponent]
public class InvisibleShield : Editor, IActivable, IDeserializable
{
    private BoxCollider2D _collider;

    [SerializeInEditor(name: "Is Active")]
    public bool IsActive { get; private set; } = true;

    [SerializeField]
    private WallType type = WallType.Explosion;

    [SerializeInEditor(name: "Type")]
    public WallType Type
    {
        get => type;
        set => gameObject.layer = (type = value) switch
        {
            WallType.Both => LayerMask.NameToLayer("Gameplay"),
            WallType.Human => LayerMask.NameToLayer("Block Humans"),
            WallType.Zombie => LayerMask.NameToLayer("Block Zombies"),
            WallType.Explosion => LayerMask.NameToLayer("Block Explosion"),
            _ => throw new ArgumentOutOfRangeException(nameof(WallType), value, null)
        };
    }

    [SerializeInEditor(name: "Direction")]
    public Vector2 Direction
    {
        get => (_collider ??= GetComponent<BoxCollider2D>()).offset;
        set => (_collider ??= GetComponent<BoxCollider2D>()).offset = value;
    }

    protected override void OnCreate()
    {
        Direction = Vector2.right;
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
        (_collider ??= GetComponent<BoxCollider2D>()).enabled = IsActive = state;
    }

    public void SetActive() => SetActive(true);

    public void SetInactive() => SetActive(false);

    public void ToggleActivation() => SetActive(!IsActive);

    private void OnSpawned()
    {
        transform.localPosition = new Vector2(0.0f, 1.0f);
    }

    private void OnDespawned()
    {
        Type = WallType.Explosion;
        Direction = Vector2.right;
    }

    private static PoolSettingsAsset.PoolPrefab _prefab;

    // ReSharper disable Unity.PerformanceAnalysis
    public static PoolSettingsAsset.PoolPrefab PoolPrefab()
    {
        if (_prefab != null) return _prefab;
        var prefab = new GameObject(name: nameof(InvisibleShield));
        prefab.AddComponent<BoxCollider2D>().size = new Vector2(0.3f, 1.95f);
        prefab.AddComponent<InvisibleShield>();
        prefab.AddTags(Tag.Indestructible);
        prefab.layer = LayerMask.NameToLayer("Block Explosion");
        DontDestroyOnLoad(prefab);
        // ReSharper disable once Unity.UnknownResource
        var pool = Resources.Load<PoolSettingsAsset>("Assets/GamePoolSettings");
        pool.Prefabs.Add(_prefab = new PoolSettingsAsset.PoolPrefab
        {
            Prefab = prefab.transform,
            Amount = 9
        });
        return _prefab;
    }
}