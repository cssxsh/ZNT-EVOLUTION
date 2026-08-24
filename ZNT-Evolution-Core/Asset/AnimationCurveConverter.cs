using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

internal class AnimationCurveConverter : CustomCreationConverter<AnimationCurve>
{
    private static readonly Regex LinearRegex = new(
        @"^\(([+-]?\d*\.?\d+), ([+-]?\d*\.?\d+)\) - \(([+-]?\d*\.?\d+), ([+-]?\d*\.?\d+)\)$",
        RegexOptions.Compiled);

    private static readonly Regex CurveRegex = new(
        @"^\(([+-]?\d*\.?\d+), ([+-]?\d*\.?\d+), ([+-]?\d*\.?\d+)\) ~ \(([+-]?\d*\.?\d+), ([+-]?\d*\.?\d+), ([+-]?\d*\.?\d+)\)$",
        RegexOptions.Compiled);

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var curve = value as AnimationCurve;
        switch (curve)
        {
            case null:
                writer.WriteNull();
                break;
            case { keys.Length: 2, preWrapMode: WrapMode.ClampForever, postWrapMode: WrapMode.ClampForever }
                when curve.keys.All(frame => frame.weightedMode is WeightedMode.None):
            {
                var x1 = curve.keys[0].time;
                var y1 = curve.keys[0].value;
                var k1 = curve.keys[0].outTangent;
                var x2 = curve.keys[1].time;
                var y2 = curve.keys[1].value;
                var k2 = curve.keys[1].inTangent;
                var k = (y2 - y1) / (x2 - x1);
                writer.WriteValue(Mathf.Approximately(k, k1) && Mathf.Approximately(k, k2)
                    ? $"({x1}, {y1}) - ({x2}, {y2})"
                    : $"({x1}, {y1}, {k1}) ~ ({x2}, {y2}, {k2})");
            }
                break;
            default:
                serializer.Serialize(writer, new JObject
                {
                    [nameof(AnimationCurve.keys)] = curve.keys.Aggregate(new JArray(), (keys, frame) =>
                    {
                        keys.Add(new JObject
                        {
                            [nameof(Keyframe.time)] = frame.time,
                            [nameof(Keyframe.value)] = frame.value,
                            [nameof(Keyframe.inTangent)] = frame.inTangent,
                            [nameof(Keyframe.outTangent)] = frame.outTangent,
                            [nameof(Keyframe.inWeight)] = frame.inWeight,
                            [nameof(Keyframe.outWeight)] = frame.outWeight,
                            [nameof(Keyframe.weightedMode)] = frame.weightedMode.ToString()
                        });
                        return keys;
                    }),
                    [nameof(AnimationCurve.preWrapMode)] = curve.preWrapMode.ToString(),
                    [nameof(AnimationCurve.postWrapMode)] = curve.postWrapMode.ToString()
                });
                break;
        }
    }

    public override bool CanRead => true;

    public override AnimationCurve Create(Type type) => new();

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        if (reader.TokenType is not JsonToken.String) return base.ReadJson(reader, type, _, serializer);
        return TextToAnimationCurve(serializer.Deserialize<string>(reader));
    }

    [UsedImplicitly]
    internal static AnimationCurve TextToAnimationCurve(string text)
    {
        // ReSharper disable once InvertIf
        if (LinearRegex.Match(text) is { Success: true } a)
        {
            var x1 = float.Parse(a.Groups[1].Value);
            var y1 = float.Parse(a.Groups[2].Value);
            var x2 = float.Parse(a.Groups[3].Value);
            var y2 = float.Parse(a.Groups[4].Value);
            var linear = AnimationCurve.Linear(x1, y1, x2, y2);
            return linear;
        }

        // ReSharper disable once InvertIf
        if (CurveRegex.Match(text) is { Success: true } b)
        {
            var x1 = float.Parse(b.Groups[1].Value);
            var y1 = float.Parse(b.Groups[2].Value);
            var k1 = float.Parse(b.Groups[3].Value);
            var x2 = float.Parse(b.Groups[4].Value);
            var y2 = float.Parse(b.Groups[5].Value);
            var k2 = float.Parse(b.Groups[6].Value);
            var curve = AnimationCurve.EaseInOut(x1, y1, x2, y2);
            curve.keys[0].outTangent = k1;
            curve.keys[1].inTangent = k2;
            return curve;
        }

        throw new FormatException(text);
    }
}