using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

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

    public static void Merge(Object o, IDictionary<string, string> fields)
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

    // internal static SpriteInfo FetchSpriteInfo(this tk2dSpriteCollectionData sprites)
    // {
    //     var material = sprites.material ?? sprites.materials[0];
    //     var texture = sprites.textures[0] ?? material.mainTexture;
    //     var names = new string[sprites.spriteDefinitions.Length];
    //     var regions = new Rect[sprites.spriteDefinitions.Length];
    //     var anchors = new Vector2[sprites.spriteDefinitions.Length];
    //     var points = new Dictionary<int, tk2dSpriteDefinition.AttachPoint[]>();
    //     const float t = 1f / 1000f;
    //     for (var i = 0; i < sprites.spriteDefinitions.Length; i++)
    //     {
    //         var definition = sprites.spriteDefinitions[i];
    //         names[i] = definition.name;
    //         regions[i].x = definition.uvs[0].x * texture.width - t;
    //         regions[i].y = (1.0f - definition.uvs[2].y) * texture.height + t;
    //         regions[i].width = definition.uvs[3].x * texture.width + t - regions[i].x;
    //         regions[i].height = (1.0f - definition.uvs[1].y) * texture.height - t - regions[i].y;
    //         anchors[i].x = (0.0f - definition.positions[0].x) / definition.texelSize.x;
    //         anchors[i].y = definition.positions[2].y / definition.texelSize.y;
    //         if (definition.attachPoints.Length == 0) continue;
    //         points[i] = definition.attachPoints;
    //     }
    //
    //     return new SpriteInfo(
    //         orthoSize: 1.0f / sprites.invOrthoSize,
    //         targetHeight: 2.0f * sprites.halfTargetHeight,
    //         names: names,
    //         regions: regions,
    //         anchors: anchors,
    //         points: points,
    //         name: sprites.name,
    //         material: material
    //     );
    // }
}