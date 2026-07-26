using System;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "InvisibleShield")]
[RequireComponent(typeof(BoxCollider2D))]
[DisallowMultipleComponent]
public class InvisibleShield : Editor, IActivable, IDeserializable
{
    [field: NonSerialized]
    private BoxCollider2D Collider => field ??= GetComponent<BoxCollider2D>();

    [SerializeInEditor(name: "Is Active")]
    public bool IsActive { get; private set; } = true;

    [SerializeInEditor(name: "Type")]
    public WallType Type
    {
        get;
        set => gameObject.layer = (field = value) switch
        {
            WallType.Both => LayerMask.NameToLayer("Gameplay"),
            WallType.Human => LayerMask.NameToLayer("Block Humans"),
            WallType.Zombie => LayerMask.NameToLayer("Block Zombies"),
            WallType.Explosion => LayerMask.NameToLayer("Block Explosion"),
            _ => throw new ArgumentOutOfRangeException(nameof(WallType), value, null)
        };
    } = WallType.Explosion;

    [SerializeInEditor(name: "Offset")]
    public Vector2 Offset
    {
        get => Collider.offset;
        set => Collider.offset = value;
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
        Collider.enabled = IsActive = state;
    }

    public void SetActive() => SetActive(true);

    public void SetInactive() => SetActive(false);

    public void ToggleActivation() => SetActive(!IsActive);

    private void OnDespawned()
    {
        Collider.size = new Vector2(0.3f, 1.95f);
        Collider.offset = new Vector2(0.65f, 0.975f);
        Type = WallType.Explosion;
    }

    private static PoolSettingsAsset.PoolPrefab _prefab;

    // ReSharper disable Unity.PerformanceAnalysis
    public static PoolSettingsAsset.PoolPrefab PoolPrefab()
    {
        if (_prefab != null) return _prefab;
        var prefab = new GameObject(name: nameof(InvisibleShield));
        DontDestroyOnLoad(prefab);
        prefab.SetActive(false);
        var collider = prefab.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.3f, 1.95f);
        collider.offset = new Vector2(0.65f, 0.975f);
        var shield = prefab.AddComponent<InvisibleShield>();
        shield.Type = WallType.Explosion;
        prefab.AddTags(Tag.Indestructible);
        prefab.SetActive(true);
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