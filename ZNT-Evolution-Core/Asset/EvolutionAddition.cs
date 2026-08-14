using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

public abstract class EvolutionAddition<TK, TV> :
    ScriptableObject,
    IReadOnlyCollection<KeyValuePair<TK, TV>>,
    ISerializationCallbackReceiver
{
    public abstract void Push(TK target, TV source);

    public abstract void Apply();

    protected abstract void OnDestroy();

    public abstract IEnumerator<KeyValuePair<TK, TV>> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public abstract int Count { get; }

    public virtual void OnBeforeSerialize()
    {
        // ...
    }

    public virtual void OnAfterDeserialize()
    {
        // ...
    }
}