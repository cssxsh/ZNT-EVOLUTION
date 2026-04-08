using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;
using ZNT.Evolution.Core.Effect;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class CustomAssetObjectPatch
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(CustomAssetObject));

    private static DamageType GetDamageType(this Parameters parameters, string key = nameof(DamageType))
    {
        return parameters.ContainsKey(key) ? parameters.GetValue<DamageType>(key) : DamageType.None;
    }

    private static Transform CreateComponent(this ExplosionAsset explosion, Transform parent)
    {
        var prefab = ComponentSingleton<GamePoolManager>.Instance.Spawn(explosion.Prefab, parent);
        var explode = Traverse.Create(explosion).Field<bool>("autoExplode");
        var auto = explode.Value;
        try
        {
            explode.Value = false;
            explosion.LoadFromAsset(prefab.gameObject);
            return prefab;
        }
        finally
        {
            explode.Value = auto;
        }
    }

    private static void Despawn(this AnimationDespawner despawn, AnimationSettings animation)
    {
        if (animation == null) return;
        Traverse.Create(despawn).Field<AnimationEventHandler>("eventHandler").Value
            .RegisterEndEvent(animation, (Action)Delegate.CreateDelegate(typeof(Action), despawn, "Despawn"));
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
        if (gameObject.transform.parent is null)
        {
            gameObject.GetComponentSafe<SignalReceiverLinker>();
        }
        else
        {
            UnityEngine.Object.Destroy(gameObject.GetComponent<SignalLinkerGui>());
            UnityEngine.Object.Destroy(gameObject.GetComponent<SignalReceiverLinker>());
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MineBehaviour), "OnCreate")]
    public static void OnCreate(MineBehaviour __instance)
    {
        var prefab = Traverse.Create(__instance).Field<Transform>("explosionPrefab").Value;
        if (prefab is not null && prefab.IsChildOf(__instance.transform)) return;
        var explosion = Traverse.Create(__instance).Field<ExplosionAsset>("explosion").Value;
        var explode = explosion.CreateComponent(parent: __instance.transform);
        Traverse.Create(__instance).Field<Transform>("explosionPrefab").Value = explode;
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
        Traverse.Create(__instance).Field<SoundEventPlayer>("soundEventPlayer").Value
            .PlaySound(__instance.Asset.StandSound);
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
        Traverse.Create(__instance).Field<SoundEventPlayer>("soundEventPlayer").Value
            .PlaySound(__instance.Asset.DisableSound);
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
        Traverse.Create(__instance).Field<SoundEventPlayer>("soundEventPlayer").Value
            .PlaySound(__instance.Asset.MoveSound);
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
        Traverse.Create(__instance).Field<SoundEventPlayer>("soundEventPlayer").Value.Stop();
        Traverse.Create(__instance).Field<SoundPlayer>("soundPlayer").Value.Sound = __instance.Asset.StopSound;
        Traverse.Create(__instance).Field<SoundPlayer>("soundPlayer").Value.Play();
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
        Traverse.Create(__instance).Field<SoundPlayer>("soundPlayer").Value.Sound = __instance.Asset.HitSound;
        Traverse.Create(__instance).Field<SoundPlayer>("soundPlayer").Value.Play();
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
    [HarmonyPatch(typeof(PhysicObjectAsset), "LoadFromAsset")]
    public static void LoadFromAsset(PhysicObjectAsset __instance, GameObject gameObject)
    {
        var behaviour = gameObject.GetComponent<PhysicObjectBehaviour>();
        if (behaviour.Physic.StartDirection.IsZero()
            && behaviour.Physic.StartForce != 0) Logger.LogWarning($"{__instance} StartDirection is zero");
        behaviour.DamageTriger.enabled = behaviour.DamageCharacterOnTrigger
                                         || behaviour.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Zombie)
                                         || behaviour.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Climber)
                                         || behaviour.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Blocker)
                                         || behaviour.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Tank);

        if (behaviour.DamageTriger.enabled && behaviour.ExplodeOn.HasFlag(ExplodeSurfaceConverter.Target))
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

        var targets = other.GetComponent<Character>()?.AnimationController switch
        {
            ZombieAnimationController => ExplodeSurfaceConverter.Zombie,
            ClimberAnimationController => ExplodeSurfaceConverter.Climber,
            BlockerAnimationController => ExplodeSurfaceConverter.Blocker,
            TankAnimationController => ExplodeSurfaceConverter.Tank,
            _ => ExplodeSurfaceConverter.None
        };
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
        if (__instance.SharedAsset.CharacterType == CharacterType.Cultist
            && !CultistBuff.ContainsKey(__instance.Character))
        {
            var effect = CultistBuff[__instance.Character] = ComponentSingleton<GamePoolManager>.Instance
                .Spawn(SphereBuffEffect.PoolPrefab().Prefab, __instance.Character.transform)
                .GetComponent<SphereBuffEffect>();
            effect.name = nameof(CultistBuff);
        }

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
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HumanBehaviour), "OnDespawned")]
    public static void OnDespawned(HumanBehaviour __instance)
    {
        if (CultistBuff.Remove(__instance.Character, out var effect))
        {
            ComponentSingleton<GamePoolManager>.Instance.Despawn(effect);
        }

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
                    Logger.LogDebug($"Despawn {attachment} for {__instance.gameObject} Attachments[\"{key}\"]");
                    // 'OnDespawned' triggered by 'BroadcastMessage'
                    ComponentSingleton<GamePoolManager>.Instance.Despawn(__instance.transform.Find(key));
                    break;
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Rage), "Repulsion", MethodType.Setter)]
    public static void SetRepulsion(Rage __instance, ExplosionAsset value)
    {
        var repulse = Traverse.Create(__instance).Field<GameObject>("repulse").Value;
        if (repulse)
        {
            repulse.GetComponent<ExplosionEditor>().EditorVisibility.CustomName = null;
            repulse.GetComponent<ExplosionEffect>().DespawnOnEnd = true;
            ComponentSingleton<GamePoolManager>.Instance.Despawn(repulse);
        }

        if (value is null) return;
        var explode = value.CreateComponent(parent: __instance.transform);
        explode.name = "Repulse";
        explode.GetComponent<ExplosionEditor>().EditorVisibility.CustomName = nameof(Rage.Repulsion);
        explode.GetComponent<ExplosionEffect>().DespawnOnEnd = false;
        Traverse.Create(__instance).Field<GameObject>("repulse").Value = explode.gameObject;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Rage), "OnHit")]
    public static bool OnHit(Rage __instance, Parameters param)
    {
        if (!__instance.enabled) return false;
        var repulse = Traverse.Create(__instance).Field<GameObject>("repulse").Value;
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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Rage), "OnDespawned")]
    public static void OnDespawned(Rage __instance)
    {
        __instance.Repulsion = null;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Stopper), "Initialize")]
    public static void Initialize(Stopper __instance, bool block, int maxOpponents)
    {
        var detector = Traverse.Create(__instance).Field<BoxDetection>("detector").Value;
        var effect = detector.gameObject.GetComponentSafe<CharacterAllocationEffect>();
        effect.capacity = block ? maxOpponents : 0;
        if (block) effect.StartEffect();
        else effect.StopEffect();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Stopper), "SetActive")]
    public static void SetActive(Stopper __instance)
    {
        var mover = __instance.GetComponent<Moveable>();
        if (mover is null) return;
        mover.UpdateIsGrounded();
        mover.Body.isKinematic = mover.IsGrounded && __instance.enabled;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Stopper), "OnDespawned")]
    public static void OnDespawned(Stopper __instance)
    {
        var detector = Traverse.Create(__instance).Field<BoxDetection>("detector").Value;
        var effect = detector.GetComponent<CharacterAllocationEffect>();
        effect.StopEffect();
    }

    private static readonly Dictionary<Character, SphereBuffEffect> CultistBuff = new();

    #endregion

    #region VisualEffect

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EffectManager), "GetEffect")]
    public static void GetEffect(EffectManager __instance, VisualEffect effect, Transform __result)
    {
        if (effect is not CustomVisualEffect custom) return;
        var despawn = __result.GetComponent<AnimationDespawner>();
        if (despawn) despawn.Despawn(custom.animation);
        var animator = __result.GetComponent<SpriteAnimator>();
        if (animator) animator.ForcePlay(custom.animation);
    }

    #endregion

    #region Rotorz.Tile.Brush

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Rotorz.Tile.OrientedBrush), "Awake")]
    public static void Awake(Rotorz.Tile.OrientedBrush __instance)
    {
        if (__instance.DefaultOrientation.GetVariation(0) is not GameObject prefab) return;
        if (prefab.GetComponentInChildren<Health>() is { } health) health.EditorVisibility = true;
        if (prefab.TryGetComponent(out OneWayCollider _)) prefab.GetComponentSafe<OneWayEditor>().FixResizeHandles();
        if (prefab.TryGetComponent(out PropMoveable _)) prefab.GetComponentSafe<PropMoveableEditor>();
        switch (prefab.GetComponent<BaseBehaviour>())
        {
            case PropBehaviour prop:
                _ = prefab.GetComponentSafe<LayerEditor>();
                switch (prop)
                {
                    case MineBehaviour:
                        _ = prefab.GetComponentSafe<MineTrapEditor>();
                        break;
                    case TutorialLoader:
                        _ = prefab.GetComponentSafe<TutorialBreakingNews>();
                        break;
                }

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
        if (__instance.TryGetComponent(out OneWayEditor _)) yield break;
        yield return __result;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StairBehaviour), "OnAwake")]
    private static void OnAwake(StairBehaviour __instance)
    {
        _ = __instance.gameObject.GetComponentSafe<StairEditor>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(TutorialScreen), "SetNews")]
    private static void SetNews(TutorialScreen __instance, out List<string> __state)
    {
        __state = Traverse.Create(__instance).Field<List<string>>("breakingNews").Value;
        var settings = Traverse.Create(__instance).Field<TutorialSettings>("tutorialSettings").Value;
        if (!settings.ShowBreakingNews) return;
        var news = settings.GetComponent<TutorialBreakingNews>();
        if (news is null) return;
        var lines = new List<string>(news);
        if (lines.Count == 0) return;
        Traverse.Create(__instance).Field<List<string>>("breakingNews").Value = lines;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TutorialScreen), "SetNews")]
    private static void SetNews(TutorialScreen __instance, List<string> __state)
    {
        Traverse.Create(__instance).Field<List<string>>("breakingNews").Value = __state;
        foreach (var (key, gui) in Traverse.Create(__instance)
                     .Field<Dictionary<string, TMPro.TextMeshProUGUI>>("currentNews").Value)
        {
            gui.text = gui.text.StartsWith("Headlines/") ? key : gui.text;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PropMoveable), "Move", new Type[] { })]
    private static void Move(PropMoveable __instance)
    {
        if (__instance.StopAtNextStep) return;
        var editor = __instance.GetComponent<PropMoveableEditor>();
        if (editor is null) return;
        var speed = Traverse.Create(__instance).Field<float>("currentSpeed");
        speed.Value = 0;
        if (editor.Tweener != null) editor.enabled = false;
        editor.Tweener = DOTween.To(() => speed.Value, value => speed.Value = value, __instance.Speed, editor.Duration)
            .SetEase(editor.SpeedEase);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PropMoveable), "Stop")]
    private static void Stop(PropMoveable __instance)
    {
        if (__instance.StopAtNextStep) return;
        var editor = __instance.GetComponent<PropMoveableEditor>();
        if (editor is null) return;
        var speed = Traverse.Create(__instance).Field<float>("currentSpeed");
        speed.Value = __instance.Speed;
        if (editor.Tweener != null) editor.enabled = false;
        editor.Tweener = DOTween.To(() => speed.Value, value => speed.Value = value, 0, editor.Duration)
            .SetEase(editor.SpeedEase);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ResizableParticleSystem), "SetActive", typeof(bool))]
    private static bool SetActive(ResizableParticleSystem __instance, bool state)
    {
        if (__instance is not FogOfWar fog) return true;
        fog.SetActive(state);
        return false;
    }

    #endregion
}