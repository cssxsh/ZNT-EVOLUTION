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
            ? (Flags)(0x01 << value >> 0x01)
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
        None = 0x00000000,
        Fall = 0x00000001,
        Bite = 0x00000002,
        Gun = 0x00000004,
        Rifle = 0x00000008,
        Shotgun = 0x00000010,
        Melee = 0x00000020,
        Sword = 0x00000040,
        Canon = 0x00000080,
        Explosion = 0x00000100,
        Spikes = 0x00000200,
        Fire = 0x00000400,
        Electricity = 0x00000800,
        Laser = 0x00001000,
        Sentry = 0x00002000,
        Contamination = 0x00004000,
        Sacrifice = 0x00008000,
        Acid = 0x00010000,
        Radioactivity = 0x00020000,
        Ripped = 0x00040000,
        Plasma = 0x00080000,
        Squashed = 0x00100000,
        TankDash = 0x00200000,
        MachineGun = 0x00400000,
        Crawler = 0x00800000,
        Tank = 0x01000000,
        Boomer = 0x02000000,
        Spit = 0x04000000,
        HolyFire = 0x08000000
    }
}