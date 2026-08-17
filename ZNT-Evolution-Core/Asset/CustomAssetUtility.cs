using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
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
            new StringEnumConverter(),
            new LayerMaskConverter(),
            new ColorConverter(),
            new Vector2Converter(),
            new Vector3Converter(),
            new Vector4Converter(),
            new Matrix4x4Converter(),
            new RectConverter(),
            new AnimationCurveConverter()
        }
    });

    // ReSharper disable once InconsistentNaming
    private static Encoding UTF8NoBOM => field ??= new UTF8Encoding(false, true);

    public static string NameAndType(this Object o) => $"{o.name} : {o.GetType()}";

    [UsedImplicitly]
    public static void SerializeObjectToPath(string target, object data)
    {
        using var stream = File.Open(target, FileMode.Create, FileAccess.Write);
        SerializeObject(stream, data, target.EndsWith(".bson"));
    }

    [UsedImplicitly]
    public static void SerializeObject(Stream target, object data, bool bson = false)
    {
        using JsonWriter writer = bson
            ? new BsonDataWriter(target)
            : new JsonTextWriter(new StreamWriter(target, UTF8NoBOM, 1024, true));
        writer.Formatting = Formatting.Indented;
        Serializer.Serialize(writer, data);
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
        return DeserializeObject<T>(stream, source.EndsWith(".bson"));
    }

    [UsedImplicitly]
    public static T DeserializeObject<T>(Stream source, bool bson = false)
    {
        using JsonReader reader = bson
            ? new BsonDataReader(source)
            : new JsonTextReader(new StreamReader(source, Encoding.UTF8, true, 1024, true));
        return Serializer.Deserialize<T>(reader);
    }

    [UsedImplicitly]
    public static T DeserializeObject<T>(JToken token)
    {
        using var json = new JTokenReader(token);
        return Serializer.Deserialize<T>(json);
    }

    internal static IEnumerator LoadBuildIn<T>(UnityAction<T> action)
    {
        using var fs = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("ZNT.Evolution.Core.Resources.index.bundle");
        var create = AssetBundle.LoadFromStreamAsync(fs ?? throw new FileNotFoundException("index.bundle"));
        yield return create;
        var bundle = create.assetBundle;
        var request = bundle.LoadAllAssetsAsync<T>();
        yield return request;
        try
        {
            foreach (var font in request.allAssets.OfType<T>()) action.Invoke(font);
        }
        finally
        {
            bundle.Unload(false);
        }
    }

    internal static IEnumerator LoadPatch<T>(UnityAction<T> action)
    {
        using var fs = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("ZNT.Evolution.Core.Resources.patch.bundle");
        var create = AssetBundle.LoadFromStreamAsync(fs ?? throw new FileNotFoundException("patch.bundle"));
        yield return create;
        var bundle = create.assetBundle;
        var request = bundle.LoadAllAssetsAsync<T>();
        yield return request;
        try
        {
            foreach (var font in request.allAssets.OfType<T>()) action.Invoke(font);
        }
        finally
        {
            bundle.Unload(false);
        }
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

        {
            using var fs = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ZNT.Evolution.Core.Resources.index.bundle");
            var bundle = AssetBundle.LoadFromStream(fs ?? throw new FileNotFoundException("index.bundle"));
            try
            {
                prefab = bundle.LoadAsset<GameObject>($"prefab/{name}")?.transform;
                if (prefab is not null) return true;
            }
            finally
            {
                bundle.Unload(false);
            }
        }

        prefab = null;
        return false;
    }
}