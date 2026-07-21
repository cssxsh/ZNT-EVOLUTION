using Newtonsoft.Json;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace ZNT.Evolution.Core.Asset;

public abstract class EvolutionInfo<T>(string name) where T : UnityEngine.Object
{
    [JsonProperty("Name")]
    public readonly string Name = name;

    public abstract T Create();
}