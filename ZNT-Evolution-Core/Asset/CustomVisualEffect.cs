using System;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ZNT.Evolution.Core.Editor;

namespace ZNT.Evolution.Core.Asset;

public class CustomVisualEffect : VisualEffect, ISerializationCallbackReceiver
{
    public AnimationSettings animation;

    public ParticleConfig particles = new();

    [NonSerialized]
    private Transform OriginPrefab;

    [Serializable]
    public class ParticleConfig : UnityDictionary<string, JObject>
    {
        protected override void OnDeserialize()
        {
            // ...
        }
    }

    public void OnBeforeSerialize()
    {
        // ...
    }

    public void OnAfterDeserialize()
    {
        if (Prefab is null || OriginPrefab is not null) return;
        this.SetPrefab(Instantiate(OriginPrefab = Prefab));
        DontDestroyOnLoad(Prefab.gameObject);
        Prefab.name = name;
        Prefab.gameObject.SetActive(false);
        if (Prefab.TryGetComponent(out PoolRetriever retriever)) Destroy(retriever);
        if (Prefab.TryGetComponent(out AnimationDespawner despawn)) Destroy(despawn);
        Populate(Prefab);
        Prefab.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (Prefab is null || OriginPrefab is null) return;
        Destroy(Prefab);
    }

    public void Populate(Transform prefab)
    {
        if (Prefab.TryGetComponent(out SpriteAnimator animator))
        {
            animator.Animator.playAutomatically = false;
            var controller = animator.gameObject.GetComponentSafe<VisualEffectAnimationController>();
            controller.SetAnimator(animator);
            controller.animation = animation;
        }

        foreach (var (key, value) in particles)
        {
            var system = prefab.Find(key)?.GetComponent<ParticleSystem>();
            if (system is null) continue;
            using var reader = new JTokenReader(value);
            CustomAssetUtility.Serializer.Populate(reader, system);
        }
    }
}