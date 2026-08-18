using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Mod;

public class ModContext
{
    private static readonly Dictionary<string, ModContext> Contexts = new();

    private static readonly ReaderWriterLockSlim Lock = new();

    public static IReadOnlyCollection<ModContext> Allocated()
    {
        Lock.EnterReadLock();
        try
        {
            return Contexts.Values;
        }
        finally
        {
            Lock.ExitReadLock();
        }
    }

    public static ModContext Allocate(string path)
    {
        Lock.EnterWriteLock();
        try
        {
            var metadata = Directory.Exists(path)
                ? ModMetadata.FromFolder(path)
                : ModMetadata.FromPackage(path);
            // ReSharper disable once InvertIf
            if (Contexts.TryGetValue(metadata.Id, out var allocated))
            {
                throw new AssetException($"'{metadata.Id}' of '{path}' is allocated by '{allocated.Path}'");
            }

            return Contexts[metadata.Id] = new ModContext(path, metadata);
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    public static void Free(string id)
    {
        Lock.EnterWriteLock();
        try
        {
            // ReSharper disable once InvertIf
            if (Contexts.TryGetValue(id, out var allocated))
            {
                if (allocated.State is not ModState.Idle) throw new AssetException($"'{id}' is not Idle");
                Contexts.Remove(allocated.Metadata.Id);
            }
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    public readonly string Path;

    public readonly ModMetadata Metadata;

    public readonly ManualLogSource Logger;

    public ModState State { private set; get; }

    public readonly I2.Loc.LanguageSourceData Localization;

    public string Title => $"{Metadata.Name} v{Metadata.Version}";

    public System.Version Version => System.Version.Parse(Metadata.Version);

    private ModContext(string path, ModMetadata metadata)
    {
        Path = path;
        Metadata = metadata;
        Logger = BepInExLogger.CreateLogSource(metadata.Name);
        State = ModState.Idle;
        Localization = new I2.Loc.LanguageSourceData
        {
            GoogleUpdateFrequency = I2.Loc.LanguageSourceData.eGoogleUpdateFrequency.Never,
            GoogleInEditorCheckFrequency = I2.Loc.LanguageSourceData.eGoogleUpdateFrequency.Never
        };
    }

    private static readonly Regex InfoRegex = new("""^(?:.+\/)?([^.]+)(?:\.(.*))?\.(\w+)$""", RegexOptions.Compiled);

    public bool IsLoadReady()
    {
        Lock.EnterReadLock();
        try
        {
            if (State is not ModState.Idle) return false;
            foreach (var (id, version) in Metadata.Dependencies)
            {
                var need = System.Version.Parse(version);
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue(id, out var plugin) &&
                    plugin.Metadata.Version >= need) continue;
                if (Contexts.TryGetValue(id, out var allocated) &&
                    allocated.Version >= need &&
                    allocated.State is ModState.Loaded) continue;
                return false;
            }

            return true;
        }
        finally
        {
            Lock.ExitReadLock();
        }
    }

    public async Task Load()
    {
        Lock.EnterWriteLock();
        try
        {
            if (State is ModState.Loaded)
            {
                Logger.LogInfo($"[{Title}] is already loaded");
                return;
            }

            State = ModState.Frozen;

            foreach (var (id, version) in Metadata.Dependencies)
            {
                var need = System.Version.Parse(version);
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue(id, out var plugin) &&
                    plugin.Metadata.Version >= need) continue;
                if (Contexts.TryGetValue(id, out var allocated) &&
                    allocated.Version >= need &&
                    allocated.State is ModState.Loaded) continue;
                throw new AssetException($"[{Title}] dependency {id} - {version}]");
            }

            using var buffer = new MemoryStream();
            if (Directory.Exists(Path))
            {
                Logger.LogInfo($"load [{Title}] from folder '{Path}'");
                var folder = new DirectoryInfo(Path);
                foreach (var resource in
                         from file in folder.EnumerateFiles("*", SearchOption.AllDirectories)
                         let match = InfoRegex.Match(file.Name)
                         where match.Success
                         select new ModResource<FileInfo>
                         {
                             File = file,
                             Path = file.FullName.Substring(folder.FullName.Length + 1).Replace('\\', '/'),
                             Name = match.Groups[1].Value,
                             Type = match.Groups[2].Value,
                             Format = match.Groups[3].Value
                         }
                         into resource
                         orderby resource.Order
                         select resource)
                {
                    buffer.SetLength(0);
                    using var temp = resource.File.OpenRead();
                    await temp.CopyToAsync(buffer);
                    buffer.Position = 0;
                    try
                    {
                        LoadResource(resource, buffer);
                    }
                    catch (System.Exception e)
                    {
                        throw new AssetException($"[{Title}]", e);
                    }
                }
            }
            else
            {
                Logger.LogInfo($"load [{Title}] from package '{Path}'");
                using var package = ZipStorer.Open(Path, FileAccess.Read);
                foreach (var resource in
                         from entry in package.ReadCentralDir()
                         let match = InfoRegex.Match(entry.FilenameInZip)
                         where match.Success
                         select new ModResource<ZipFileEntry>
                         {
                             File = entry,
                             Path = entry.FilenameInZip,
                             Name = match.Groups[1].Value,
                             Type = match.Groups[2].Value,
                             Format = match.Groups[3].Value
                         }
                         into resource
                         orderby resource.Order
                         select resource)
                {
                    buffer.SetLength(0);
                    await package.ExtractFileAsync(resource.File, buffer);
                    buffer.Position = 0;
                    try
                    {
                        LoadResource(resource, buffer);
                    }
                    catch (System.Exception e)
                    {
                        throw new AssetException($"[{Title}]", e);
                    }
                }
            }

            if (Localization.mTerms.Count > 0 && !I2.Loc.LocalizationManager.Sources.Contains(Localization))
            {
                I2.Loc.LocalizationManager.Sources.Add(Localization);
            }

            State = ModState.Loaded;
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    public bool IsUnloadReady()
    {
        Lock.EnterReadLock();
        try
        {
            foreach (var (_, context) in Contexts)
            {
                if (context.Metadata.Dependencies.ContainsKey(Metadata.Id) &&
                    context.State is ModState.Loaded) return false;
            }

            return true;
        }
        finally
        {
            Lock.ExitReadLock();
        }
    }

    public async Task Unload()
    {
        Lock.EnterWriteLock();
        try
        {
            foreach (var (_, context) in Contexts)
            {
                if (!context.Metadata.Dependencies.ContainsKey(Metadata.Id) ||
                    context.State is not ModState.Loaded) continue;
                var name = context.Metadata.Name;
                var version = context.Metadata.Version;
                throw new AssetException($"[{name} {version}] dependency {Metadata.Id} - {Metadata.Version}]");
            }

            var keys = new List<string>(_cache.Keys.Reverse());
            foreach (var key in keys)
            {
                await Task.CompletedTask;
                try
                {
                    Release(key);
                }
                catch (System.Exception e)
                {
                    throw new AssetException($"[{Title}]", e);
                }
            }

            I2.Loc.LocalizationManager.Sources.Remove(Localization);
            State = ModState.Idle;
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }

    private void LoadResource<T>(ModResource<T> resource, MemoryStream buffer)
    {
        switch (resource)
        {
            // ModMetadata
            case { Name: "metadata", Type: "", Format: "json" }:
                return;
            // Localization
            case { Type: "localization", Format: "csv" }:
            {
                Localization.Import_CSV(Category: resource.Name, CSVstring: Encoding.UTF8.GetString(buffer.ToArray()));
                Logger.LogDebug($"{resource.Path} -> Localization");
            }
                return;
            // FMOD.Studio.Bank
            case { Format: "bank", Type: "strings" }:
            {
                if (FMODUnity.RuntimeManager.HasBankLoaded(resource.Name + ".strings")) break;
                _ = ReadBank(resource.Name + ".strings", buffer.ToArray());
                Logger.LogDebug($"{resource.Path} -> index of {resource.Name}");
            }
                break;
            // FMOD.Studio.Bank
            case { Format: "bank" }:
            {
                if (FMODUnity.RuntimeManager.HasBankLoaded(resource.Name + ".strings")) break;
                if (FMODUnity.RuntimeManager.HasBankLoaded(resource.Name)) break;
                var bank = ReadBank(resource.Name, buffer.ToArray());
                Logger.LogDebug($"{resource.Path} -> bank:/{bank.name}");
            }
                break;
            // UnityEngine.Texture2D
            case { Format: "tga" or "png" or "exr", Type: "" }:
            {
                var texture = ReadTexture2D(resource.Name, buffer.ToArray());
                Logger.LogDebug($"{resource.Path} -> {texture}");
            }
                break;
            // UnityEngine.Material
            case { Type: "material.merge", Format: "json" or "bson" }:
            {
                var material = ReadMaterial(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {material} with {material.shader}");
            }
                break;
            // tk2dSpriteCollectionData
            case { Type: "sprite.info", Format: "json" or "bson" }:
            {
                var sprites = ReadSpriteInfo(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {sprites} from {sprites.material}");
            }
                break;
            // tk2dSpriteCollectionData
            case { Type: "sprite.merge", Format: "json" or "bson" }:
            {
                var sprites = ReadSpriteMerge(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {sprites} from {sprites.material}");
            }
                break;
            // tk2dSpriteAnimation
            case { Type: "animation", Format: "json" or "bson" }:
            {
                var animation = ReadAnimation(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {animation}");
            }
                break;
            // ZNT.Evolution.Core.Asset.AnimationAddition
            case { Type: "animation.addition", Format: "json" or "bson" }:
            {
                var addition = ReadAnimationAddition(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {addition}");
            }
                break;
            // ZNT.Evolution.Core.Asset.CustomVisualEffect
            case { Type: "visual", Format: "json" or "bson" }:
            {
                var visual = ReadVisualEffect(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {visual} from {visual.animation?.Library}");
            }
                break;
            // TMPro.TMP_FontAsset
            case { Type: "font", Format: "json" or "bson" }:
            {
                var font = ReadFont(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {font} from {font.material}");
            }
                break;
            // TMPro.TMP_SpriteAsset
            case { Type: "emoji", Format: "json" or "bson" }:
            {
                var emoji = ReadEmoji(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {emoji} from {emoji.material}");
            }
                break;
            // ExplosionAsset
            case { Type: "explosion", Format: "json" or "bson" }:
            {
                var explosion = ReadExplosionAsset(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {explosion} from {explosion.EffectToSpawn}");
            }
                break;
            // DecorAsset
            case { Type: "decor", Format: "json" or "bson" }:
            {
                var decor = ReadDecorAsset(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {decor} from {decor.Animation}");
            }
                break;
            // BreakablePropAsset
            case { Type: "breakable", Format: "json" or "bson" }:
            {
                var breakable = ReadBreakablePropAsset(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {breakable} from {breakable.Animation}");
            }
                break;
            // TriggerAsset
            case { Type: "trigger", Format: "json" or "bson" }:
            {
                var trigger = ReadTriggerAsset(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {trigger} from {trigger.Animation}");
            }
                break;
            // MovingObjectAsset
            case { Type: "moving", Format: "json" or "bson" }:
            {
                var moving = ReadMovingObjectAsset(buffer, resource.Format);
                var animation = Traverse.Create(moving).Field<tk2dSpriteAnimation>("library").Value;
                Logger.LogDebug($"{resource.Path} -> {moving} from {animation}");
            }
                break;
            // SentryGunAsset
            case { Type: "sentry", Format: "json" or "bson" }:
            {
                var sentry = ReadSentryGunAsset(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {sentry} from {sentry.Animation}");
            }
                break;
            // PhysicObjectAsset
            case { Type: "physic", Format: "json" or "bson" }:
            {
                var physic = ReadPhysicObjectAsset(buffer, resource.Format);
                var animation = Traverse.Create(physic).Field<tk2dSpriteAnimation>("library").Value;
                Logger.LogDebug($"{resource.Path} -> {physic} from {animation}");
            }
                break;
            // HumanAsset
            case { Type: "human", Format: "json" or "bson" }:
            {
                var human = ReadHumanAsset(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {human} from {human.AnimationLibrary}");
            }
                break;
            // ZNT.Evolution.Core.Asset.SpawnPointAsset
            case { Type: "spawn", Format: "json" or "bson" }:
            {
                var spawn = ReadSpawnPointAsset(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {spawn}");
            }
                break;
            // ZNT.Evolution.Core.Asset.AnimationAddition
            case { Type: "asset.addition", Format: "json" or "bson" }:
            {
                var addition = ReadAssetAddition(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {addition}");
            }
                break;
            // Rotorz.Tile.OrientedBrush
            case { Type: "brush.info", Format: "json" or "bson" }:
            {
                var brush = ReadBrushInfo(buffer, resource.Format);
                var prefab = brush.DefaultOrientation.GetVariation(0);
                Logger.LogDebug($"{resource.Path} -> {brush} from {prefab}");
            }
                break;
            // Rotorz.Tile.OrientedBrush
            case { Type: "brush.merge", Format: "json" or "bson" }:
            {
                var brush = ReadBrushMerge(buffer, resource.Format);
                var prefab = brush.DefaultOrientation.GetVariation(0);
                Logger.LogDebug($"{resource.Path} -> {brush} from {prefab}");
            }
                break;
            // UnityEngine.Sprite
            case { Type: "preview", Format: "tga" or "png" or "exr" }:
            {
                var preview = ReadPreview(resource.Name, buffer.ToArray());
                Logger.LogDebug($"{resource.Path} -> {preview}");
            }
                break;
            // UnityEngine.Sprite
            case { Type: "preview.info", Format: "json" or "bson" }:
            {
                var preview = ReadPreviewInfo(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {preview} from {preview.texture}");
            }
                break;
            // LevelElement
            case { Type: "element", Format: "json" or "bson" }:
            {
                var element = ReadLevelElement(buffer, resource.Format);
                Logger.LogDebug($"{resource.Path} -> {element}");

                // ReSharper disable once InvertIf
                if (element is { ElementType: LevelElement.Type.Brush, Brush: null, LinkedElement.Element: not null })
                {
                    element.Brush = MakeBrush(element);
                    Logger.LogDebug($"{resource.Path} -> {element.Brush}");
                }

                // ReSharper disable once InvertIf
                if (element is { ElementType: LevelElement.Type.Brush, DecorPrefab.name: "Chopter" })
                {
                    element.CustomAsset = MakeHook(element);
                    Logger.LogDebug($"{resource.Path} -> {element.CustomAsset}");
                }
            }
                break;
            // Unsupported
            default:
                Logger.LogWarning($"{resource.Path} is not supported");
                break;
        }
    }

    #region FMOD

    private BankAsset ReadBank(string name, byte[] input)
    {
        var bank = new BankAsset { name = name, data = input };
        Acquire(bank);
        bank.data = null;
        return bank;
    }

    #endregion

    #region Sprite

    private Texture2D ReadTexture2D(string name, byte[] input)
    {
        var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false, true);
        texture.LoadImage(input, true);
        texture.name = name;
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        Acquire(texture);
        return texture;
    }

    private Material ReadMaterial(Stream input, string format)
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

    private tk2dSpriteCollectionData ReadSpriteInfo(Stream input, string format)
    {
        var info = CustomAssetUtility.DeserializeObject<SpriteInfo>(input, format is "bson");
        var sprites = info.Create();
        Acquire(sprites);
        return sprites;
    }

    private tk2dSpriteCollectionData ReadSpriteMerge(Stream input, string format)
    {
        var merge = CustomAssetUtility.DeserializeObject<SpriteMerge>(input, format is "bson");
        var sprites = merge.Create();
        Acquire(sprites);
        return sprites;
    }

    private tk2dSpriteAnimation ReadAnimation(Stream input, string format)
    {
        var animation = CustomAssetUtility.DeserializeObject<tk2dSpriteAnimation>(input, format is "bson");
        Acquire(animation);
        return animation;
    }

    private AnimationAddition ReadAnimationAddition(Stream input, string format)
    {
        var addition = CustomAssetUtility.DeserializeObject<AnimationAddition>(input, format is "bson");
        Acquire(addition);
        return addition;
    }

    private CustomVisualEffect ReadVisualEffect(Stream input, string format)
    {
        var visual = CustomAssetUtility.DeserializeObject<CustomVisualEffect>(input, format is "bson");
        Acquire(visual);
        return visual;
    }

    private TMPro.TMP_FontAsset ReadFont(Stream input, string format)
    {
        var font = CustomAssetUtility.DeserializeObject<TMPro.TMP_FontAsset>(input, format is "bson");
        Acquire(font);
        return font;
    }

    private TMPro.TMP_SpriteAsset ReadEmoji(Stream input, string format)
    {
        var emoji = CustomAssetUtility.DeserializeObject<TMPro.TMP_SpriteAsset>(input, format is "bson");
        Acquire(emoji);
        return emoji;
    }

    #endregion

    #region Asset

    private ExplosionAsset ReadExplosionAsset(Stream input, string format)
    {
        var explosion = CustomAssetUtility.DeserializeObject<ExplosionAsset>(input, format is "bson");
        Acquire(explosion);
        return explosion;
    }

    private DecorAsset ReadDecorAsset(Stream input, string format)
    {
        var decor = CustomAssetUtility.DeserializeObject<DecorAsset>(input, format is "bson");
        Acquire(decor);
        return decor;
    }

    private BreakablePropAsset ReadBreakablePropAsset(Stream input, string format)
    {
        var breakable = CustomAssetUtility.DeserializeObject<BreakablePropAsset>(input, format is "bson");
        Acquire(breakable);
        return breakable;
    }

    private TriggerAsset ReadTriggerAsset(Stream input, string format)
    {
        var trigger = CustomAssetUtility.DeserializeObject<TriggerAsset>(input, format is "bson");
        Acquire(trigger);
        return trigger;
    }

    private MovingObjectAsset ReadMovingObjectAsset(Stream input, string format)
    {
        var moving = CustomAssetUtility.DeserializeObject<MovingObjectAsset>(input, format is "bson");
        Acquire(moving);
        return moving;
    }

    private PhysicObjectAsset ReadPhysicObjectAsset(Stream input, string format)
    {
        var physic = CustomAssetUtility.DeserializeObject<PhysicObjectAsset>(input, format is "bson");
        Acquire(physic);
        return physic;
    }

    private SentryGunAsset ReadSentryGunAsset(Stream input, string format)
    {
        var sentry = CustomAssetUtility.DeserializeObject<SentryGunAsset>(input, format is "bson");
        Acquire(sentry);
        return sentry;
    }

    private HumanAsset ReadHumanAsset(Stream input, string format)
    {
        var human = CustomAssetUtility.DeserializeObject<HumanAsset>(input, format is "bson");
        Acquire(human);
        return human;
    }

    private SpawnPointAsset ReadSpawnPointAsset(Stream input, string format)
    {
        var spawn = CustomAssetUtility.DeserializeObject<SpawnPointAsset>(input, format is "bson");
        Acquire(spawn);
        return spawn;
    }

    private AssetAddition ReadAssetAddition(Stream input, string format)
    {
        var addition = CustomAssetUtility.DeserializeObject<AssetAddition>(input, format is "bson");
        Acquire(addition);
        return addition;
    }

    #endregion

    #region LevelElement

    private Rotorz.Tile.OrientedBrush ReadBrushInfo(Stream input, string format)
    {
        var info = CustomAssetUtility.DeserializeObject<BrushInfo>(input, format is "bson");
        var brush = info.Create();
        Acquire(brush);
        return brush;
    }

    private Rotorz.Tile.OrientedBrush ReadBrushMerge(Stream input, string format)
    {
        var merge = CustomAssetUtility.DeserializeObject<BrushMerge>(input, format is "bson");
        var brush = merge.Create();
        Acquire(brush);
        return brush;
    }

    private Sprite ReadPreview(string name, byte[] input)
    {
        var texture = new Texture2D(0, 0, TextureFormat.RGBA32, false, true);
        texture.LoadImage(input, true);
        texture.name = name;
        var preview = Sprite.Create(
            texture: texture,
            rect: new Rect(0, 0, texture.width, texture.height),
            pivot: Vector2.one / 2.0f);
        preview.name = name;
        Acquire(preview);
        return preview;
    }

    private Sprite ReadPreviewInfo(Stream input, string format)
    {
        var info = CustomAssetUtility.DeserializeObject<PreviewInfo>(input, format is "bson");
        var preview = info.Create();
        Acquire(preview);
        return preview;
    }

    private LevelElement ReadLevelElement(Stream input, string format)
    {
        var element = CustomAssetUtility.DeserializeObject<LevelElement>(input, format is "bson");
        Acquire(element);
        return element;
    }

    private Rotorz.Tile.OrientedBrush MakeBrush(LevelElement element)
    {
        var merge = new BrushMerge(
            source: element.LinkedElement.Element.Brush as Rotorz.Tile.OrientedBrush,
            name: "brush_" + element.name,
            prefab: element.CustomAsset?.Prefab.gameObject ?? element.DecorPrefab);
        var brush = merge.Create();
        Acquire(brush);
        return brush;
    }

    private HookAsset MakeHook(LevelElement element)
    {
        var hook = HookAsset.Invoke(body =>
        {
            var animator = body.GetComponentInChildren<tk2dSpriteAnimator>();
            animator.Library = element.AnimationLibrary;
            animator.DefaultClipId = element.AnimClipId;
            animator.Sprite.SetSprite(element.SpriteCollection, element.SpriteIndex);
        });
        hook.name = element.name + "_hook";
        Acquire(hook);
        return hook;
    }

    #endregion

    private readonly Dictionary<string, Object> _cache = new();

    private void Acquire(Object obj)
    {
        var key = obj.NameAndType();
        if (!_cache.TryAdd(key, obj))
        {
            Logger.LogWarning($"{key} is already acquired");
            return;
        }

        switch (obj)
        {
            case BankAsset bank:
                bank.Load();
                Logger.LogInfo($"Fetch FMODAsset from {bank.Path}");
                break;
            case VisualEffect visual:
                _ = visual.Bind();
                Logger.LogInfo($"Bind VisualEffect {visual.AssetId} - {visual.name}");
                break;
            case LevelElement element:
                _ = element.Bind();
                Logger.LogInfo($"Bind LevelElement {element.AssetId} - {element.Title}");
                break;
            case TMPro.TMP_Asset tmp:
                _ = tmp.Bind();
                Logger.LogInfo($"Bind TMPro.TMP_Asset 0x{tmp.hashCode:x08} - {tmp.name}");
                break;
            case AnimationAddition addition:
                addition.Apply();
                break;
            case AssetAddition addition:
                addition.Apply();
                break;
        }

        CustomAssetUtility.Cache[key] = obj;
        Object.DontDestroyOnLoad(obj);
    }

    private void Release(string key)
    {
        if (!_cache.Remove(key, out var o))
        {
            Logger.LogWarning($"{key} is already released");
            return;
        }

        switch (o)
        {
            case BankAsset bank:
                bank.UnLoad();
                Logger.LogInfo($"Clear FMODAsset from {bank.Path}");
                break;
            case VisualEffect visual:
                visual.Unbind();
                Logger.LogInfo($"Unbind VisualEffect {visual.AssetId} - {visual.name}");
                break;
            case LevelElement element:
                element.Unbind();
                Logger.LogInfo($"Unbind LevelElement {element.AssetId} - {element.Title}");
                break;
            case TMPro.TMP_Asset tmp:
                tmp.Unbind();
                Logger.LogInfo($"Unbind TMPro.TMP_Asset 0x{tmp.hashCode:x08} - {tmp.name}");
                break;
            case AnimationAddition addition:
                addition.Clear();
                break;
            case AssetAddition addition:
                addition.Clear();
                break;
        }

        CustomAssetUtility.Cache.Remove(key);
        Object.Destroy(o);
    }
}