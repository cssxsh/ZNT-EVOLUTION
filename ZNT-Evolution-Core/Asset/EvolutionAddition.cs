using Newtonsoft.Json;

// ReSharper disable UnusedMemberInSuper.Global
namespace ZNT.Evolution.Core.Asset
{
    public abstract class EvolutionAddition<T> where T : UnityEngine.Object
    {
        [JsonProperty("Targets")] public readonly T[] Targets;

        protected EvolutionAddition(T[] targets)
        {
            Targets = targets;
        }

        public abstract void Apply();
    }
}