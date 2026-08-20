using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZNT.Evolution.Core.Asset;

internal class DamageFlagsConverter : CustomCreationConverter<DamageType>
{
    public static readonly DamageFlagsConverter Instance = new();

    public static Flags GetDamageFlags(DamageType damage)
    {
        var value = (int)damage;
        return (value & int.MinValue) is 0
            ? (Flags)(0x01 << value)
            : (Flags)(value & int.MaxValue);
    }

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(GetDamageFlags((DamageType)value));
    }

    public override bool CanRead => true;

    public override DamageType Create(Type objectType) => DamageType.None;

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        return (DamageType)((int)serializer.Deserialize<Flags>(reader) | int.MinValue);
    }

    [Flags]
    public enum Flags
    {
        None,
        Fall,
        Bite,
        Gun,
        Rifle,
        Shotgun,
        Melee,
        Sword,
        Canon,
        Explosion,
        Spikes,
        Fire,
        Electricity,
        Laser,
        Sentry,
        Contamination,
        Sacrifice,
        Acid,
        Radioactivity,
        Ripped,
        Plasma,
        Squashed,
        TankDash,
        MachineGun,
        Crawler,
        Tank,
        Boomer,
        Spit,
        HolyFire
    }
}