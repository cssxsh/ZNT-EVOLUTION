using System;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ZNT.Evolution.Core.Editor;

namespace ZNT.Evolution.Core.Asset;

public class CustomVisualEffect : VisualEffect
{
    public AnimationSettings animation;

    public ParticleConfig particles = new();

    [NonSerialized]
    internal Transform RuntimePrefab;

    [Serializable]
    public class ParticleConfig : UnityDictionary<string, JObject>
    {
        protected override void OnDeserialize()
        {
            // ...
        }
    }

    private void OnDestroy() => Destroy(RuntimePrefab);

    public void Populate(Transform prefab)
    {
        if (prefab.TryGetComponent(out PoolRetriever retriever)) Destroy(retriever);
        if (prefab.TryGetComponent(out AnimationDespawner despawn)) Destroy(despawn);
        if (prefab.TryGetComponent(out SpriteAnimator animator))
        {
            animator.Animator.playAutomatically = false;
            var controller = animator.gameObject.GetComponentSafe<VisualEffectAnimationController>();
            controller.SetAnimator(animator);
            controller.animation = animation;
        }

        foreach (var (key, value) in particles ??= new ParticleConfig())
        {
            var system = key is "."
                ? prefab.GetComponent<ParticleSystem>()
                : prefab.Find(key)?.GetComponent<ParticleSystem>();
            if (system is null) continue;
            using var reader = new JTokenReader(value);
            CustomAssetUtility.Serializer.Populate(reader, system);
        }
    }
}