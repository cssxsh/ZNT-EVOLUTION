using System;
using System.Collections.Generic;
using System.Linq;
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

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization serialization)
    {
        // ReSharper disable once InvertIf
        if (typeof(ParticleSystem) == type)
        {
            var tNativeNameAttribute =
                Type.GetType("UnityEngine.Bindings.NativeNameAttribute, UnityEngine.SharedInternalsModule")!;
            var source = new JsonPropertyCollection(type);
            foreach (var member in GetSerializableMembers(type).OfType<PropertyInfo>())
            {
                switch (member)
                {
                    case { PropertyType.IsNested: true }:
                    {
                        foreach (var info in GetSerializableMembers(member.PropertyType).OfType<PropertyInfo>())
                        {
                            var property = base.CreateProperty(info, serialization);
                            property.PropertyName = $"{member.Name}.{info.Name}";
                            property.ValueProvider = new ModuleValueProvider(member, info);
                            property.Ignored = !property.Writable || info.IsDefined(typeof(ObsoleteAttribute));
                            property.Readable = !info.IsDefined(tNativeNameAttribute);
                            source.AddProperty(property);
                        }
                    }
                        break;
                    case not null when typeof(ParticleSystem) == member.DeclaringType:
                    case { Name: nameof(UnityEngine.Object.name) }:
                    case { Name: nameof(UnityEngine.Object.hideFlags) }:
                    {
                        var property = base.CreateProperty(member, serialization);
                        property.Ignored = !property.Writable || member.IsDefined(typeof(ObsoleteAttribute));
                        source.AddProperty(property);
                    }
                        break;
                    default:
                    {
                        var property = base.CreateProperty(member, serialization);
                        property.Readable = false;
                        source.AddProperty(property);
                    }
                        break;
                }
            }

            return source;
        }

        return base.CreateProperties(type, serialization);
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
    {
        var property = base.CreateProperty(member, serialization);
        if (!IsSerializable(member.DeclaringType)) return property;
        switch (member)
        {
            case not null when member.IsDefined(typeof(NonSerializedAttribute)):
            case FieldInfo { IsPrivate: true } when !(member.IsDefined(typeof(SerializeField)) ||
                                                      member.IsDefined(typeof(JsonPropertyAttribute))):
            case { Name: nameof(LevelElement.SpriteDefinition) } when typeof(LevelElement) == member.DeclaringType:
                property.Ignored = true;
                break;
            case { Name: nameof(LevelElement.SpriteName) } when typeof(LevelElement) == member.DeclaringType:
                property.Readable = false;
                break;
            case { Name: nameof(LevelElement.AttachPoints) } when typeof(LevelElement) == member.DeclaringType:
                property.DefaultValue ??= new List<AttachPoint>();
                property.DefaultValueHandling = DefaultValueHandling.Populate;
                break;
            case { Name: nameof(HumanAsset.RageDamageType) } when typeof(HumanAsset) == member.DeclaringType:
                property.Converter = property.MemberConverter = DamageFlagsConverter.Instance;
                break;
            case { Name: nameof(HumanAsset.RiseAsset) } when typeof(HumanAsset) == member.DeclaringType:
                property.Converter = property.MemberConverter = new LazyAsset.MemberConverter(member);
                break;
            case not null when member.IsDefined(typeof(LayerAttribute)):
                property.Converter = property.MemberConverter = LayerConverter.Instance;
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

    private class ModuleValueProvider(PropertyInfo module, PropertyInfo member) : IValueProvider
    {
        public void SetValue(object target, object value) => member.SetValue(module.GetValue(target), value);

        public object GetValue(object target) => member.GetValue(module.GetValue(target));
    }
}