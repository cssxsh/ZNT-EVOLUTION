using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public class LayerConverter : CustomCreationConverter<int>
    {
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
            var name = serializer.Deserialize<string>(reader);
            return LayerMask.NameToLayer(name);
        }
    }
}