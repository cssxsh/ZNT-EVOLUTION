using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;
using ZNT.Evolution.Core.Effect;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class CustomAssetObjectPatch
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(CustomAssetObject));

    private static DamageType GetDamageType(this Parameters parameters, string key = nameof(DamageType))
    {
        return parameters.ContainsKey(key) ? parameters.GetValue<DamageType>(key) : DamageType.None;
    }

    private static Transform CreatePrefab(this ExplosionAsset explosion, Transform parent = null)
    {
        var explode = Traverse.Create(explosion).Field<bool>("autoExplode");
        var auto = explode.Value;
        explode.Value = false;
        var prefab = ComponentSingleton<GamePoolManager>.Instance.Spawn(explosion.Prefab, parent);
        explosion.LoadFromAsset(prefab.gameObject);
        explode.Value = auto;
        return prefab;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CustomAssetObject), "LoadFromAsset")]
    public static void LoadFromAsset(CustomAssetObject __instance, GameObject gameObject)
    {
        Logger.LogDebug($"LoadFromAsset: {gameObject} {gameObject.transform.position} for {__instance}");
    }

    #region ExplosionAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ExplosionAsset), "LoadFromAsset")]
    public static void LoadFromAsset(ExplosionAsset __instance, GameObject gameObject)
    {
        if (Traverse.Create(__instance).Field<bool>("autoExplode").Value) return;
        gameObject.GetComponentSafe<ExplosionEditor>();
        gameObject.GetComponentSafe<SignalReceiverLinker>();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MineBehaviour), "OnCreate")]
    public static void OnCreate(MineBehaviour __instance)
    {
        var prefab = Traverse.Create(__instance).Field<Transform>("explosionPrefab");
        if (prefab.Value.IsChildOf(__instance.transform)) return;
        var explosion = Traverse.Create(__instance).Field<ExplosionAsset>("explosion").Value;
        prefab.Value = explosion.CreatePrefab(parent: __instance.transform);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MineBehaviour), "Explode")]
    public static bool Explode(MineBehaviour __instance)
    {
        var prefab = Traverse.Create(__instance).Field<Transform>("explosionPrefab").Value;
        if (!prefab.IsChildOf(__instance.transform)) return true;
        Traverse.Create(__instance).Field<Trigger>("trigger").Value.enabled = false;
        prefab.GetComponent<ExplosionEditor>().StartExplosion();
        Traverse.Create(__instance).Field<MineAnimationController>("animation").Value.PlayExplosion();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MineBehaviour), "Destroy")]
    public static void Destroy(MineBehaviour __instance)
    {
        var prefab = Traverse.Create(__instance).Field<Transform>("explosionPrefab").Value;
        ComponentSingleton<GamePoolManager>.Instance.Despawn(prefab);
    }

    #endregion

    #region MovingObjectAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MovingObjectAsset), "LoadFromAsset")]
    public static void LoadFromAsset(MovingObjectAsset __instance, GameObject gameObject)
    {
        var controller = gameObject.GetComponent<MovingObjectAnimationController>();
        if (controller is null) return;
        var orientation = gameObject.GetComponent<ObjectOrientation>().CurrentOrientation;
        var clip = string.Format(__instance.StandAnimation, orientation.ToString().ToLower());
        if (!controller.Animator.AnimationExists(clip)) return;
        var frame = controller.Animator.GetAnimationClip(clip).frames[0];
        controller.Animator.Sprite.SetSprite(frame.spriteCollection, frame.spriteId);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ObjectOrientation), "orientation", MethodType.Setter)]
    public static void SetOrientation(ObjectOrientation __instance, ObjectOrientation.Orientation value)
    {
        var controller = __instance.GetComponent<MovingObjectAnimationController>();
        if (controller is null) return;
        if (!controller.Asset.StandAnimation.Contains('{')) return;
        var clip = string.Format(controller.Asset.StandAnimation, value.ToString().ToLower());
        controller.Animator.Sprite.SetSprite(clip);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MovingObjectAnimationController), "OnStart")]
    public static bool OnStart(MovingObjectAnimationController __instance)
    {
        if (!__instance.Asset.StandAnimation.Contains('{')) return true;
        var orientation = __instance.GetComponent<ObjectOrientation>().CurrentOrientation;
        var clip = string.Format(__instance.Asset.StandAnimation, orientation.ToString().ToLower());
        if (!string.IsNullOrEmpty(clip)) __instance.ForcePlay(clip);
        __instance.GetComponent<SoundEventPlayer>().PlaySound(__instance.Asset.StandSound);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MovingObjectAnimationController), "OnDeactivate")]
    public static bool OnDeactivate(MovingObjectAnimationController __instance)
    {
        if (!__instance.Asset.DisableAnimation.Contains('{')) return true;
        var orientation = __instance.GetComponent<ObjectOrientation>().CurrentOrientation;
        var clip = string.Format(__instance.Asset.DisableAnimation, orientation.ToString().ToLower());
        if (!string.IsNullOrEmpty(clip)) __instance.ForcePlay(clip);
        __instance.GetComponent<SoundEventPlayer>().PlaySound(__instance.Asset.DisableSound);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MovingObjectAnimationController), "OnMove")]
    public static bool OnMove(MovingObjectAnimationController __instance)
    {
        var behaviour = __instance.GetComponent<MovingObjectBehaviour>();
        if (behaviour is null || !behaviour.IsActive) return true;
        if (!__instance.Asset.MoveAnimation.Contains('{')) return true;
        var orientation = __instance.GetComponent<ObjectOrientation>().CurrentOrientation;
        var clip = string.Format(__instance.Asset.MoveAnimation, orientation.ToString().ToLower());
        if (!string.IsNullOrEmpty(clip)) __instance.ForcePlay(clip);
        __instance.GetComponent<SoundEventPlayer>().PlaySound(__instance.Asset.MoveSound);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MovingObjectAnimationController), "OnStop")]
    public static bool OnStop(MovingObjectAnimationController __instance)
    {
        var behaviour = __instance.GetComponent<MovingObjectBehaviour>();
        if (behaviour is null || !behaviour.IsActive) return true;
        if (!__instance.Asset.StopAnimation.Contains('{')) return true;
        var orientation = __instance.GetComponent<ObjectOrientation>().CurrentOrientation;
        var clip = string.Format(__instance.Asset.StopAnimation, orientation.ToString().ToLower());
        if (!string.IsNullOrEmpty(clip)) __instance.ForcePlay(clip);
        __instance.GetComponent<SoundEventPlayer>().Stop();
        __instance.GetComponent<SoundPlayer>().Sound = __instance.Asset.StopSound;
        __instance.GetComponent<SoundPlayer>().Play();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MovingObjectAnimationController), "HitCharacter")]
    public static bool HitCharacter(MovingObjectAnimationController __instance)
    {
        if (!__instance.Asset.HitAnimation.Contains('{')) return true;
        var orientation = __instance.GetComponent<ObjectOrientation>().CurrentOrientation;
        var name = string.Format(__instance.Asset.HitAnimation, orientation.ToString().ToLower());
        if (!string.IsNullOrEmpty(name)) __instance.ForcePlay(name);
        __instance.GetComponent<SoundPlayer>().Sound = __instance.Asset.HitSound;
        __instance.GetComponent<SoundPlayer>().Play();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MovingObjectAnimationController), "OnDestroyed")]
    public static bool OnDestroyed(MovingObjectAnimationController __instance)
    {
        if (!Application.isPlaying) return true;
        if (!__instance.Asset.DestroyAnimation.Contains('{')) return true;
        var orientation = __instance.GetComponent<ObjectOrientation>().CurrentOrientation;
        var name = string.Format(__instance.Asset.DestroyAnimation, orientation.ToString().ToLower());
        if (!string.IsNullOrEmpty(name)) __instance.ForcePlay(name);
        return false;
    }

    #endregion

    #region PhysicObjectAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PhysicObjectBehaviour), "Initialize")]
    public static void Initialize(PhysicObjectBehaviour __instance)
    {
        __instance.DamageTriger.enabled = __instance.DamageCharacterOnTrigger
                                          || __instance.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Zombie)
                                          || __instance.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Climber)
                                          || __instance.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Blocker)
                                          || __instance.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Tank);

        if (__instance.DamageTriger.enabled && __instance.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Target))
        {
            Logger.LogWarning($"{__instance} ExplodeOn is invalid");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PhysicObjectBehaviour), "OnTriggerEnter2D")]
    public static bool OnTriggerEnter2D(PhysicObjectBehaviour __instance, Collider2D other)
    {
        var flag = __instance.DamageCharacterOnTrigger
                   && __instance.TargetLayers.ContainsLayer(other.gameObject.layer);
        if (flag) __instance.SendMessage(methodName: "SendTargetDamage", value: other.gameObject);
        // TODO param by setting
        if (flag && __instance.Physic.GravityScale == 0.0f)
        {
            var physic = __instance.Physic;
            var direction = physic.Body.velocity.normalized;
            var force = direction * physic.StartForce * physic.Body.mass * physic.Collider.friction * -1;
            physic.Body.AddForce(force, ForceMode2D.Impulse);
            if (physic.Body.velocity.normalized != direction) physic.Body.ResetVelocity();
            if (physic.Body.velocity.magnitude <= physic.StartForce * 0.5) __instance.OnDie(null);
        }

        var targets = other.GetComponents<BaseAnimationController>()
            .Aggregate(ExplodeSurfaceConverter.None, (mask, controller) => mask | controller switch
            {
                ZombieAnimationController { enabled: true } => ExplodeSurfaceConverter.Zombie,
                ClimberAnimationController { enabled: true } => ExplodeSurfaceConverter.Climber,
                BlockerAnimationController { enabled: true } => ExplodeSurfaceConverter.Blocker,
                TankAnimationController { enabled: true } => ExplodeSurfaceConverter.Tank,
                _ => ExplodeSurfaceConverter.None
            });
        if (targets == ExplodeSurfaceConverter.None) return false;
        if (__instance.ExplodeOn.HasFlag(targets)) __instance.OnDie(null);
        return false;
    }

    #endregion

    #region HumanAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HumanBehaviour), "Initialize")]
    public static void Initialize(HumanBehaviour __instance)
    {
        foreach (var (key, attachment) in __instance.SharedAsset.Attachments as IDictionary<string, GameObject>)
        {
            switch (key)
            {
                case "moving_attack":
                case "shield_attack":
                case "shield_effect":
                case "attach_laser":
                    continue;
                default:
                    if (attachment is null) continue;
                    if (__instance.transform.Find(key)) continue;
                    Logger.LogDebug($"Spawn {attachment} for {__instance.gameObject} Attachments[\"{key}\"]");
                    // 'OnSpawned' triggered by 'BroadcastMessage'
                    ComponentSingleton<GamePoolManager>.Instance.Spawn(attachment, __instance.transform).name = key;
                    break;
            }
        }

        // ReSharper disable once InvertIf
        if (__instance.SharedAsset.BlockOpponents && !StopperShield.ContainsKey(__instance.Stopper))
        {
            var shield = StopperShield[__instance.Stopper] = ComponentSingleton<GamePoolManager>.Instance
                    .Spawn(InvisibleShield.PoolPrefab().Prefab, __instance.transform)
                    .GetComponent<InvisibleShield>();
            shield.name = nameof(StopperShield);
        }
    }

    internal static GameObject GetRepulse(this Rage __instance)
    {
        var repulse = Traverse.Create(__instance).Field<GameObject>("repulse");
        if (repulse.Value) return repulse.Value;
        return repulse.Value = __instance.transform.Find("Repulse")?.gameObject;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Rage), "Repulsion", MethodType.Setter)]
    public static void SetRepulsion(Rage __instance, ExplosionAsset value)
    {
        var repulse = __instance.GetRepulse();
        if (repulse)
        {
            repulse.GetComponent<ExplosionEditor>().EditorVisibility.CustomName = null;
            repulse.GetComponent<ExplosionEffect>().DespawnOnEnd = true;
            ComponentSingleton<GamePoolManager>.Instance.Despawn(repulse);
        }

        if (value is null) return;
        var prefab = value.CreatePrefab(parent: __instance.transform);
        prefab.name = "Repulse";
        prefab.GetComponent<ExplosionEditor>().EditorVisibility.CustomName = nameof(Rage.Repulsion);
        prefab.GetComponent<ExplosionEffect>().DespawnOnEnd = false;
        Traverse.Create(__instance).Field<GameObject>("repulse").Value = prefab.gameObject;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Rage), "OnHit")]
    public static bool OnHit(Rage __instance, Parameters param)
    {
        if (!__instance.enabled) return false;
        var repulse = __instance.GetRepulse();
        if (repulse is null) return false;
        var flags = DamageFlagsConverter.GetDamageFlags(__instance.DamageType);
        var damage = param.GetDamageType();
        if (flags.All(flag => flag != DamageType.None && flag != damage)) return false;
        var timer = Traverse.Create(__instance).Field<Timer>("refillTimer").Value;
        var hits = Traverse.Create(__instance).Field<int>("currentHitCount").Value;
        if (--hits > 0)
        {
            timer.Start();
            Traverse.Create(__instance).Field<Timer>("refillTimer").Value = timer;
            Traverse.Create(__instance).Field<int>("currentHitCount").Value = hits;
            return false;
        }

        hits = __instance.RefillOnEnraged ? __instance.TargetHitCount : 0;
        timer.Stop();
        Traverse.Create(__instance).Field<Timer>("refillTimer").Value = timer;
        Traverse.Create(__instance).Field<int>("currentHitCount").Value = hits;
        repulse.GetComponent<ExplosionEditor>().StartExplosion();
        Traverse.Create(__instance).Field("events").Field<BoolEvent>("OnRage").Value
            .Invoke(__instance.FreezeOnRage);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Rage), "OnDespawned")]
    public static void OnDespawned(Rage __instance)
    {
        __instance.Repulsion = null;
    }

    private static readonly Dictionary<Stopper, InvisibleShield> StopperShield = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Stopper), "Initialize")]
    public static void Initialize(Stopper __instance, bool block, int maxOpponents)
    {
        var detector = Traverse.Create(__instance).Field<BoxDetection>("detector").Value;
        var effect = detector.gameObject.GetComponentSafe<CharacterAllocationEffect>();
        if (block)
        {
            effect.capacity = maxOpponents;
            effect.StartEffect();
        }
        else
        {
            effect.StopEffect();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Stopper), "SetActive")]
    public static void SetActive(Stopper __instance)
    {
        if (StopperShield.TryGetValue(__instance, out var shield))
        {
            shield.gameObject.SetActive(__instance.enabled);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Stopper), "OnDespawned")]
    public static void OnDespawned(Stopper __instance)
    {
        var detector = Traverse.Create(__instance).Field<BoxDetection>("detector").Value;
        var effect = detector.GetComponent<CharacterAllocationEffect>();
        effect.StopEffect();
        if (StopperShield.Remove(__instance, out var shield))
        {
            ComponentSingleton<GamePoolManager>.Instance.Despawn(shield);
        }
    }

    #endregion

    #region VisualEffect

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EffectManager), "GetEffect")]
    public static void GetEffect(EffectManager __instance, VisualEffect effect, Transform __result)
    {
        if (effect is not CustomVisualEffect custom) return;
        var animator = __result.GetComponentInChildren<SpriteAnimator>();
        if (animator is not null && custom.animation is not null) animator.ForcePlay(custom.animation);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(VisualEffectController), "OnDespawned")]
    public static void OnDespawned(VisualEffectController __instance)
    {
        var prefab = __instance.GetComponent<PoolRetriever>()?.Prefab;
        if (prefab is null) return;
        var animator = __instance.GetComponentInChildren<tk2dSpriteAnimator>();
        // ReSharper disable once InvertIf
        if (animator)
        {
            var origin = prefab.GetComponent<tk2dSpriteAnimator>();
            animator.Library = origin.Library;
            animator.DefaultClipId = origin.DefaultClipId;
        }
    }

    #endregion

    #region Rotorz.Tile.Brush

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Rotorz.Tile.OrientedBrush), "Awake")]
    public static void Awake(Rotorz.Tile.OrientedBrush __instance)
    {
        if (__instance.DefaultOrientation.GetVariation(0) is not GameObject prefab) return;
        if (prefab.GetComponentInChildren<Health>() is { } health) health.EditorVisibility = true;
        if (prefab.TryGetComponent(out OneWayCollider _)) prefab.AddComponent<OneWayEditor>().FixResizeHandles();
        switch (prefab.GetComponent<BaseBehaviour>())
        {
            case MineBehaviour:
                _ = prefab.GetComponentSafe<LayerEditor>();
                _ = prefab.GetComponentSafe<MineTrapEditor>();
                break;
            case PropBehaviour:
                _ = prefab.GetComponentSafe<LayerEditor>();
                break;
            case HumanBehaviour:
                _ = prefab.GetComponentSafe<HumanEditor>();
                break;
        }
    }

    private static void FixResizeHandles(this OneWayEditor prefab)
    {
        var resize = prefab.GetComponent<ResizeHandles>();
        resize.MinBounds = new Bounds(center: Vector2.zero, size: Vector2.one * 0.6f);
        resize.RoundToNearest = 1f / 4f;
        resize.Bounds = new Bounds(center: Vector2.zero, size: Vector2.one);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(OneWayCollider), "Start")]
    private static IEnumerator Start(IEnumerator __result, OneWayCollider __instance)
    {
        if (__instance.GetComponent<OneWayEditor>()) yield break;
        yield return __result;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StairBehaviour), "OnAwake")]
    private static void OnAwake(StairBehaviour __instance)
    {
        _ = __instance.gameObject.GetComponentSafe<StairEditor>();
    }

    #endregion
}