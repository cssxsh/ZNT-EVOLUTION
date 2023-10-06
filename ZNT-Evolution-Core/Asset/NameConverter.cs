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

        public NameConverter(params Type[] exclude) => _exclude = exclude;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case FMODAsset fmod:
                    writer.WriteValue(fmod.path);
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