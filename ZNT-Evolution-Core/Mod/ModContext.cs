using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Mod;

internal class ModContext
{
    public static readonly Dictionary<string, ModContext> Allocated = new();

    public static ModContext Allocate(ModMetadata metadata, string path)
    {
        lock (Allocated)
        {
            // ReSharper disable once InvertIf
            if (Allocated.TryGetValue(metadata.Id, out var allocated))
            {
                throw new AssetException($"'{metadata.Id}' of '{path}' is allocated by '{allocated.Path}'");
            }

            return Allocated[metadata.Id] = new ModContext(path, metadata);
        }
    }

    public static string Release(ModMetadata metadata)
    {
        lock (Allocated)
        {
            // ReSharper disable once InvertIf
            if (Allocated.TryGetValue(metadata.Id, out var allocated))
            {
                foreach (var o in allocated._cache.Values.ToArray()) allocated.Release(o);
                Allocated.Remove(allocated.Metadata.Id);
                return allocated.Path;
            }

            return null;
        }
    }

    public readonly string Path;

    public readonly ModMetadata Metadata;

    public readonly ManualLogSource Logger;

    private readonly Dictionary<string, Object> _cache;

    private ModContext(string path, ModMetadata metadata)
    {
        Path = path;
        Metadata = metadata;
        Logger = BepInExLogger.CreateLogSource(metadata.Name);
        _cache = new Dictionary<string, Object>();
    }

    #region Sprite

