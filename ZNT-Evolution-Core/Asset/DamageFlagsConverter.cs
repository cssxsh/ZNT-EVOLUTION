using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZNT.Evolution.Core.Asset;

internal class DamageFlagsConverter : CustomCreationConverter<DamageType>
{
    public static readonly DamageFlagsConverter Instance = new();

    public static Flags GetDamageFlags(DamageType damage)
    {
        return damage.HasFlag(Flags._)
            ? (Flags)((int)damage & int.MaxValue)
            : (Flags)(0x01 << (int)damage);
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
        return serializer.Deserialize<Flags>(reader) | Flags._;
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
        HolyFire,
        _ = int.MinValue
    }
}