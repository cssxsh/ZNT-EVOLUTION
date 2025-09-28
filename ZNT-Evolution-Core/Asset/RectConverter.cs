using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public class RectConverter : CustomCreationConverter<Rect>
    {
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rect = (Rect)value;
            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue($"{rect.GetType().FullName}, {rect.GetType().Assembly.GetName().Name}");
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

        public override bool CanRead => false;

        public override Rect Create(Type objectType) => Rect.zero;
    }
}