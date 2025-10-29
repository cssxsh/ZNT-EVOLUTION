using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

internal class FrameworkInputsConverter : CustomCreationConverter<Framework.Inputs.BaseInput>
{
    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var index = serializer.Converters.IndexOf(this);
        var handling = serializer.TypeNameHandling;
        serializer.Converters.RemoveAt(index);
        serializer.TypeNameHandling = TypeNameHandling.Objects;
        serializer.Serialize(writer, value);
        serializer.Converters.Insert(index, this);
        serializer.TypeNameHandling = handling;
    }

    public override bool CanRead => true;

    public override Framework.Inputs.BaseInput Create(Type type)
    {
        return ScriptableObject.CreateInstance(type) as Framework.Inputs.BaseInput;
    }

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        type = Type.GetType(jObject.Value<string>("$type")) ?? type;
        using var jReader = new JTokenReader(jObject);
        return base.ReadJson(jReader, type, _, serializer);
    }
}