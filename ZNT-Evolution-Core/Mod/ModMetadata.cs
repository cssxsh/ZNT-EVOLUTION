using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Mod;

// ReSharper disable once ClassNeverInstantiated.Global
[JsonObject]
public record ModMetadata
{
    public string Id;
    public string Name;
    public string Version;
    public Dictionary<string, string> Dependencies;
}