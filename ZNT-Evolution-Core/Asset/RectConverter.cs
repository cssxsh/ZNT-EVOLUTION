using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

public class RectConverter : JsonConverter
{
    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var rect = (Rect)value;
        writer.WriteStartObject();
        writer.WritePropertyName("$type");
        writer.WriteValue($"{typeof(Rect).FullName}, {typeof(Rect).Assembly.GetName().Name}");
        writer.WritePropertyName("x");
        writer.WriteValue(rect.x);
        writer.WritePropertyName("y");
        writer.WriteValue(rect.y);
        writer.WritePropertyName("width");
        writer.WriteValue(rect.width);
        writer.WritePropertyName("height");
        writer.WriteValue(rect.height);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead => false;

    public override bool CanConvert(Type type) => type == typeof(Rect);
}