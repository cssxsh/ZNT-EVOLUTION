using System;
using System.Linq;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZNT.Evolution.Core.Asset;

internal class DamageFlagsConverter : CustomCreationConverter<DamageType>
{
    public static DamageType[] GetDamageFlags(DamageType damage)
    {
        if (!damage.HasFlag((DamageType)int.MinValue)) return new[] { damage };
        return Enum.GetValues(typeof(DamageType)).Cast<DamageType>()
            .Where(type => damage.HasFlag((DamageType)(0x01 << (int)type)))
            .ToArray();
    }

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(GetDamageFlags((DamageType)value).Join());
    }

    public override bool CanRead => true;

    public override DamageType Create(Type objectType) => DamageType.None;

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        var flags = serializer.Deserialize<string>(reader);
        if (!flags.Contains(',')) return Enum.Parse(type, flags, true);
        return (DamageType)flags.Split(',').Aggregate(int.MinValue, (mask, flag) =>
        {
            Enum.TryParse<DamageType>(flag.Trim(), true, out var damage);
            return mask | (0x01 << (int)damage);
        });
    }
}