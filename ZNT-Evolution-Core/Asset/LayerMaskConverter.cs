using System;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

public class LayerMaskConverter : CustomCreationConverter<LayerMask>
{
    private static readonly Regex FlagRegex = new("""(\w(?:[\s\w]*\w)?)""", RegexOptions.Compiled);

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

    public override LayerMask Create(Type type) => 0x00000000;

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        if (reader.TokenType is JsonToken.Integer) return (LayerMask)serializer.Deserialize<int>(reader);
        if (reader.TokenType is not JsonToken.String) return JToken.Load(reader).ToObject<LayerMask>();
        var value = serializer.Deserialize<string>(reader);
        var mask = FlagRegex.Matches(value).Cast<Match>()
            .Select(match => LayerConverter.TextToLayer(match.Value))
            .Where(layer => layer is not -1)
            .Aggregate(0x00000000, (current, layer) => current | 0x01 << layer);
        return (LayerMask)mask;
    }
}