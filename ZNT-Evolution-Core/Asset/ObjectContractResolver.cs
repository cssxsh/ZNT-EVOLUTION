using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

internal class ObjectContractResolver() : DefaultContractResolver(shareCache: true)
{
    protected override JsonObjectContract CreateObjectContract(Type type)
    {
        DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public;
        if (IsSerializable(type)) DefaultMembersSearchFlags |= BindingFlags.NonPublic;
        return base.CreateObjectContract(type);
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
    {
        var property = base.CreateProperty(member, serialization);
        if (!IsSerializable(member.DeclaringType)) return property;
        switch (member)
        {
            case not null
                when member.IsDefined(typeof(NonSerializedAttribute)):
            case FieldInfo { IsPrivate: true }
                when !(member.IsDefined(typeof(SerializeField)) || member.IsDefined(typeof(JsonPropertyAttribute))):
            case { Name: nameof(LevelElement.SpriteDefinition) } when typeof(LevelElement) == member.DeclaringType:
                property.Ignored = true;
                break;
            case { Name: nameof(LevelElement.AttachPoints) } when typeof(LevelElement) == member.DeclaringType:
                property.DefaultValue ??= new List<AttachPoint>();
                property.ShouldSerialize = points => points is List<AttachPoint> { Count: > 0 };
                property.DefaultValueHandling = DefaultValueHandling.Populate;
                break;
            case { Name: nameof(HumanAsset.RageDamageType) } when typeof(HumanAsset) == member.DeclaringType:
                property.Converter = property.MemberConverter = DamageFlagsConverter.Instance;
                break;
            case { Name: nameof(HumanAsset.RiseAsset) } when typeof(HumanAsset) == member.DeclaringType:
                property.Converter = property.MemberConverter = new LazyAsset.MemberConverter(member);
                break;
            case not null
                when member.IsDefined(typeof(LayerAttribute)):
                property.PropertyType = typeof(int);
                property.Converter = property.MemberConverter = LayerConverter.Instance;
                property.ValueProvider = new LayerProvider(origin: property.ValueProvider);
                break;
            case { Name: nameof(UnityEngine.Object.name) }:
            case { Name: nameof(UnityEngine.Object.hideFlags) }:
                break;
            case PropertyInfo:
                property.Readable = false;
                break;
        }

        return property;
    }

    private static bool IsSerializable(Type type)
    {
        return type.IsDefined(typeof(SerializableAttribute)) || typeof(UnityEngine.Object).IsAssignableFrom(type);
    }

    private class LayerProvider(IValueProvider origin) : IValueProvider
    {
        public void SetValue(object target, object value)
        {
            var layer = (int)value;
            origin.SetValue(target, origin.GetValue(target) switch
            {
                LayerMask => (LayerMask)layer,
                int => layer,
                _ => throw new FormatException("Invalid value")
            });
        }

        public object GetValue(object target)
        {
            return origin.GetValue(target) switch
            {
                LayerMask mask => mask.value,
                int index => index,
                _ => throw new FormatException("Invalid value")
            };
        }
    }
}