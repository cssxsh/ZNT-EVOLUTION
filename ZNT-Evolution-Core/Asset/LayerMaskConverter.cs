using System;
using System.Collections.Generic;
using System.Linq;
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
            var mask = (LayerMask)value;
            var names = new List<string>();
            for (var layer = 0x00; layer < 0x20; layer++)
            {
                var name = LayerMask.LayerToName(layer);
                if (string.IsNullOrEmpty(name)) name = layer.ToString();
                if (BitMask.HasAny(mask.value, 0x01 << layer)) names.Add(name);
            }

            writer.WriteValue(names.Join());
        }

        public override bool CanRead => true;

        public override LayerMask Create(Type type) => 0x00000000;

        public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String) return (LayerMask)serializer.Deserialize<Wrapper>(reader).value;
            var names = serializer.Deserialize<string>(reader).Split(',')
                .Select(n => n.Trim()).Where(n => n.Length > 0);
            return (LayerMask)names.Aggregate(0x00000000, (mask, name) =>
            {
                var layer = LayerMask.NameToLayer(name);
                if (layer == -1) layer = int.Parse(name);
                return mask | (0x01 << layer);
            });
        }

        [Serializable]
        private class Wrapper
        {
            public int value;
        }
    }
}