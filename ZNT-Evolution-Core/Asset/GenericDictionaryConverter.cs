using System;
using System.Collections;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZNT.Evolution.Core.Asset;

internal class GenericDictionaryConverter : CustomCreationConverter<IGenericDictionary>
{
    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, Traverse.Create(value).Property<IDictionary>("Data").Value);
    }

    public override bool CanRead => true;

    public override IGenericDictionary Create(Type type)
    {
        return AccessTools.CreateInstance(type) as IGenericDictionary;
    }

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        var data = serializer.Deserialize(reader, Traverse.Create(type).Property("Data").GetValueType());
        if (data is null) return null;
        var dictionary = Create(type);
        foreach (var entry in (IDictionary)data)
        {
            var (key, value) = (DictionaryEntry)entry;
            Traverse.Create(dictionary).Method("Add", key, value).GetValue();
        }

        return dictionary;
    }
}