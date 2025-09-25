using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public static class CustomAssetUtility
    {
        internal static readonly Dictionary<string, Object> Cache = new Dictionary<string, Object>();

        private static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new UnityEngineObjectContractResolver(),
            Converters =
            {
                new UnityEngineObjectConverter(),
                new StringEnumConverter(),
                new LayerMaskConverter(),
                new ColorConverter(),
                new Vector2Converter(),
                new Vector3Converter(),
                new Vector4Converter(),
                new Matrix4x4Converter()
            }
        };

        public static string NameAndType(this Object o) => $"{o.name} : {o.GetType()}";

        public static void SerializeObjectToPath(string target, object data)
        {
            var serializer = JsonSerializer.Create(SerializerSettings);
            SaveObjectToPath(serializer, data, target);
        }

        private static void SaveObjectToPath(JsonSerializer serializer, object data, string path)
        {
            using var writer = new StreamWriter(path);
            using var json = new JsonTextWriter(writer);
            json.Formatting = Formatting.Indented;
            serializer.Serialize(json, data);
        }

        public static T DeserializeObjectFromPath<T>(string source)
        {
            var serializer = JsonSerializer.Create(SerializerSettings);
            return LoadObjectFromPath<T>(serializer, source);
        }

        public static T DeserializeObjectFromTextAsset<T>(TextAsset asset)
        {
            var serializer = JsonSerializer.Create(SerializerSettings);
            return LoadObjectFromTextAsset<T>(serializer, asset);
        }

        private static T LoadObjectFromPath<T>(JsonSerializer jsonSerializer, string path)
        {
            using var reader = new StreamReader(path);
            using var json = new JsonTextReader(reader);
            return jsonSerializer.Deserialize<T>(json);
        }

        private static T LoadObjectFromTextAsset<T>(JsonSerializer jsonSerializer, TextAsset asset)
        {
            using var reader = new StringReader(asset.text);
            using var json = new JsonTextReader(reader);
            return jsonSerializer.Deserialize<T>(json);
        }
    }
}