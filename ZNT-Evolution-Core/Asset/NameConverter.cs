using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class NameConverter : JsonConverter
    {
        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case FMODAsset fmod:
                    writer.WriteValue(fmod.path);
                    break;
                case Shader shader:
                    writer.WriteValue(shader.name);
                    break;
                case UnityEngine.Object impl:
                    writer.WriteValue($"{impl.name} : {value.GetType()}");
                    break;
                default:
                    throw new NotSupportedException($"write ${value.GetType()}");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object _, JsonSerializer serializer)
        {
            var key = serializer.Deserialize<string>(reader);
            if (key == null) return null;
            if (objectType == typeof(Shader)) return Shader.Find(key);
            if (objectType == typeof(FMODAsset))
            {
                FmodAssetIndex.PathIndex.TryGetValue(key, out var asset);
                if (asset == null) throw new KeyNotFoundException(message: $"Not Found FMODAsset from '{key}'");
                return asset;
            }

            var name = key.Split(':')[0].Trim();
            Cache.TryGetValue(name, out var impl);

            if (objectType.IsInstanceOfType(impl)) return impl;

            if (objectType == typeof(GameObject) && GameObject.Find(name) is { } body)
            {
                Cache[name] = body;
                return body;
            }

            foreach (var asset in Resources.FindObjectsOfTypeAll(objectType))
            {
                if (asset.name != name) continue;
                Cache[name] = asset;
                return asset;
            }

            throw new KeyNotFoundException(message: $"{objectType.FullName}(name: {name})");
        }

        public override bool CanConvert(Type objectType) => typeof(UnityEngine.Object).IsAssignableFrom(objectType);
    }
}