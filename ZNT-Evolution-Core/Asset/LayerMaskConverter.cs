using System;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Asset;

public class LayerMaskConverter : CustomCreationConverter<LayerMask>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(LayerMaskConverter));

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var mask = (LayerMask)value;
        if (mask.value == 0x00000000)
        {
            writer.WriteValue(0x00000000);
            return;
        }

        var names =
            from index in Enumerable.Range(0x00, 0x20)
            let name = LayerMask.LayerToName(index)
            select string.IsNullOrEmpty(name) ? index.ToString() : name;

        writer.WriteValue(names.Join());
    }

    public override bool CanRead => true;

    public override LayerMask Create(Type type) => 0x00000000;

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Integer) return (LayerMask)serializer.Deserialize<int>(reader);
        if (reader.TokenType != JsonToken.String) return JToken.Load(reader).ToObject<LayerMask>();
        var names = serializer.Deserialize<string>(reader).Split(',');
        return (LayerMask)names.Select(n => n.Trim()).Aggregate(0x00000000, (mask, name) =>
        {
            var layer = LayerMask.NameToLayer(name);
            if (layer == -1 && !int.TryParse(name, out layer)) Logger.LogError($"Invalid Layer '{name}'");
            return mask | (0x01 << layer);
        });
    }
}