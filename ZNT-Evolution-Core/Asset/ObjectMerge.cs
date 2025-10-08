using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
namespace ZNT.Evolution.Core.Asset;

[JsonObject]
[UsedImplicitly]
internal class ObjectMerge : EvolutionMerge<Object>
{
    [JsonProperty("Fields")]
    public readonly Dictionary<string, string> Fields;

    [JsonConstructor]
    public ObjectMerge(Component source, string name, Dictionary<string, string> fields) : base(name, source)
    {
        Fields = fields;
    }

    public override Object Create()
    {
        var clone = Object.Instantiate(Source);

        clone.name = Name;
        CustomAssetUtility.Merge(clone, Fields);

        Object.DontDestroyOnLoad(clone);
        return clone;
    }
}