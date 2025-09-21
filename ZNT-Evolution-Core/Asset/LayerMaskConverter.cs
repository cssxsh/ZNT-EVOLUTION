using System;
using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public class LayerMaskConverter : CustomCreationConverter<LayerMask>
    {
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var mask = (int)(LayerMask)value;
            var names = new List<string>();
            for (var layer = 0x00; layer < 0x20; layer++)
            {
                var name = LayerMask.LayerToName(layer);
                if (string.IsNullOrEmpty(name)) name = layer.ToString();
                if (BitMask.HasAny(mask, 0x01 << layer)) names.Add(name);
            }

            writer.WriteValue(names.Join());
        }

        public override bool CanRead => true;

        public override LayerMask Create(Type objectType) => 0x00000000;

        public override object ReadJson(JsonReader reader, Type objectType, object _, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject) return base.ReadJson(reader, objectType, _, serializer);
            var mask = 0;
            foreach (var name in serializer.Deserialize<string>(reader).Split(','))
            {
                var layer = LayerMask.NameToLayer(name.Trim());
                if (layer == -1) layer = int.Parse(name.Trim());
                mask |= 0x01 << layer;
            }

            return (LayerMask)mask;
        }
    }
}