using System.IO;
using HarmonyLib;
using Newtonsoft.Json.Bson;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Live.Net;

public class BsonDataReader : BsonReader
{
    public BsonDataReader(Stream stream) : base(stream)
    {
        Traverse.Create(this).Field<BinaryReader>("_reader").Value = new FixTypeReader(stream);
    }

    private class FixTypeReader(Stream input) : BinaryReader(input)
    {
        public override sbyte ReadSByte()
        {
            var type = base.ReadSByte();
            return type switch { 17 => 18, 18 => 19, _ => type };
        }
    }
}