    public Texture2D ReadTexture2D(string name, byte[] input)
    {
        var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false, true);
        texture.LoadImage(input);
        texture.name = name;
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        // texture.Apply(true, true);
        Acquire(texture);
        return texture;
    }

    public Sprite MakePreview(Texture2D texture, Rect rect)
    {
        // LevelElement.Preview
        var preview = Sprite.Create(texture: texture, rect: rect, pivot: Vector2.one / 2.0f);
        preview.name = texture.name;
        Acquire(preview);
        return preview;
    }

    public Material ReadMaterial(Stream input, string format)
    {
        var merge = CustomAssetUtility.DeserializeObject<MaterialMerge>(input, format is "bson");
        var material = merge.Create();
        var texture = (Texture2D)material.mainTexture;
        texture.filterMode = material.shader.name switch
        {
            "ZNT/Effects/Wind" => FilterMode.Bilinear,
            "ZNT/Effects/Radioactivity" => FilterMode.Bilinear,
            "ZNT/Effects/Haze" => FilterMode.Bilinear,
            "ZNT/Effects/Water Lit" => FilterMode.Bilinear,
            "ZNT/Effects/Steam" => FilterMode.Bilinear,
            _ => FilterMode.Point
        };
        texture.wrapMode = material.shader.name switch
        {
            "ZNT/Common/Animated Flat Textured Cutout" => TextureWrapMode.Repeat,
            "ZNT/Common/Animated Flat Textured Transparent" => TextureWrapMode.Repeat,
            _ => TextureWrapMode.Clamp
        };
        Acquire(material);
        return material;
    }

    public tk2dSpriteCollectionData ReadSpriteInfo(Stream input, string format)
    {
        var info = CustomAssetUtility.DeserializeObject<SpriteInfo>(input, format is "bson");
        var sprites = info.Create();
        Acquire(sprites);
        return sprites;
    }

    public tk2dSpriteCollectionData ReadSpriteMerge(Stream input, string format)
    {
        var merge = CustomAssetUtility.DeserializeObject<SpriteMerge>(input, format is "bson");
        var sprites = merge.Create();
        Acquire(sprites);
        return sprites;
    }

    public tk2dSpriteAnimation ReadAnimation(Stream input, string format)
    {
        var animation = CustomAssetUtility.DeserializeObject<tk2dSpriteAnimation>(input, format is "bson");
        Acquire(animation);
        return animation;
    }

    public AnimationAddition ReadAnimationAddition(Stream input, string format)
    {
        var addition = CustomAssetUtility.DeserializeObject<AnimationAddition>(input, format is "bson");
        addition.Apply();
        // TODO Acquire(addition);
        return addition;
    }

    public CustomVisualEffect ReadVisualEffect(Stream input, string format)
    {
        var visual = CustomAssetUtility.DeserializeObject<CustomVisualEffect>(input, format is "bson");
        Acquire(visual);
        return visual;
    }

    #endregion

    #region Asset

    public ExplosionAsset ReadExplosionAsset(Stream input, string format)
    {
        var explosion = CustomAssetUtility.DeserializeObject<ExplosionAsset>(input, format is "bson");
        Acquire(explosion);
        return explosion;
    }

    public DecorAsset ReadDecorAsset(Stream input, string format)
    {
        var decor = CustomAssetUtility.DeserializeObject<DecorAsset>(input, format is "bson");
        Acquire(decor);
        return decor;
    }

    public BreakablePropAsset ReadBreakablePropAsset(Stream input, string format)
    {
        var breakable = CustomAssetUtility.DeserializeObject<BreakablePropAsset>(input, format is "bson");
        Acquire(breakable);
        return breakable;
    }

    public TriggerAsset ReadTriggerAsset(Stream input, string format)
    {
        var trigger = CustomAssetUtility.DeserializeObject<TriggerAsset>(input, format is "bson");
        Acquire(trigger);
        return trigger;
    }

    public MovingObjectAsset ReadMovingObjectAsset(Stream input, string format)
    {
        var moving = CustomAssetUtility.DeserializeObject<MovingObjectAsset>(input, format is "bson");
        Acquire(moving);
        return moving;
    }

    public PhysicObjectAsset ReadPhysicObjectAsset(Stream input, string format)
    {
        var physic = CustomAssetUtility.DeserializeObject<PhysicObjectAsset>(input, format is "bson");
        Acquire(physic);
        return physic;
    }

    public SentryGunAsset ReadSentryGunAsset(Stream input, string format)
    {
        var sentry = CustomAssetUtility.DeserializeObject<SentryGunAsset>(input, format is "bson");
        Acquire(sentry);
        return sentry;
    }

    public HumanAsset ReadHumanAsset(Stream input, string format)
    {
        var human = CustomAssetUtility.DeserializeObject<HumanAsset>(input, format is "bson");
        Acquire(human);
        return human;
    }

    public SpawnPointAsset ReadSpawnPointAsset(Stream input, string format)
    {
        var spawn = CustomAssetUtility.DeserializeObject<SpawnPointAsset>(input, format is "bson");
        Acquire(spawn);
        return spawn;
    }

    #endregion

    #region LevelElement

    public Rotorz.Tile.OrientedBrush ReadBrushInfo(Stream input, string format)
    {
        var info = CustomAssetUtility.DeserializeObject<BrushInfo>(input, format is "bson");
        var brush = info.Create();
        Acquire(brush);
        return brush;
    }

    public Rotorz.Tile.OrientedBrush ReadBrushMerge(Stream input, string format)
    {
        var merge = CustomAssetUtility.DeserializeObject<BrushMerge>(input, format is "bson");
        var brush = merge.Create();
        Acquire(brush);
        return brush;
    }

    public LevelElement ReadLevelElement(Stream input, string format)
    {
        var element = CustomAssetUtility.DeserializeObject<LevelElement>(input, format is "bson");
        Acquire(element);
        return element;
    }

    #endregion

    private void Acquire(Object o)
    {
        lock (_cache)
        {
            if (!_cache.TryAdd(o.NameAndType(), o))
            {
                Logger.LogWarning($"{o.NameAndType()} is already cached");
                return;
            }

            switch (o)
            {
                case VisualEffect visual:
                    _ = visual.Bind();
                    Logger.LogInfo($"Bind VisualEffect {visual.AssetId} - {visual.name}");
                    break;
                case LevelElement element:
                    _ = element.Bind();
                    Logger.LogInfo($"Bind LevelElement {element.AssetId} - {element.Title}");
                    break;
            }

            CustomAssetUtility.Cache[o.NameAndType()] = o;
            Object.DontDestroyOnLoad(o);
        }
    }

    private void Release(Object o)
    {
        lock (_cache)
        {
            if (!_cache.Remove(o.NameAndType()))
            {
                Logger.LogWarning($"{o.NameAndType()} is already released");
                return;
            }

            switch (o)
            {
                case VisualEffect visual:
                    visual.Unbind();
                    Logger.LogInfo($"Unbind VisualEffect {visual.AssetId} - {visual.name}");
                    break;
                case LevelElement element:
                    element.Unbind();
                    Logger.LogInfo($"Unbind LevelElement {element.AssetId} - {element.Title}");
                    break;
            }

            CustomAssetUtility.Cache.Remove(o.NameAndType());
            Object.Destroy(o);
        }
    }
}