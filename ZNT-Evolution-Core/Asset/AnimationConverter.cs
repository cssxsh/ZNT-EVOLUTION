using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class AnimationConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new Wrapper((tk2dSpriteAnimation)value));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object _, JsonSerializer serializer)
        {
            var wrapper = serializer.Deserialize<Wrapper>(reader);
            var impl = new GameObject(wrapper.Name + "(Clone)", typeof(tk2dSpriteAnimation))
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            var animation = impl.GetComponent<tk2dSpriteAnimation>();
            animation.clips = wrapper.Clips;
            animation.InitializeClipCache();

            return animation;
        }

        public override bool CanConvert(Type objectType) => typeof(tk2dSpriteAnimation) == objectType;

        [Serializable]
        internal class Wrapper
        {
            [JsonProperty("name")] public readonly string Name;

            [JsonProperty("clips")] public readonly tk2dSpriteAnimationClip[] Clips;

            [JsonConstructor]
            public Wrapper(string name, tk2dSpriteAnimationClip[] clips)
            {
                Name = name;
                Clips = clips;
            }

            public Wrapper(tk2dSpriteAnimation source)
            {
                Name = source.name;
                Clips = source.clips;
            }
        }
    }
}