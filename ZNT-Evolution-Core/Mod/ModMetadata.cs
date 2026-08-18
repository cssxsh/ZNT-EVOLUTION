using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using ZNT.Evolution.Core.Asset;

namespace ZNT.Evolution.Core.Mod;

// ReSharper disable once ClassNeverInstantiated.Global
[JsonObject]
public class ModMetadata(
    string id,
    string name,
    string version,
    string link = null,
    Dictionary<string, string> dependencies = null)
{
    public readonly string Id = id;
    public readonly string Name = name;
    public readonly string Version = version;
    public readonly string Link = link;
    public readonly IReadOnlyDictionary<string, string> Dependencies = dependencies ?? new(0);

    public static ModMetadata FromPackage(string path)
    {
        using var package = ZipStorer.Open(path, FileAccess.Read);
        _ = package.ReadCentralDir();
        var entry = package.GetEntry("metadata.json");
        if (entry is null) throw new FileNotFoundException($"metadata in {path}", "metadata.json");
        using var buffer = new MemoryStream();
        package.ExtractFile(entry, buffer);
        buffer.Position = 0;
        var metadata = CustomAssetUtility.DeserializeObject<ModMetadata>(buffer);
        System.Version.Parse(metadata.Version);
        foreach (var (_, version) in metadata.Dependencies) System.Version.Parse(version);
        return metadata;
    }

    public static ModMetadata FromFolder(string path)
    {
        var file = Path.Combine(path, "metadata.json");
        if (!File.Exists(file)) throw new FileNotFoundException($"metadata in {path}", "metadata.json");
        var metadata = CustomAssetUtility.DeserializeObjectFromPath<ModMetadata>(file);
        System.Version.Parse(metadata.Version);
        foreach (var (_, version) in metadata.Dependencies) System.Version.Parse(version);
        return metadata;
    }
}