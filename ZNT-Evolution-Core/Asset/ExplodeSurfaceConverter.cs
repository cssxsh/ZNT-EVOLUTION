using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZNT.Evolution.Core.Asset
{
    internal class ExplodeSurfaceConverter : CustomCreationConverter<PhysicObjectBehaviour.ExplodeSurface>
    {
        public const PhysicObjectBehaviour.ExplodeSurface None = 0x00000000;

        public const PhysicObjectBehaviour.ExplodeSurface Wall = PhysicObjectBehaviour.ExplodeSurface.Wall;

        public const PhysicObjectBehaviour.ExplodeSurface Ground = PhysicObjectBehaviour.ExplodeSurface.Ground;

        public const PhysicObjectBehaviour.ExplodeSurface Ceiling = PhysicObjectBehaviour.ExplodeSurface.Ceiling;

        public const PhysicObjectBehaviour.ExplodeSurface Target = PhysicObjectBehaviour.ExplodeSurface.Target;

        public const PhysicObjectBehaviour.ExplodeSurface Zombie = (PhysicObjectBehaviour.ExplodeSurface)0x00000010;

        public const PhysicObjectBehaviour.ExplodeSurface Climber = (PhysicObjectBehaviour.ExplodeSurface)0x00000020;

        public const PhysicObjectBehaviour.ExplodeSurface Blocker = (PhysicObjectBehaviour.ExplodeSurface)0x00000040;

        public const PhysicObjectBehaviour.ExplodeSurface Tank = (PhysicObjectBehaviour.ExplodeSurface)0x00000080;

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var mask = (PhysicObjectBehaviour.ExplodeSurface)value;
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

        public override PhysicObjectBehaviour.ExplodeSurface Create(Type type) => None;

        public override bool CanRead => true;

        public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
        {
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
                _ => None
            });
        }
    }
}