using System;
using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class UnityEngineObjectConverter : CustomCreationConverter<UnityEngine.Object>
    {
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer.WriteState == WriteState.Start)
            {
                Traverse.Create(serializer)
                    .Field("_serializerWriter")
                    .Method("SerializeObject", new[]
                    {
                        typeof(JsonWriter),
                        typeof(object),
                        typeof(JsonObjectContract),
                        typeof(JsonProperty),
                        typeof(JsonContract)
                    })
                    .GetValue(writer, value, serializer.ContractResolver.ResolveContract(value.GetType()), null, null);
                return;
            }

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

        public override bool CanRead => true;

        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();

        public override UnityEngine.Object Create(Type type)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                return ScriptableObject.CreateInstance(type);
            }

            if (typeof(Component).IsAssignableFrom(type))
            {
                return new GameObject(type.Name).AddComponent(type);
            }

            return type.GetConstructor(Array.Empty<Type>())?.Invoke(null) as UnityEngine.Object;
        }

        public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                var result = base.ReadJson(reader, type, _, serializer) as UnityEngine.Object;
                if (result) Cache[result.name] = result;
                return result;
            }

            var key = serializer.Deserialize<string>(reader);
            if (key == null) return null;
            if (type == typeof(Shader)) return Shader.Find(key);
            if (type == typeof(FMODAsset)) return FmodAssetIndex.PathIndex[key];

            var name = key.Split(':')[0].Trim();
            if (Cache.TryGetValue(key, out var value) && type.IsInstanceOfType(value)) return value;
            if (type == typeof(GameObject)) return GameObject.Find(name);
            if (typeof(Component).IsAssignableFrom(type) && GameObject.Find(name) is { } o) return o.GetComponent(type);
            foreach (var asset in Resources.FindObjectsOfTypeAll(type))
            {
                if (asset.name != name) continue;
                Cache[key] = asset;
                return asset;
            }

            throw new KeyNotFoundException(message: $"{type.FullName}(name: {name})");
        }
    }
}