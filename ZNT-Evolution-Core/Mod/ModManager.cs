using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Mod;

public static class ModManager
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(ModManager));

    static ModManager() => Encoding.RegisterProvider(new Fix437EncodingProvider(Encoding.ASCII));

    private class Fix437EncodingProvider(Encoding fallback) : EncodingProvider
    {
        public override Encoding GetEncoding(string name) => name is "IBM437" ? fallback : null;
        public override Encoding GetEncoding(int codepage) => codepage is 437 ? fallback : null;
    }

    private static readonly Dictionary<string, ModContext> Allocated = new();

    private static IEnumerator ToCoroutine(this Task task)
    {
        while (!task.IsCompleted) yield return null;
        if (task.Exception != null) throw task.Exception;
    }

    private static bool IsBson(this ZipFileEntry entry) => entry.FilenameInZip.EndsWith(".bson");

    public static IEnumerator LoadFromPackage(string path)
    {
        using var zip = ZipStorer.Open(path, FileAccess.Read);
        var entries = zip.ReadCentralDir();
        var meta = zip.GetEntry("metadata.json") ?? zip.GetEntry("metadata.bson");
        if (meta is null) throw new FileNotFoundException($"metadata in {path}");
        using var buffer = new MemoryStream();
        yield return zip.ExtractFileAsync(meta, buffer).ToCoroutine();
        buffer.Position = 0;
        var metadata = CustomAssetUtility.DeserializeObject<ModMetadata>(buffer, meta.IsBson());
        Logger.LogInfo($"load [{metadata.Name} {metadata.Version}] from package '{path}'.");
        var context = AllocateContext(metadata, path);
        if (context is null) yield break;

        var sprite = entries.Where(entry => entry.FilenameInZip.StartsWith("Sprite/")).ToList();
        // UnityEngine.Texture2D
        foreach (var entry in sprite.Where(entry => entry.FilenameInZip.EndsWith(".png")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false, true);
            texture.LoadImage(buffer.ToArray());
            texture.name = Path.GetFileNameWithoutExtension(entry.FilenameInZip);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            // texture.Apply(true, true);
            Logger.LogDebug($"{entry.FilenameInZip} -> {texture}");
            CustomAssetUtility.Cache[texture.NameAndType()] = texture;
            // LevelElement.Preview
            // ReSharper disable once InvertIf
            if (texture.name.StartsWith("preview_"))
            {
                var preview = Sprite.Create(
                    texture: texture,
                    rect: new Rect(x: 0, y: 0, width: texture.width, height: texture.height),
                    pivot: Vector2.one / 2.0f);
                preview.name = texture.name;
                Object.DontDestroyOnLoad(preview);
                Logger.LogDebug($"{entry.FilenameInZip} -> {preview}");
                CustomAssetUtility.Cache[preview.NameAndType()] = preview;
            }
        }

        // UnityEngine.Material
        foreach (var entry in entries.Where(entry =>
                     entry.FilenameInZip.EndsWith(".material.merge.json") ||
                     entry.FilenameInZip.EndsWith(".material.merge.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var merge = CustomAssetUtility.DeserializeObject<MaterialMerge>(buffer, entry.IsBson());
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
            Logger.LogDebug($"{entry.FilenameInZip} -> {material} from {texture}");
            CustomAssetUtility.Cache[material.NameAndType()] = material;
        }

        // tk2dSpriteCollectionData
        foreach (var entry in entries.Where(entry =>
                     entry.FilenameInZip.EndsWith(".sprite.info.json") ||
                     entry.FilenameInZip.EndsWith(".sprite.info.bson") ||
                     entry.FilenameInZip.EndsWith(".sprite.merge.json") ||
                     entry.FilenameInZip.EndsWith(".sprite.merge.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            EvolutionInfo<tk2dSpriteCollectionData> info = entry.FilenameInZip.Contains(".info.")
                ? CustomAssetUtility.DeserializeObject<SpriteInfo>(buffer, entry.IsBson())
                : CustomAssetUtility.DeserializeObject<SpriteMerge>(buffer, entry.IsBson());
            var sprites = info.Create();
            Logger.LogDebug($"{entry.FilenameInZip} -> {sprites} from {sprites.material}");
            CustomAssetUtility.Cache[sprites.NameAndType()] = sprites;
        }

        // tk2dSpriteAnimation
        foreach (var entry in entries.Where(entry =>
                     entry.FilenameInZip.EndsWith(".animation.json") ||
                     entry.FilenameInZip.EndsWith(".animation.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var animation = CustomAssetUtility.DeserializeObject<tk2dSpriteAnimation>(buffer, entry.IsBson());
            Logger.LogDebug($"{entry.FilenameInZip} -> {animation}");
            CustomAssetUtility.Cache[animation.NameAndType()] = animation;
        }

        // ZNT.Evolution.Core.Asset.AnimationAddition
        foreach (var entry in entries.Where(entry =>
                     entry.FilenameInZip.EndsWith(".animation.addition.json") ||
                     entry.FilenameInZip.EndsWith(".animation.addition.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var addition = CustomAssetUtility.DeserializeObject<AnimationAddition>(buffer, entry.IsBson());
            addition.Apply();
            Logger.LogDebug($"{entry.FilenameInZip} -> {addition}");
        }

        // ZNT.Evolution.Core.Asset.CustomVisualEffect
        foreach (var entry in entries.Where(entry =>
                     entry.FilenameInZip.EndsWith(".visual.json") ||
                     entry.FilenameInZip.EndsWith(".visual.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var visual = CustomAssetUtility.DeserializeObject<CustomVisualEffect>(buffer, entry.IsBson());
            Logger.LogDebug($"{entry.FilenameInZip} -> {visual} from {visual.animation?.Library}");
            CustomAssetUtility.Cache[visual.NameAndType()] = visual;
            _ = visual.Bind();
        }

        var asset = entries.Where(entry => entry.FilenameInZip.StartsWith("Asset/")).ToList();
        // ExplosionAsset
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".explosion.json") ||
                     entry.FilenameInZip.EndsWith(".explosion.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var explosion = CustomAssetUtility.DeserializeObject<ExplosionAsset>(buffer, entry.IsBson());
            Logger.LogDebug($"{entry.FilenameInZip} -> {explosion} from {explosion.EffectToSpawn}");
            CustomAssetUtility.Cache[explosion.NameAndType()] = explosion;
        }

        // PhysicObjectAsset
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".physic.json") ||
                     entry.FilenameInZip.EndsWith(".physic.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var physic = CustomAssetUtility.DeserializeObject<PhysicObjectAsset>(buffer, entry.IsBson());
            var animation = Traverse.Create(physic).Field<tk2dSpriteAnimation>("library").Value;
            Logger.LogDebug($"{entry.FilenameInZip} -> {physic} from {animation}");
            CustomAssetUtility.Cache[physic.NameAndType()] = physic;
        }

        // HumanAsset
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".human.json") ||
                     entry.FilenameInZip.EndsWith(".human.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var human = CustomAssetUtility.DeserializeObject<HumanAsset>(buffer, entry.IsBson());
            Logger.LogDebug($"{entry.FilenameInZip} -> {human} from {human.AnimationLibrary}");
            CustomAssetUtility.Cache[human.NameAndType()] = human;
        }

        // DecorAsset
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".decor.json") ||
                     entry.FilenameInZip.EndsWith(".decor.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var decor = CustomAssetUtility.DeserializeObject<DecorAsset>(buffer, entry.IsBson());
            Logger.LogDebug($"{entry.FilenameInZip} -> {decor} from {decor.Animation}");
            CustomAssetUtility.Cache[decor.NameAndType()] = decor;
        }

        // BreakablePropAsset
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".breakable.json") ||
                     entry.FilenameInZip.EndsWith(".breakable.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var breakable = CustomAssetUtility.DeserializeObject<BreakablePropAsset>(buffer, entry.IsBson());
            Logger.LogDebug($"{entry.FilenameInZip} -> {breakable} from {breakable.Animation}");
            CustomAssetUtility.Cache[breakable.NameAndType()] = breakable;
        }

        // SentryGunAsset
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".sentry.json") ||
                     entry.FilenameInZip.EndsWith(".sentry.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var sentry = CustomAssetUtility.DeserializeObject<SentryGunAsset>(buffer, entry.IsBson());
            Logger.LogDebug($"{entry.FilenameInZip} -> {sentry} from {sentry.Animation}");
            CustomAssetUtility.Cache[sentry.NameAndType()] = sentry;
        }

        // MovingObjectAsset
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".moving.json") ||
                     entry.FilenameInZip.EndsWith(".moving.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var moving = CustomAssetUtility.DeserializeObject<MovingObjectAsset>(buffer, entry.IsBson());
            var animation = Traverse.Create(moving).Field<tk2dSpriteAnimation>("library").Value;
            Logger.LogDebug($"{entry.FilenameInZip} -> {moving} from {animation}");
            CustomAssetUtility.Cache[moving.NameAndType()] = moving;
        }

        // TriggerAsset
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".trigger.json") ||
                     entry.FilenameInZip.EndsWith(".trigger.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var trigger = CustomAssetUtility.DeserializeObject<TriggerAsset>(buffer, entry.IsBson());
            Logger.LogDebug($"{entry.FilenameInZip} -> {trigger} from {trigger.Animation}");
            CustomAssetUtility.Cache[trigger.NameAndType()] = trigger;
        }

        // ZNT.Evolution.Core.Asset.SpawnPointAsset
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".spawn.json") ||
                     entry.FilenameInZip.EndsWith(".spawn.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var spawn = CustomAssetUtility.DeserializeObject<SpawnPointAsset>(buffer, entry.IsBson());
            Logger.LogDebug($"{entry.FilenameInZip} -> {spawn}");
            CustomAssetUtility.Cache[spawn.NameAndType()] = spawn;
        }

        // Rotorz.Tile.OrientedBrush
        foreach (var entry in asset.Where(entry =>
                     entry.FilenameInZip.EndsWith(".brush.info.json") ||
                     entry.FilenameInZip.EndsWith(".brush.info.bson") ||
                     entry.FilenameInZip.EndsWith(".brush.merge.json") ||
                     entry.FilenameInZip.EndsWith(".brush.merge.bson")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            EvolutionInfo<Rotorz.Tile.OrientedBrush> info = entry.FilenameInZip.Contains(".info.")
                ? CustomAssetUtility.DeserializeObject<BrushInfo>(buffer, entry.IsBson())
                : CustomAssetUtility.DeserializeObject<BrushMerge>(buffer, entry.IsBson());
            var brush = info.Create();
            Logger.LogDebug($"{entry.FilenameInZip} -> {brush} from {brush.DefaultOrientation.GetVariation(0)}");
            CustomAssetUtility.Cache[brush.NameAndType()] = brush;
        }

        // LevelElement
        foreach (var entry in asset.Where(entry => entry.FilenameInZip.EndsWith(".element.json")))
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(entry, buffer).ToCoroutine();
            buffer.Position = 0;
            var element = CustomAssetUtility.DeserializeObject<LevelElement>(buffer);
            Logger.LogDebug($"{entry.FilenameInZip} -> {element}");
            CustomAssetUtility.Cache[element.NameAndType()] = element;
            _ = element.Bind();
            Logger.LogInfo($"LevelElement {element.name} - {element.Title} Loaded");
        }
    }

    private static ModContext AllocateContext(ModMetadata metadata, string path)
    {
        lock (Allocated)
        {
            if (!Allocated.TryGetValue(metadata.Id, out var prev))
                return Allocated[metadata.Id] = new ModContext(path, metadata);
            Logger.LogWarning($"{metadata.Id} has been loaded from {prev.Path}");
            return null;
        }
    }
}