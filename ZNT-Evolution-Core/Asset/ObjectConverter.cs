using System;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Asset;

internal class ObjectConverter : CustomCreationConverter<UnityEngine.Object>
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(ObjectConverter));

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (writer.WriteState == WriteState.Start)
        {
            if (value is ISerializationCallbackReceiver receiver) receiver.OnBeforeSerialize();
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
                writer.WriteValue(impl.NameAndType());
                break;
            default:
                throw new NotSupportedException($"write ${value.GetType()}");
        }
    }

    public override bool CanRead => true;

    public override UnityEngine.Object Create(Type type)
    {
        if (typeof(ScriptableObject).IsAssignableFrom(type)) return ScriptableObject.CreateInstance(type);
        if (typeof(Component).IsAssignableFrom(type)) return new GameObject(type.Name).AddComponent(type);
        return AccessTools.CreateInstance(type) as UnityEngine.Object;
    }

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.String)
        {
            var result = base.ReadJson(reader, type, _, serializer) as UnityEngine.Object;
            if (result is ISerializationCallbackReceiver receiver) receiver.OnAfterDeserialize();
            if (result) CustomAssetUtility.Cache[result.NameAndType()] = result;
            if (result) UnityEngine.Object.DontDestroyOnLoad(result);
            return result;
        }

        var key = serializer.Deserialize<string>(reader);
        if (key == null) return null;
        if (type == typeof(Shader)) return Shader.Find(key);
        if (type == typeof(FMODAsset)) return FmodAssetIndex.PathIndex[key];

        if (CustomAssetUtility.Cache.TryGetValue(key, out var value)) return value;
        var name = key.Split(':')[0].Trim();
        var t = key.IndexOf(':') >= 0 ? AccessTools.TypeByName(key.Split(':')[1].Trim()) ?? type : type;
        if (t == typeof(Transform)
            && CustomAssetUtility.TryGetPrefab(name, out var prefab)) return prefab;
        if (t == typeof(GameObject)
            && CustomAssetUtility.TryGetPrefab(name, out var transform)) return transform.gameObject;
        foreach (var asset in Resources.FindObjectsOfTypeAll(t))
        {
            if (asset.name != name) continue;
            CustomAssetUtility.Cache[key] = asset;
            return asset;
        }

        if (type == typeof(CustomAssetObject))
        {
            var lazy = ScriptableObject.CreateInstance<LazyRef>();
            lazy.HierarchyName = key;
            lazy.name = name;
            return lazy;
        }

        Logger.LogError($"NotFound {type.FullName} {{ name: \"{name}\" }}");
        return null;
    }
}