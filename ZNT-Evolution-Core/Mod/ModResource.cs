using System;

namespace ZNT.Evolution.Core.Mod;

internal record ModResource<T> : IComparable<ModResource<T>>
{
    public T File;
    public string Path;
    public string Name;
    public string Type;
    public string Format;

    public int Order => this switch
    {
        { Name: "metadata", Type: "", Format: "json" } => 0x0000_0000,
        { Format: "bank", Type: "strings" } => 0x0001_0000,
        { Format: "bank" } => 0x0001_0001,
        { Format: "tga" or "png" or "exr" } => 0x0002_0000,
        { Type: "material.merge", Format: "json" or "bson" } => 0x0002_0001,
        { Type: "sprite.info" or "sprite.merge", Format: "json" or "bson" } => 0x0002_0002,
        { Type: "animation", Format: "json" or "bson" } => 0x0002_0003,
        { Type: "animation.addition", Format: "json" or "bson" } => 0x0002_0004,
        { Type: "visual", Format: "json" or "bson" } => 0x0002_0005,
        { Type: "explosion", Format: "json" or "bson" } => 0x0003_0001,
        { Type: "decor", Format: "json" or "bson" } => 0x0003_0002,
        { Type: "breakable", Format: "json" or "bson" } => 0x0003_0003,
        { Type: "trigger", Format: "json" or "bson" } => 0x0003_0004,
        { Type: "moving", Format: "json" or "bson" } => 0x0003_0005,
        { Type: "physic", Format: "json" or "bson" } => 0x0003_0006,
        { Type: "sentry", Format: "json" or "bson" } => 0x0003_0007,
        { Type: "human", Format: "json" or "bson" } => 0x0003_0008,
        { Type: "spawn", Format: "json" or "bson" } => 0x0003_0009,
        { Type: "brush.info" or "brush.merge", Format: "json" or "bson" } => 0x0004_0000,
        { Type: "preview.info", Format: "json" or "bson" } => 0x0004_0001,
        { Type: "element", Format: "json" or "bson" } => 0x0004_0002,
        _ => int.MaxValue
    };

    public int CompareTo(ModResource<T> other) => Order.CompareTo(other.Order);
}