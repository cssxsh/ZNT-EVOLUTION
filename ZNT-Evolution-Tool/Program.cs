using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZNT.Evolution.Tool;

internal static class Program
{
    public static void Main(string[] args)
    {
        switch (args)
        {
            case []:
                break;
            case { Length: 1 } when File.Exists(args[0]):
            {
                var file = args[0];
                switch (Path.GetExtension(file))
                {
                    case ".psp":
                    {
                        using var fs = File.OpenRead(file);
                        using var sr = new StreamReader(fs);
                        using var reader = new JsonTextReader(sr);
                        var psp = JObject.Load(reader);
                        GameData.PixelStudioToFiles(psp, Path.GetDirectoryName(file) ?? ".");
                    }
                        break;
                    case ".psx":
                    {
                        using var fs = File.OpenRead(file);
                        using var zlib = new ZLibStream(fs, CompressionMode.Decompress);
                        using var sr = new StreamReader(zlib);
                        using var reader = new JsonTextReader(sr);
                        var psp = JObject.Load(reader);
                        GameData.PixelStudioToFiles(psp, Path.GetDirectoryName(file) ?? ".");
                    }
                        break;
                }
            }
                break;
        }
    }
}