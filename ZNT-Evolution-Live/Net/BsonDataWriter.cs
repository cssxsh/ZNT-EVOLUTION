using System.IO;
using HarmonyLib;
using Newtonsoft.Json.Bson;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Live.Net;

public class BsonDataWriter : BsonWriter
{
    public BsonDataWriter(Stream stream) : base(stream)
    {
        Traverse.Create(this).Field("_writer").Field<BinaryWriter>("_writer").Value = new FixTypeWriter(stream);
    }

    private class FixTypeWriter : BinaryWriter
    {
        public FixTypeWriter(Stream output) : base(output)
        {
            // Fix Write(sbyte value) for Newtonsoft.Json.Bson.BsonType
        }

        public override void Write(sbyte value)
        {
            base.Write((sbyte)(value switch { 17 => 16, 18 => 17, 19 => 18, 20 => 18, _ => value }));
        }
    }
}