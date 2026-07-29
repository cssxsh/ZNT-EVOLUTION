using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json;
using Rotorz.Tile;
using UnityEngine;

namespace ZNT.Evolution.Core.Effect;

[SerializeInEditor(name: "Fog")]
[DisallowMultipleComponent]
public class FogOfWar : ResizableParticleSystem, IResizable, IActivable
{
    [JsonIgnore]
    private Dictionary<TileData, Tileset> _tiles = new();

    private static LevelLoaderManager Manager => GameManager.Instance ?? FindObjectOfType<LevelLoaderManager>();

    protected override void OnCreate()
    {
        // base.OnCreate();
    }

    public new void OnResize(Bounds colliderBounds)
    {
        var effect = GetComponentInParent<SignalEffect>();
        Traverse.Create(effect).Field("events").Field<GameObjectEvent>("OnDetected").Value.AddListener(OnEntry);
        Traverse.Create(effect).Field("events").Field<GameObjectEvent>("OnDetectedExit").Value.AddListener(OnExit);
        var system = Manager.TileSystems[TileSystemLayer.LayerType.Foreground];
        if (!system) return;
        _tiles.Clear();
        var a = system.ClosestTileIndexFromWorld(colliderBounds.min);
        var b = system.ClosestTileIndexFromWorld(colliderBounds.max);
        system.ClampIndex(ref a);
        system.ClampIndex(ref b);
        for (var row = b.row; row <= a.row; row++)
        {
            for (var column = a.column; column <= b.column; column++)
            {
                var tile = system.GetTileOrNull(row, column);
                if (tile == null || tile.Empty) continue;
                _tiles[tile] = tile.tileset;
            }
        }
    }

    public new void SetActive(bool state)
    {
        Traverse.Create(this).Field<bool>(nameof(IsActive)).Value = state;
    }

    public void OnEntry(GameObject _)
    {
        foreach (var (data, context) in _tiles) data.tileset = Alpha(context);
        Manager.TileSystems[TileSystemLayer.LayerType.Foreground].UpdateProceduralTiles(true);
    }

    public void OnExit(GameObject _)
    {
        foreach (var (data, context) in _tiles) data.tileset = context;
    }

    private static Dictionary<string, Tileset> _cache = new();

    private static Tileset Alpha(Tileset origin)
    {
        var key = $"{origin.AtlasMaterial}_";
        if (_cache.TryGetValue(key, out var tileset)) return tileset;
        tileset = Instantiate(origin);
        tileset.name = key;
        tileset.AtlasMaterial = Instantiate(origin.AtlasMaterial);
        tileset.AtlasMaterial.color = Color.clear;
        DontDestroyOnLoad(tileset);
        return _cache[key] = tileset;
    }

    private static PoolSettingsAsset.PoolPrefab _prefab;

    // ReSharper disable Unity.PerformanceAnalysis
    public static PoolSettingsAsset.PoolPrefab PoolPrefab()
    {
        if (_prefab != null) return _prefab;
        var prefab = new GameObject(name: nameof(FogOfWar));
        DontDestroyOnLoad(prefab);
        prefab.SetActive(false);
        var fog = prefab.AddComponent<FogOfWar>();
        fog.SetActive(false);
        fog.EditorVisibility = true;
        prefab.SetActive(true);
        // ReSharper disable once Unity.UnknownResource
        var pool = Resources.Load<PoolSettingsAsset>("Assets/GamePoolSettings");
        pool.Prefabs.Add(_prefab = new PoolSettingsAsset.PoolPrefab
        {
            Prefab = prefab.transform,
            Amount = 1
        });
        {
            // TODO: ...
            foreach (var (_, value) in Asset.CustomAssetUtility.Cache)
            {
                if (value is not TriggerAsset { Prefab.name: "InvisibleTrigger", EffectPrefab: null } asset) continue;
                asset.EffectPrefab = fog;
            }
        }
        return _prefab;
    }
}