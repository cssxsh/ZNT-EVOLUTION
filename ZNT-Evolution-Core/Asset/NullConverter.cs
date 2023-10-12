using System;
using System.Linq;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset
{
    internal class NullConverter : JsonConverter
    {
        private readonly Type[] _include;

        public NullConverter(params Type[] include) => _include = include;

        public override void WriteJson(JsonWriter writer, object _, JsonSerializer serializer) => writer.WriteNull();

        public override object ReadJson(JsonReader reader, Type objectType, object _, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;

        public override bool CanConvert(Type objectType) => _include.Contains(objectType);
    }
}