using System;

namespace ZNT.Evolution.Core.Asset;

[Serializable]
public class DamageDictionary<T> : UnityDictionary<DamageType, T>
{
    protected override void OnDeserialize() => SetComparer(DamageTypeComparer.Instance);
}