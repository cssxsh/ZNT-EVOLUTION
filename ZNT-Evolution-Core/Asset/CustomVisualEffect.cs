using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

public class CustomVisualEffect : VisualEffect, ISerializationCallbackReceiver
{
    public AnimationSettings animation;

    public void OnBeforeSerialize()
    {
        // ...
    }

    public void OnAfterDeserialize()
    {
        Traverse.Create(this).Field<Transform>("prefab").Value = Instantiate(Prefab);
        DontDestroyOnLoad(Prefab.gameObject);
        Prefab.name = Prefab.name.Replace("(Clone)", $"({name})");
        Prefab.gameObject.SetActive(false);
        var pool = Prefab.GetComponent<PoolRetriever>();
        if (pool) Destroy(pool);
        var animator = Prefab.GetComponent<SpriteAnimator>();
        if (animator) animator.Animator.playAutomatically &= !(animation?.PlayAnimation ?? false);
        Prefab.gameObject.SetActive(true);
    }
}