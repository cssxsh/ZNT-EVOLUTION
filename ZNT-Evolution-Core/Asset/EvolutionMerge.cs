using Newtonsoft.Json;

// ReSharper disable MemberCanBeProtected.Global
namespace ZNT.Evolution.Core.Asset;

public abstract class EvolutionMerge<T> : EvolutionInfo<T> where T : UnityEngine.Object
{
    [JsonProperty("Source")]
    public readonly T Source;

    protected EvolutionMerge(string name, T source) : base(name)
    {
        Source = source;
    }
}