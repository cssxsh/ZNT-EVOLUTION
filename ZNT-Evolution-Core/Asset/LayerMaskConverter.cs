using System;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

public class LayerMaskConverter : JsonConverter
{
    private static readonly Regex FlagRegex = new(@"(\w(?:[\s\w]*\w)?)", RegexOptions.Compiled);

    public override bool CanConvert(Type type) => type == typeof(LayerMask) || type == typeof(Layer);

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var mask = (LayerMask)value;
        if (mask.value is 0x00000000)
        {
            writer.WriteValue(0x00000000);
            return;
        }

        var names =
            from index in Enumerable.Range(0x00, 0x20)
            where (mask.value | (0x01 << index)) == mask.value
            let name = LayerMask.LayerToName(index)
            select name is null or "" ? index.ToString() : name;

        writer.WriteValue(names.Join());
    }

    public override bool CanRead => true;

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        var mask = reader.TokenType switch
        {
            JsonToken.Integer => serializer.Deserialize<int>(reader),
            JsonToken.String => TextToLayerMask(serializer.Deserialize<string>(reader)),
            _ => (int)JToken.Load(reader).ToObject<LayerMask>()
        };
        if (type == typeof(Layer)) return (Layer)mask;
        return (LayerMask)mask;
    }

    [UsedImplicitly]
    public static int TextToLayerMask(string text)
    {
        return FlagRegex.Matches(text).Cast<Match>()
            .Select(match => LayerConverter.TextToLayer(match.Value))
            .Where(layer => layer is not -1)
            .Aggregate(0x00000000, (current, layer) => current | 0x01 << layer);
    }
}