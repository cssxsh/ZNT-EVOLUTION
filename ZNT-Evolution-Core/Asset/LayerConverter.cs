using System;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Asset;

public class LayerConverter : JsonConverter
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(LayerMask));

    private static readonly Regex SpaceRegex = new(@"[\s_]+", RegexOptions.Compiled);

    public static readonly LayerConverter Instance = new();

    public override bool CanConvert(Type type) => type == typeof(int) || type == typeof(LayerMask);

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var layer = (int)value;
        var name = LayerMask.LayerToName(layer);
        if (name is null or "") writer.WriteValue(layer);
        else writer.WriteValue(name);
    }

    public override bool CanRead => true;

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        var layer = reader.TokenType switch
        {
            JsonToken.Integer => serializer.Deserialize<int>(reader),
            JsonToken.String => TextToLayer(serializer.Deserialize<string>(reader)),
            _ => (int)JToken.Load(reader).ToObject<LayerMask>()
        };
        if (type == typeof(LayerMask)) return (LayerMask)layer;
        return layer;
    }

    internal static int TextToLayer(string text)
    {
        var layer = LayerMask.NameToLayer(text);
        if (layer is not -1) return layer;
        var sample = SpaceRegex.Replace(text, "");
        for (var i = 0x00; i < 0x20; i++)
        {
            if (sample == i.ToString()) return i;
            var name = SpaceRegex.Replace(LayerMask.LayerToName(i), "");
            if (name is "") continue;
            if (name.Equals(sample, StringComparison.OrdinalIgnoreCase)) return i;
        }

        Logger.LogError($"Invalid Layer '{text}'");
        return -1;
    }
}