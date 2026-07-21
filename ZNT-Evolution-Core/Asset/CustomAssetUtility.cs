using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ZNT.Evolution.Core.Asset;

public static class CustomAssetUtility
{
    internal static readonly Dictionary<string, Object> Cache = new();

    private static readonly JsonSerializer Serializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Auto,
        ContractResolver = new ObjectContractResolver(),
        Converters =
        {
            new FrameworkInputsConverter(),
            new GenericDictionaryConverter(),
            new ObjectConverter(),
            new ExplodeSurfaceConverter(),
            new DamageFlagsConverter(),
            new StringEnumConverter(),
            new LayerMaskConverter(),
            new ColorConverter(),
            new Vector2Converter(),
            new Vector3Converter(),
            new Vector4Converter(),
            new Matrix4x4Converter(),
            new RectConverter()
        }
    });

    public static string NameAndType(this Object o) => $"{o.name} : {o.GetType()}";

    [UsedImplicitly]
    public static void SerializeObjectToPath(string target, object data)
    {
        using var stream = File.OpenWrite(target);
        if (target.EndsWith(".bson")) SerializeObjectToBson(stream, data);
        else SerializeObjectToJson(stream, data);
    }

    [UsedImplicitly]
    public static void SerializeObjectToBson(Stream target, object data)
    {
        using var bson = new BsonDataWriter(target);
        Serializer.Serialize(bson, data);
    }

    [UsedImplicitly]
    public static void SerializeObjectToJson(Stream target, object data)
    {
        using var writer = new StreamWriter(target, Encoding.UTF8, 1024, true);
        using var json = new JsonTextWriter(writer);
        json.Formatting = Formatting.Indented;
        Serializer.Serialize(json, data);
    }

    [UsedImplicitly]
    public static void SerializeObject(out JToken token, object data)
    {
        using var json = new JTokenWriter();
        Serializer.Serialize(json, data);
        token = json.Token;
    }

    [UsedImplicitly]
    public static T DeserializeObjectFromPath<T>(string source)
    {
        using var stream = File.OpenRead(source);
        return source.EndsWith(".bson")
            ? DeserializeObjectFromBson<T>(stream)
            : DeserializeObjectFromJson<T>(stream);
    }

    [UsedImplicitly]
    public static T DeserializeObjectFromBson<T>(Stream source)
    {
        using var bson = new BsonDataReader(source);
        return Serializer.Deserialize<T>(bson);
    }

    [UsedImplicitly]
    public static T DeserializeObjectFromJson<T>(Stream source)
    {
        using var reader = new StreamReader(source, Encoding.UTF8, true, 1024, true);
        using var json = new JsonTextReader(reader);
        return Serializer.Deserialize<T>(json);
    }

    [UsedImplicitly]
    public static T DeserializeObject<T>(JToken token)
    {
        using var json = new JTokenReader(token);
        return Serializer.Deserialize<T>(json);
    }

    internal static void Merge(Object o, IDictionary<string, JToken> fields)
    {
        foreach (var (path, token) in fields)
        {
            var field = path.Split('.').Aggregate(Traverse.Create(o), (t, name) => t.Field(name));
            using var json = new JTokenReader(token);
            var value = Serializer.Deserialize(json, field.GetValueType());
            field.SetValue(value);
        }
    }

    internal static IEnumerator LoadBuildIn<T>(UnityAction<T> action)
    {
        using var fs = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("ZNT.Evolution.Core.Resources.index.bundle");
        var create = AssetBundle.LoadFromStreamAsync(fs ?? throw new FileNotFoundException("index.bundle"));
        yield return create;
        var bundle = create.assetBundle;
        var path = "all";
        if (typeof(CustomAsset).IsAssignableFrom(typeof(T))) path = "asset";
        if (typeof(tk2dSpriteCollectionData) == typeof(T)) path = "tk2d";
        if (typeof(tk2dSpriteAnimation) == typeof(T)) path = "tk2d";
        var request = bundle.LoadAssetAsync(path);
        yield return request;
        var source = ((I2.Loc.LanguageSourceAsset)request.asset).SourceData;
        foreach (var asset in source.Assets.OfType<T>()) action.Invoke(asset);
        bundle.Unload(true);
    }

    public static bool TryGetPrefab(string name, out Transform prefab)
    {
        foreach (var (_, pool) in PathologicalGames.PoolManager.Pools)
        {
            if (pool.prefabs.TryGetValue(name, out prefab)) return true;
        }

        foreach (var pool in Resources.LoadAll<PoolSettingsAsset>(""))
        {
            foreach (var info in pool.Prefabs)
            {
                prefab = info.Prefab;
                if (info.Prefab.name == name) return true;
            }
        }

        prefab = null;
        return false;
    }
}