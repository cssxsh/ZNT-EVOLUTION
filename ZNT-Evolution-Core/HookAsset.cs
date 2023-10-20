using System;
using UnityEngine;

namespace ZNT.Evolution.Core
{
    internal class HookAsset : CustomAssetObject
    {
        private Action<GameObject> _action;

        public override void LoadFromAsset(GameObject gameObject) => _action?.Invoke(gameObject);

        public static HookAsset Invoke(Action<GameObject> action)
        {
            var hook = CreateInstance<HookAsset>();
            hook._action = action;
            return hook;
        }
    }
}