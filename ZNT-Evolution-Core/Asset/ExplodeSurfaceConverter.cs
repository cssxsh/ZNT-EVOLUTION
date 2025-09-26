using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ExplodeSurface = PhysicObjectBehaviour.ExplodeSurface;

namespace ZNT.Evolution.Core.Asset
{
    internal class ExplodeSurfaceConverter : CustomCreationConverter<ExplodeSurface>
    {
        public const ExplodeSurface None = 0x00000000;

        public const ExplodeSurface Wall = ExplodeSurface.Wall;

        public const ExplodeSurface Ground = ExplodeSurface.Ground;

        public const ExplodeSurface Ceiling = ExplodeSurface.Ceiling;

        public const ExplodeSurface Target = ExplodeSurface.Target;

        public const ExplodeSurface Zombie = (ExplodeSurface)0x00000010;

        public const ExplodeSurface Climber = (ExplodeSurface)0x00000020;

        public const ExplodeSurface Blocker = (ExplodeSurface)0x00000040;

        public const ExplodeSurface Tank = (ExplodeSurface)0x00000080;

        public const ExplodeSurface IgnoreHuman = Zombie | Climber | Blocker | Tank;

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var mask = (ExplodeSurface)value;
            if (mask == None) writer.WriteValue((int)None);
            var flags = new List<string>();
            if (mask.HasFlag(Wall)) flags.Add(nameof(Wall));
            if (mask.HasFlag(Ground)) flags.Add(nameof(Ground));
            if (mask.HasFlag(Ceiling)) flags.Add(nameof(Ceiling));
            if (mask.HasFlag(Target)) flags.Add(nameof(Target));
            if (mask.HasFlag(Zombie)) flags.Add(nameof(Zombie));
            if (mask.HasFlag(Climber)) flags.Add(nameof(Climber));
            if (mask.HasFlag(Blocker)) flags.Add(nameof(Blocker));
            if (mask.HasFlag(Tank)) flags.Add(nameof(Tank));
            writer.WriteValue(flags.Join());
        }

        public override bool CanRead => true;

        public override ExplodeSurface Create(Type type) => None;

        public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer) return (ExplodeSurface)serializer.Deserialize<int>(reader);
            var flags = serializer.Deserialize<string>(reader).Split(',', ' ');
            return flags.Aggregate(None, (mask, flag) => mask | flag switch
            {
                nameof(Wall) => Wall,
                nameof(Ground) => Ground,
                nameof(Ceiling) => Ceiling,
                nameof(Target) => Target,
                nameof(Zombie) => Zombie,
                nameof(Climber) => Climber,
                nameof(Blocker) => Blocker,
                nameof(Tank) => Tank,
                nameof(IgnoreHuman) => IgnoreHuman,
                _ => None
            });
        }
    }
}