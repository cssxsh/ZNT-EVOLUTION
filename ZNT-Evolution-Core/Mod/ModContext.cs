using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZNT.Evolution.Core.Asset;

namespace ZNT.Evolution.Core.Mod;

internal class ModContext
{
    private static readonly Dictionary<string, ModContext> Allocated = new();

    public static ModContext Allocate(ModMetadata metadata, string path)
    {
        lock (Allocated)
        {
            return Allocated.TryGetValue(metadata.Id, out var allocated)
                ? allocated
                : Allocated[metadata.Id] = new ModContext(path, metadata);
        }
    }

    public readonly string Path;

    public readonly ModMetadata Metadata;

    private ModContext(string path, ModMetadata metadata)
    {
        Path = path;
        Metadata = metadata;
    }

    #region Sprite / Animation / VisualEffect

    internal Texture2D ReadTexture2D(string name, byte[] input)
    {
        var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false, true);
        texture.LoadImage(input);
        texture.name = name;
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        // texture.Apply(true, true);
        Object.DontDestroyOnLoad(texture);
        CustomAssetUtility.Cache[texture.NameAndType()] = texture;
        return texture;
    }

    internal Sprite MakePreview(Texture2D texture, Rect rect)
    {
        // LevelElement.Preview
        var preview = Sprite.Create(texture: texture, rect: rect, pivot: Vector2.one / 2.0f);
        preview.name = texture.name;
        Object.DontDestroyOnLoad(preview);
        CustomAssetUtility.Cache[preview.NameAndType()] = preview;
        return preview;
    }

    internal Material ReadMaterial(Stream input, string format)
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
        CustomAssetUtility.Cache[material.NameAndType()] = material;
        return material;
    }

    internal tk2dSpriteCollectionData ReadSpriteInfo(Stream input, string format)
    {
        var info = CustomAssetUtility.DeserializeObject<SpriteInfo>(input, format is "bson");
        var sprites = info.Create();
        CustomAssetUtility.Cache[sprites.NameAndType()] = sprites;
        return sprites;
    }

    internal tk2dSpriteCollectionData ReadSpriteMerge(Stream input, string format)
    {
        var merge = CustomAssetUtility.DeserializeObject<SpriteMerge>(input, format is "bson");
        var sprites = merge.Create();
        CustomAssetUtility.Cache[sprites.NameAndType()] = sprites;
        return sprites;
    }

    internal tk2dSpriteAnimation ReadAnimation(Stream input, string format)
    {
        var animation = CustomAssetUtility.DeserializeObject<tk2dSpriteAnimation>(input, format is "bson");
        CustomAssetUtility.Cache[animation.NameAndType()] = animation;
        return animation;
    }

    internal AnimationAddition ReadAnimationAddition(Stream input, string format)
    {
        var addition = CustomAssetUtility.DeserializeObject<AnimationAddition>(input, format is "bson");
        addition.Apply();
        return addition;
    }

    internal CustomVisualEffect ReadVisualEffect(Stream input, string format)
    {
        var visual = CustomAssetUtility.DeserializeObject<CustomVisualEffect>(input, format is "bson");
        CustomAssetUtility.Cache[visual.NameAndType()] = visual;
        _ = visual.Bind();
        return visual;
    }

    #endregion

    #region CustomAsset / LevelElement

    internal ExplosionAsset ReadExplosionAsset(Stream input, string format)
    {
        var explosion = CustomAssetUtility.DeserializeObject<ExplosionAsset>(input, format is "bson");
        CustomAssetUtility.Cache[explosion.NameAndType()] = explosion;
        return explosion;
    }

    internal DecorAsset ReadDecorAsset(Stream input, string format)
    {
        var decor = CustomAssetUtility.DeserializeObject<DecorAsset>(input, format is "bson");
        CustomAssetUtility.Cache[decor.NameAndType()] = decor;
        return decor;
    }

    internal BreakablePropAsset ReadBreakablePropAsset(Stream input, string format)
    {
        var breakable = CustomAssetUtility.DeserializeObject<BreakablePropAsset>(input, format is "bson");
        CustomAssetUtility.Cache[breakable.NameAndType()] = breakable;
        return breakable;
    }

    internal TriggerAsset ReadTriggerAsset(Stream input, string format)
    {
        var trigger = CustomAssetUtility.DeserializeObject<TriggerAsset>(input, format is "bson");
        CustomAssetUtility.Cache[trigger.NameAndType()] = trigger;
        return trigger;
    }

    internal MovingObjectAsset ReadMovingObjectAsset(Stream input, string format)
    {
        var moving = CustomAssetUtility.DeserializeObject<MovingObjectAsset>(input, format is "bson");
        CustomAssetUtility.Cache[moving.NameAndType()] = moving;
        return moving;
    }

    internal PhysicObjectAsset ReadPhysicObjectAsset(Stream input, string format)
    {
        var physic = CustomAssetUtility.DeserializeObject<PhysicObjectAsset>(input, format is "bson");
        CustomAssetUtility.Cache[physic.NameAndType()] = physic;
        return physic;
    }

    internal SentryGunAsset ReadSentryGunAsset(Stream input, string format)
    {
        var sentry = CustomAssetUtility.DeserializeObject<SentryGunAsset>(input, format is "bson");
        CustomAssetUtility.Cache[sentry.NameAndType()] = sentry;
        return sentry;
    }

    internal HumanAsset ReadHumanAsset(Stream input, string format)
    {
        var human = CustomAssetUtility.DeserializeObject<HumanAsset>(input, format is "bson");
        CustomAssetUtility.Cache[human.NameAndType()] = human;
        return human;
    }

    internal SpawnPointAsset ReadSpawnPointAsset(Stream input, string format)
    {
        var spawn = CustomAssetUtility.DeserializeObject<SpawnPointAsset>(input, format is "bson");
        CustomAssetUtility.Cache[spawn.NameAndType()] = spawn;
        return spawn;
    }

    internal Rotorz.Tile.OrientedBrush ReadBrushInfo(Stream input, string format)
    {
        var info = CustomAssetUtility.DeserializeObject<BrushInfo>(input, format is "bson");
        var brush = info.Create();
        CustomAssetUtility.Cache[brush.NameAndType()] = brush;
        return brush;
    }

    internal Rotorz.Tile.OrientedBrush ReadBrushMerge(Stream input, string format)
    {
        var merge = CustomAssetUtility.DeserializeObject<BrushMerge>(input, format is "bson");
        var brush = merge.Create();
        CustomAssetUtility.Cache[brush.NameAndType()] = brush;
        return brush;
    }

    internal LevelElement ReadLevelElement(Stream input, string format)
    {
        var element = CustomAssetUtility.DeserializeObject<LevelElement>(input, format is "bson");
        CustomAssetUtility.Cache[element.NameAndType()] = element;
        _ = element.Bind();
        return element;
    }

    #endregion
}