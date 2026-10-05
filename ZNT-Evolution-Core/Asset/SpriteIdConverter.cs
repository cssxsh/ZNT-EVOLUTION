using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ZNT.Evolution.Core.Asset;

internal class SpriteIdConverter : JsonConverter
{
    public static readonly SpriteIdConverter Instance = new();

    private static readonly Dictionary<int, string> Map = new();

    [UsedImplicitly]
    public static void Apply(UnityEngine.Object o)
    {
        switch (o)
        {
            case tk2dSpriteAnimation animation:
            {
                if (animation.clips is null) break;
                foreach (var clip in animation.clips)
                {
                    if (clip.frames is null) continue;
                    foreach (var frame in clip.frames)
                    {
                        if (frame.spriteId >= 0 || frame.spriteCollection is null) continue;
                        if (Map.TryGetValue(frame.spriteId, out var name))
                        {
                            frame.spriteId = frame.spriteCollection.GetSpriteIdByName(name, frame.spriteId);
                        }
                    }
                }
            }
                break;
            case LevelElement element:
            {
                if (element.SpriteIndex >= 0 || element.SpriteCollection is null) break;
                if (Map.TryGetValue(element.SpriteIndex, out var name))
                {
                    element.SpriteIndex = element.SpriteCollection.GetSpriteIdByName(name, element.SpriteIndex);
                }
            }
                break;
            case HumanAsset human:
            {
                if (human.SpriteIndex >= 0 || human.SpriteCollection is null) break;
                if (Map.TryGetValue(human.SpriteIndex, out var name))
                {
                    human.SpriteIndex = human.SpriteCollection.GetSpriteIdByName(name, human.SpriteIndex);
                }
            }
                break;
        }
    }

    public override bool CanConvert(Type type) => type == typeof(int);

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }

    public override bool CanRead => true;

    public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
    {
        if (reader.TokenType is JsonToken.Integer) return serializer.Deserialize<int>(reader);
        var name = serializer.Deserialize<string>(reader);
        var key = name.GetHashCode() | int.MinValue;
        // ReSharper disable once DuplicatedSequentialIfBodies
        if (Map.TryAdd(key, name)) return key;
        if (Map.TryGetValue(key, out var old) && old == name) return key;
        throw new Exception($"{name} - 0x{key:X08} has already been used.");
    }
}