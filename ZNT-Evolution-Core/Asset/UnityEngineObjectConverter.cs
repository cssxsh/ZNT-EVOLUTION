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

        public override UnityEngine.Object Create(Type objectType)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(objectType))
            {
                return ScriptableObject.CreateInstance(objectType);
            }

            if (typeof(Component).IsAssignableFrom(objectType))
            {
                return new GameObject(objectType.Name).AddComponent(objectType);
            }

            return objectType.GetConstructor(Array.Empty<Type>())?.Invoke(null) as UnityEngine.Object;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object _, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String) return base.ReadJson(reader, objectType, _, serializer);
            var key = serializer.Deserialize<string>(reader);
            if (key == null) return null;
            if (objectType == typeof(Shader)) return Shader.Find(key);
            if (objectType == typeof(FMODAsset)) return FmodAssetIndex.PathIndex[key];

            var name = key.Split(':')[0].Trim();
            if (objectType == typeof(GameObject) && GameObject.Find(name) is { } body) return body;
            foreach (var asset in Resources.FindObjectsOfTypeAll(objectType))
            {
                if (asset.name != name) continue;
                return asset;
            }

            throw new KeyNotFoundException(message: $"{objectType.FullName}(name: {name})");
        }
    }
}