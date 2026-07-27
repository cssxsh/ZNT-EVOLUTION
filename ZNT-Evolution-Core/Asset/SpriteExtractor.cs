using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Asset;

internal class SpriteExtractor
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(SpriteExtractor));
    private static readonly Dictionary<Texture, Texture2D> Cache = new();
    private readonly Texture2D _output = new(width: 0, height: 0);
    private readonly List<string> _names = [];
    private readonly List<Rect> _regions = [];
    private readonly List<Vector2> _anchors = [];
    private readonly Dictionary<int, tk2dSpriteDefinition.AttachPoint[]> _points = new();
    private readonly int _block;
    private readonly Color32[] _clear;
    private int _row;
    private int _col;

    public SpriteExtractor(int block)
    {
        _block = block;
        if (_block <= 0) throw new System.ArgumentOutOfRangeException(nameof(block));
        _output.Resize(_block * 2048, _block * 2048);
        _clear = new Color32[_block * 96 * _block * 96];
        _row = _col = _block * 24;
    }

    public void Push(tk2dSpriteDefinition definition)
    {
        const float delta = 1f / 1000f;
        var source = MarkReadable(definition.material.mainTexture);
        var index = _names.Count;
        var region = Rect.zero;
        var anchor = Vector2.zero;
        region.x = definition.uvs[0].x * source.width - delta;
        region.y = (1.0f - definition.uvs[2].y) * source.height + delta;
        region.width = definition.uvs[3].x * source.width + delta - region.x;
        region.height = (1.0f - definition.uvs[1].y) * source.height - delta - region.y;
        anchor.x = (0.0f - definition.positions[0].x) / definition.texelSize.x;
        anchor.y = definition.positions[2].y / definition.texelSize.y;
        if (IsRowEnd()) NewRow();
        SetBlock();

        // ReSharper disable InconsistentNaming
        var src_x = (int)System.Math.Round(region.x);
        var src_y = (int)System.Math.Round(region.y);
        var src_w = (int)System.Math.Round(region.width);
        var src_h = (int)System.Math.Round(region.height);
        var offset_x = (int)System.Math.Round(anchor.x);
        var offset_y = (int)System.Math.Round(anchor.y);
        var dst_x = _block * 50 + _col - offset_x;
        var dst_y = _block * 50 + _row - offset_y;
        // ReSharper restore InconsistentNaming

        region.x = dst_x;
        region.y = dst_y;
        anchor.x = offset_x;
        anchor.y = offset_y;
        if (src_w > 0 && src_h > 0)
        {
            region.width = src_w;
            region.height = src_h;
            Graphics.CopyTexture(
                src: source,
                srcElement: 0,
                srcMip: 0,
                srcX: src_x,
                srcY: source.height - src_h - src_y,
                srcWidth: src_w,
                srcHeight: src_h,
                dst: _output,
                dstElement: 0,
                dstMip: 0,
                dstX: dst_x,
                dstY: _output.height - src_h - dst_y);
        }
        else if (src_w > 0 && src_h < 0)
        {
            region.width = System.Math.Abs(src_h);
            region.height = src_w;
            for (var x = 0; x < region.width; x++)
            {
                for (var y = 0; y < region.height; y++)
                {
                    Graphics.CopyTexture(
                        src: source,
                        srcElement: 0,
                        srcMip: 0,
                        srcX: src_x + y,
                        srcY: source.height - src_y + x,
                        srcWidth: 1,
                        srcHeight: 1,
                        dst: _output,
                        dstElement: 0,
                        dstMip: 0,
                        dstX: dst_x + x,
                        dstY: _output.height - src_w - dst_y + y);
                }
            }
        }
        else
        {
            Logger.LogError($"{definition.name} - {region} - {anchor} is bad");
        }

        _names.Add(definition.name);
        _regions.Add(region);
        _anchors.Add(anchor);
        if (definition.attachPoints.Length != 0) _points[index] = definition.attachPoints;
        NewCol();
    }

    private bool IsRowEnd()
    {
        return _col + _block * 100 >= _output.width;
    }

    private void NewRow()
    {
        _col = _block * 24;
        _row += _block * 100;
        if (_row + _block * 100 < _output.height) return;
        var pixels = _output.GetPixels32();
        var h = _output.height;
        _output.Resize(_output.width, h + _block * 2048);
        _output.SetPixels32(0, _block * 2048, _output.width, h, pixels);
        _output.Apply();
    }

    private void NewCol()
    {
        _col += _block * 100;
    }

    private void SetBlock()
    {
        _output.SetPixels32(
            x: _block * 2 + _col,
            y: _output.height - _block * 98 - _row,
            blockWidth: _block * 96,
            blockHeight: _block * 96,
            colors: _clear);
        _output.Apply();
    }

    public static Texture2D MarkReadable(Texture texture)
    {
        if (Cache.TryGetValue(texture, out var cached)) return cached;
        var source = new Texture2D(width: texture.width, height: texture.height)
        {
            name = texture.name,
            filterMode = texture.filterMode,
            wrapMode = texture.wrapMode
        };
        var render = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear);
        Graphics.Blit(texture, render);
        var previous = RenderTexture.active;
        RenderTexture.active = render;
        source.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        source.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(render);
        return Cache[texture] = source;
    }
}