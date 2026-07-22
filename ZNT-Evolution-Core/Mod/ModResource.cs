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
        { Format: "tga" or "png" or "exr" } => 0x0000_0000,
        { Type: "material.merge" } => 0x0000_0001,
        { Type: "sprite.info" or "sprite.merge" } => 0x0000_0002,
        { Type: "animation" } => 0x0000_0003,
        { Type: "animation.addition" } => 0x0000_0004,
        { Type: "visual" } => 0x0000_0005,
        { Type: "explosion" } => 0x0001_0000,
        { Type: "decor" } => 0x0001_0001,
        { Type: "breakable" } => 0x0001_0002,
        { Type: "trigger" } => 0x0001_0003,
        { Type: "moving" } => 0x0001_0004,
        { Type: "physic" } => 0x0001_0005,
        { Type: "sentry" } => 0x0001_0006,
        { Type: "human" } => 0x0001_0007,
        { Type: "spawn" } => 0x0001_0008,
        { Type: "brush.info" or "brush.merge" } => 0x0002_0000,
        { Type: "element" } => 0x0002_0001,
        _ => int.MaxValue
    };

    public int CompareTo(ModResource<T> other) => Order.CompareTo(other.Order);
}