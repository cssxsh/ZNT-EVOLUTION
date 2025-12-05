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
        // transform.localPosition = Vector3.right;
    }

    private void OnDespawned()
    {
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
        prefab.AddComponent<InvisibleShield>();
        prefab.AddTags(Tag.Indestructible);
        prefab.layer = LayerMask.NameToLayer("Block Explosion");
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