using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class NameConverter : JsonConverter
    {
        private readonly Type[] _exclude;

        public NameConverter(params Type[] exclude)
        {
            _exclude = exclude;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case tk2dSpriteAnimation animation:
                    writer.WriteValue($"{animation.name} : {value.GetType()}");
                    break;
                case tk2dSpriteCollectionData sprites:
                    writer.WriteValue($"{sprites.name} : {value.GetType()}");
                    break;
                case GameObject impl:
                    writer.WriteValue($"{impl.name} : {value.GetType()}");
                    break;
                case Transform transform:
                    writer.WriteValue($"{transform.name} : {value.GetType()}");
                    break;
                case CustomAsset asset:
                    writer.WriteValue($"{asset.name} : {value.GetType()}");
                    break;
                default:
                    throw new NotSupportedException($"write ${value.GetType()}");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object _, JsonSerializer serializer)
        {
            var key = serializer.Deserialize<string>(reader);
            if (key == null) return null;
            var name = key.Split(':')[0].Trim();
            foreach (var asset in Resources.FindObjectsOfTypeAll(objectType))
            {
                if (asset.name == name)
                {
                    return asset;
                }
            }

            throw new KeyNotFoundException($"{objectType.FullName}(name: {name})");
        }

        public override bool CanConvert(Type objectType) =>
            !_exclude.Contains(objectType) && objectType.IsSubclassOf(typeof(UnityEngine.Object));
    }
}