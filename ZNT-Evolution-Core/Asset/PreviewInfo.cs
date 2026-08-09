using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class PreviewInfo : EvolutionInfo<Sprite>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(PreviewInfo));

    [JsonProperty("Texture")]
    public readonly Texture2D Texture;

    [JsonProperty("Rect")]
    public readonly Rect Rect;

    [JsonProperty("Pivot")]
    public readonly Vector2 Pivot;

    [JsonProperty("PixelsPerUnit")]
    public readonly float PixelsPerUnit;

    public PreviewInfo(
        string name,
        Texture2D texture,
        Rect? rect = null,
        Vector2? pivot = null,
        float pixelsPerUnit = 100) : base(name)
    {
        Texture = texture;
        Rect = rect ?? new Rect(Vector2.zero, Vector2.one * 128);
        Pivot = pivot ?? Vector2.one * 0.5f;
        PixelsPerUnit = pixelsPerUnit;
        if (Texture is null) Logger.LogWarning("Source is null");
    }

    public override Sprite Create()
    {
        var impl = Sprite.Create(Texture, Rect, Pivot, PixelsPerUnit);

        impl.name = Name;

        return impl;
    }
}