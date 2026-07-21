using Newtonsoft.Json;

// ReSharper disable MemberCanBeProtected.Global
namespace ZNT.Evolution.Core.Asset;

public abstract class EvolutionMerge<T>(string name, T source) : EvolutionInfo<T>(name) where T : UnityEngine.Object
{
    [JsonProperty("Source")]
    public readonly T Source = source;
}