using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class NameConverter : JsonConverter
    {
        private readonly Type _convert;

        public NameConverter(Type convert)
        {
            _convert = convert;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case tk2dSpriteAnimation animation:
                    writer.WriteValue(animation.name);
                    break;
                case tk2dSpriteCollectionData sprites:
                    writer.WriteValue(sprites.name);
                    break;
                case Transform transform:
                    writer.WriteValue(transform.name);
                    break;
                case FMODAsset fmod:
                    writer.WriteValue(fmod.name);
                    break;
                default:
                    throw new NotSupportedException("write: " + value.GetType());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var name = serializer.Deserialize<String>(reader);
            if (name == null) return null;
            foreach (var asset in Resources.FindObjectsOfTypeAll(objectType))
            {
                if (asset.name == name)
                {
                    return asset;
                }
            }

            throw new KeyNotFoundException(objectType.FullName + "(name: " + name + ")");
        }

        public override bool CanConvert(Type objectType) => _convert == objectType;
    }
}