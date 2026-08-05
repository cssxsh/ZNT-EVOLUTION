using System;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Asset;

public class LayerConverter : CustomCreationConverter<int>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(LayerMask));

    private static readonly Regex SpaceRegex = new("""[\s_]+""", RegexOptions.Compiled);

    public static readonly LayerConverter Instance = new();

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var layer = (int)value;
        var name = LayerMask.LayerToName(layer);
        if (string.IsNullOrEmpty(name)) writer.WriteValue(layer);
        else writer.WriteValue(name);
    }

    public override bool CanRead => true;

    public override int Create(Type type) => LayerMask.NameToLayer("Default");

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Integer) return serializer.Deserialize<int>(reader);
        if (reader.TokenType != JsonToken.String) return serializer.Deserialize<LayerMask>(reader).value;
        var text = serializer.Deserialize<string>(reader);
        return TextToLayer(text);
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