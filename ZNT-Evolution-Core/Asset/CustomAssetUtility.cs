using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Events;

namespace ZNT.Evolution.Core.Asset;

public static class CustomAssetUtility
{
    internal static readonly Dictionary<string, Object> Cache = new();

    private static JsonSerializerSettings SerializerSettings => new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Auto,
        ContractResolver = new UnityEngineObjectContractResolver(),
        Converters =
        {
            new FrameworkInputsConverter(),
            new UnityEngineObjectConverter(),
            new ExplodeSurfaceConverter(),
            new StringEnumConverter(),
            new LayerMaskConverter(),
            new ColorConverter(),
            new Vector2Converter(),
            new Vector3Converter(),
            new Vector4Converter(),
            new Matrix4x4Converter(),
            new RectConverter()
        }
    };

    public static string NameAndType(this Object o) => $"{o.name} : {o.GetType()}";

    public static void SerializeObjectToPath(string target, object data)
    {
        var serializer = JsonSerializer.Create(SerializerSettings);
        using var writer = new StreamWriter(target);
        using var json = new JsonTextWriter(writer);
        json.Formatting = Formatting.Indented;
        serializer.Serialize(json, data);
    }

    public static void SerializeObjectToTextAsset(out TextAsset asset, object data)
    {
        var serializer = JsonSerializer.Create(SerializerSettings);
        using var writer = new StringWriter();
        using var json = new JsonTextWriter(writer);
        serializer.Serialize(json, data);
        asset = new TextAsset(writer.ToString());
    }

    public static T DeserializeObjectFromPath<T>(string source)
    {
        var serializer = JsonSerializer.Create(SerializerSettings);
        using var reader = new StreamReader(source);
        using var json = new JsonTextReader(reader);
        return serializer.Deserialize<T>(json);
    }

    public static T DeserializeObjectFromTextAsset<T>(TextAsset asset)
    {
        var serializer = JsonSerializer.Create(SerializerSettings);
        using var reader = new StringReader(asset.text);
        using var json = new JsonTextReader(reader);
        return serializer.Deserialize<T>(json);
    }

    internal static void Merge(Object o, IDictionary<string, string> fields)
    {
        var serializer = JsonSerializer.Create(SerializerSettings);
        foreach (var (path, text) in fields)
        {
            var field = path.Split('.').Aggregate(Traverse.Create(o), (t, name) => t.Field(name));
            using var reader = new StringReader(text);
            using var json = new JsonTextReader(reader);
            var value = serializer.Deserialize(json, field.GetValueType());
            field.SetValue(value);
        }
    }

    internal static IEnumerator LoadBuildIn<T>(UnityAction<T[]> action)
    {
        var assembly = typeof(CustomAssetUtility).Assembly;
        using var fs = assembly.GetManifestResourceStream("ZNT.Evolution.Core.Resources.index.bundle")
                       ?? throw new FileNotFoundException("index.bundle");
        var create = AssetBundle.LoadFromStreamAsync(fs);
        yield return create;
        var bundle = create.assetBundle;
        var path = "all";
        if (typeof(CustomAsset).IsAssignableFrom(typeof(T))) path = "asset";
        if (typeof(tk2dSpriteCollectionData) == typeof(T)) path = "tk2d";
        if (typeof(tk2dSpriteAnimation) == typeof(T)) path = "tk2d";
        var request = bundle.LoadAssetAsync(path);
        yield return request;
        var source = ((I2.Loc.LanguageSourceAsset)request.asset).SourceData;
        var assets = source.Assets.OfType<T>().ToArray();
        bundle.Unload(true);
        action.Invoke(assets);
    }
}