using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class UnityEngineObjectContractResolver : DefaultContractResolver
    {
        public UnityEngineObjectContractResolver() : base(shareCache: true)
        {
            // ...
        }

        protected override JsonObjectContract CreateObjectContract(Type type)
        {
            DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public;
            if (typeof(UnityEngine.Object).IsAssignableFrom(type)) DefaultMembersSearchFlags |= BindingFlags.NonPublic;
            return base.CreateObjectContract(type);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
        {
            var property = base.CreateProperty(member, serialization);
            if (!typeof(UnityEngine.Object).IsAssignableFrom(member.DeclaringType)) return property;
            switch (member)
            {
                case { }
                    when member.IsDefined(typeof(NonSerializedAttribute)):
                case FieldInfo { IsPrivate: true }
                    when !member.IsDefined(typeof(SerializeField)):
                case { Name: nameof(LevelElement.SpriteDefinition) }
                    when typeof(LevelElement).IsAssignableFrom(member.DeclaringType):
                case { Name: nameof(LevelElement.AttachPoints) }
                    when typeof(LevelElement).IsAssignableFrom(member.DeclaringType):
                    property.Ignored = true;
                    break;
                case { }
                    when member.IsDefined(typeof(LayerAttribute)):
                    property.PropertyType = typeof(int);
                    property.MemberConverter = new LayerConverter();
                    property.ValueProvider = new LayerProvider(origin: property.ValueProvider);
                    break;
                case { Name: nameof(UnityEngine.Object.name) }:
                case { Name: nameof(UnityEngine.Object.hideFlags) }:
                    break;
                case PropertyInfo _:
                    property.Readable = false;
                    break;
            }

            return property;
        }

        private class LayerProvider : IValueProvider
        {
            private readonly IValueProvider _origin;

            public LayerProvider(IValueProvider origin) => _origin = origin;

            public void SetValue(object target, object value)
            {
                var layer = (int)value;
                _origin.SetValue(target, _origin.GetValue(target) switch
                {
                    LayerMask _ => (LayerMask)layer,
                    int _ => layer,
                    _ => throw new FormatException("Invalid value")
                });
            }

            public object GetValue(object target)
            {
                return _origin.GetValue(target) switch
                {
                    LayerMask mask => mask.value,
                    int index => index,
                    _ => throw new FormatException("Invalid value")
                };
            }
        }
    }
}