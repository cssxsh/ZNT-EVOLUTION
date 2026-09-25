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

    [UsedImplicitly]
    private void OnSpawned()
    {
        Initialize();
        Play(animation);
    }

    private void Despawn() => ComponentSingleton<GamePoolManager>.Instance.Despawn(transform);
}