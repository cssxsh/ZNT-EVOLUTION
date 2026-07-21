using System.Collections.Generic;

namespace ZNT.Evolution.Core.Mod;

public class ModContext(string path, ModMetadata metadata)
{
    public readonly string Path = path;

    public readonly ModMetadata Metadata = metadata;
}