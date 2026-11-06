using JetBrains.Annotations;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[DisallowMultipleComponent]
public class VisualEffectAnimationController : BaseAnimationController
{
    public AnimationSettings animation = new() { PlayAnimation = false };

    public override void Initialize()
    {
        base.Initialize();
        EventHandler.RegisterEndEvent(animation, Despawn);
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        if (animation.OverrideLibrary) Animator.Library = animation.Library;
        Initialize();
    }

    [UsedImplicitly]
    private void OnSpawned()
    {
        Play(animation);
    }

    private void Despawn() => ComponentSingleton<GamePoolManager>.Instance.Despawn(transform);
}