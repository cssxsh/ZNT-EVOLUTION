using System;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Asset;

internal class LazyAsset : CustomAssetObject
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(LazyAsset));

    [JsonIgnore]
    [UsedImplicitly]
    public MemberInfo Member;

    public override void LoadFromAsset(GameObject gameObject) => throw new AssetException(HierarchyName);

    public void Apply()
    {
        SceneLoader.BeforeLoadScene -= Apply;
        try
        {
            var origin = CustomAssetUtility.DeserializeObject<CustomAssetObject>(HierarchyName);
            foreach (var o in CustomAssetUtility.Cache.Values.Where(Member.ReflectedType!.IsInstanceOfType))
            {
                if (this == Member.GetMemberValue(o) as LazyAsset) Member.SetMemberValue(o, origin);
            }
        }
        catch (Exception e)
        {
            Logger.LogWarning(e);
        }
        finally
        {
            Destroy(this);
        }
    }

    public class MemberConverter(MemberInfo member) : CustomCreationConverter<LazyAsset>
    {
        public override LazyAsset Create(Type type) => CreateInstance<LazyAsset>();

        public override object ReadJson(JsonReader reader, Type type, object _, JsonSerializer serializer)
        {
            var key = serializer.Deserialize<string>(reader);
            if (CustomAssetUtility.Cache.TryGetValue(key, out var value)) return value;
            var lazy = CreateInstance<LazyAsset>();
            lazy.name = member.Name;
            lazy.Member = member;
            lazy.HierarchyName = key;
            SceneLoader.BeforeLoadScene += lazy.Apply;
            return lazy;
        }
    }
}