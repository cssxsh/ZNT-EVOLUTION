using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        var context = ModContext.Allocate(metadata, path);
        if (context.Path != path)
        {
            Logger.LogWarning($"[{metadata.Name} {metadata.Version}] has been loaded from '{context.Path}'");
            yield break;
        }

        Logger.LogInfo($"load [{metadata.Name} {metadata.Version}] from package '{path}'");

        var regex = new Regex("""^(?:.+\/)?(\w+)(?:\.(.*))?\.(\w+)$""");
        foreach (var resource in
                 from entry in entries
                 let match = regex.Match(entry.FilenameInZip)
                 where match.Success
                 select new ModResource<ZipFileEntry>
                 {
                     File = entry,
                     Name = match.Groups[1].Value,
                     Type = match.Groups[2].Value,
                     Format = match.Groups[3].Value
                 }
                 into resource
                 orderby resource.Order
                 select resource)
        {
            buffer.SetLength(0);
            yield return zip.ExtractFileAsync(resource.File, buffer).ToCoroutine();
            buffer.Position = 0;
            switch (resource)
            {
                // ModMetadata
                case { Name: "metadata", Format: "json" or "bson" }:
                    continue;
                // UnityEngine.Texture2D
                case { Format: "tga" or "png" or "exr" }:
                {
                    var texture = context.ReadTexture2D(resource.Name, buffer.ToArray());
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {texture}");
                    // ReSharper disable once InvertIf
                    if (texture.name.StartsWith("preview_"))
                    {
                        var rect = new Rect(x: 0, y: 0, width: texture.width, height: texture.height);
                        var preview = context.MakePreview(texture, rect);
                        Logger.LogDebug($"{resource.File.FilenameInZip} -> {preview}");
                    }
                }
                    break;
                // UnityEngine.Material
                case { Type: "material.merge", Format: "json" or "bson" }:
                {
                    var material = context.ReadMaterial(buffer, resource.Format);
                    var texture = material.mainTexture;
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {material} with {texture}, {material.shader}");
                }
                    break;
                // tk2dSpriteCollectionData
                case { Type: "sprite.info", Format: "json" or "bson" }:
                {
                    var sprites = context.ReadSpriteInfo(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {sprites} from {sprites.material}");
                }
                    break;
                // tk2dSpriteCollectionData
                case { Type: "sprite.merge", Format: "json" or "bson" }:
                {
                    var sprites = context.ReadSpriteMerge(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {sprites} from {sprites.material}");
                }
                    break;
                // tk2dSpriteAnimation
                case { Type: "animation", Format: "json" or "bson" }:
                {
                    var animation = context.ReadAnimation(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {animation}");
                }
                    break;
                // ZNT.Evolution.Core.Asset.AnimationAddition
                case { Type: "animation.addition", Format: "json" or "bson" }:
                {
                    var addition = context.ReadAnimationAddition(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {addition.Clips.Length} clips");
                }
                    break;
                // ZNT.Evolution.Core.Asset.CustomVisualEffect
                case { Type: "visual", Format: "json" or "bson" }:
                {
                    var visual = context.ReadVisualEffect(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {visual} from {visual.animation?.Library}");
                }
                    break;
                // ExplosionAsset
                case { Type: "explosion", Format: "json" or "bson" }:
                {
                    var explosion = context.ReadExplosionAsset(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {explosion} from {explosion.EffectToSpawn}");
                }
                    break;
                // DecorAsset
                case { Type: "decor", Format: "json" or "bson" }:
                {
                    var decor = context.ReadDecorAsset(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {decor} from {decor.Animation}");
                }
                    break;
                // BreakablePropAsset
                case { Type: "breakable", Format: "json" or "bson" }:
                {
                    var breakable = context.ReadBreakablePropAsset(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {breakable} from {breakable.Animation}");
                }
                    break;
                // TriggerAsset
                case { Type: "trigger", Format: "json" or "bson" }:
                {
                    var trigger = context.ReadTriggerAsset(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {trigger} from {trigger.Animation}");
                }
                    break;
                // MovingObjectAsset
                case { Type: "moving", Format: "json" or "bson" }:
                {
                    var moving = context.ReadMovingObjectAsset(buffer, resource.Format);
                    var animation = Traverse.Create(moving).Field<tk2dSpriteAnimation>("library").Value;
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {moving} from {animation}");
                }
                    break;
                // SentryGunAsset
                case { Type: "sentry", Format: "json" or "bson" }:
                {
                    var sentry = context.ReadSentryGunAsset(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {sentry} from {sentry.Animation}");
                }
                    break;
                // PhysicObjectAsset
                case { Type: "physic", Format: "json" or "bson" }:
                {
                    var physic = context.ReadPhysicObjectAsset(buffer, resource.Format);
                    var animation = Traverse.Create(physic).Field<tk2dSpriteAnimation>("library").Value;
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {physic} from {animation}");
                }
                    break;
                // HumanAsset
                case { Type: "human", Format: "json" or "bson" }:
                {
                    var human = context.ReadHumanAsset(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {human} from {human.AnimationLibrary}");
                }
                    break;
                // ZNT.Evolution.Core.Asset..SpawnPointAsset
                case { Type: "spawn", Format: "json" or "bson" }:
                {
                    var spawn = context.ReadSpawnPointAsset(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {spawn}");
                }
                    break;
                // Rotorz.Tile.OrientedBrush
                case { Type: "brush.info", Format: "json" or "bson" }:
                {
                    var brush = context.ReadBrushInfo(buffer, resource.Format);
                    var prefab = brush.DefaultOrientation.GetVariation(0);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {brush} from {prefab}");
                }
                    break;
                // Rotorz.Tile.OrientedBrush
                case { Type: "brush.merge", Format: "json" or "bson" }:
                {
                    var brush = context.ReadBrushMerge(buffer, resource.Format);
                    var prefab = brush.DefaultOrientation.GetVariation(0);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {brush} from {prefab}");
                }
                    break;
                // LevelElement
                case { Type: "element", Format: "json" or "bson" }:
                {
                    var element = context.ReadLevelElement(buffer, resource.Format);
                    Logger.LogDebug($"{resource.File.FilenameInZip} -> {element}");
                    Logger.LogInfo($"LevelElement {element.AssetId} - {element.Title} Loaded");
                }
                    break;
                default:
                    Logger.LogWarning($"{resource.File.FilenameInZip} is not supported");
                    break;
            }
        }
    }
}