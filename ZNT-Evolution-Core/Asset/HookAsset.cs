using UnityEngine;
using UnityEngine.Events;

namespace ZNT.Evolution.Core.Asset;

internal class HookAsset : CustomAssetObject
{
    [SerializeField]
    private UnityAction<GameObject> _action;

    public override void LoadFromAsset(GameObject gameObject) => _action?.Invoke(gameObject);

    public static HookAsset Invoke(UnityAction<GameObject> action)
    {
        var hook = CreateInstance<HookAsset>();
        hook._action = action;
        return hook;
    }
}