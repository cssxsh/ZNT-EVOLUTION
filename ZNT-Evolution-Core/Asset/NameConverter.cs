﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class NameConverter : JsonConverter
    {
        private readonly Type[] _exclude;
        
        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();

        public NameConverter(params Type[] exclude) => _exclude = exclude;

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
            if (objectType == typeof(FMODAsset)) return FmodAssetIndex.PathIndex[key];
            if (objectType == typeof(Shader)) return Shader.Find(key);
            
            var name = key.Split(':')[0].Trim();
            if (Cache.TryGetValue(name, out var impl) && objectType.IsInstanceOfType(impl)) return impl;
            foreach (var asset in Resources.FindObjectsOfTypeAll(objectType))
            {
                if (asset.name == name)
                {
                    Cache[name] = asset;
                    return asset;
                }
            }

            throw new KeyNotFoundException($"{objectType.FullName}(name: {name})");
        }

        public override bool CanConvert(Type objectType) =>
            !_exclude.Contains(objectType) && objectType.IsSubclassOf(typeof(UnityEngine.Object));
    }
}