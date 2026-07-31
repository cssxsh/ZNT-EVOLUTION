using Newtonsoft.Json;

// ReSharper disable UnusedMemberInSuper.Global
namespace ZNT.Evolution.Core.Asset;

public abstract class EvolutionAddition<T>(T[] targets) where T : UnityEngine.Object
{
    [JsonProperty("Targets")]
    public readonly T[] Targets = targets;

    public abstract void Apply();
}