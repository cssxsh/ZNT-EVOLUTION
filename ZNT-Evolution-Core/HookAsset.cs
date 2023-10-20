using System;
using UnityEngine;

namespace ZNT.Evolution.Core
{
    internal class HookAsset : CustomAssetObject
    {
        public Action<GameObject> Action;

        public override void LoadFromAsset(GameObject gameObject) => Action?.Invoke(gameObject);
    }
}