using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;
using ZNT.Evolution.Core.Effect;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
// ReSharper disable Unity.PreferAddressByIdToGraphicsParams
namespace ZNT.Evolution.Core;

internal static class CustomAssetObjectPatch
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(CustomAssetObject));

    private static DamageType GetDamageType(this Parameters parameters, string key = nameof(DamageType))
    {
        return parameters.ContainsKey(key) ? parameters.GetValue<DamageType>(key) : DamageType.None;
    }

    private static Transform CreatePrefab(this ExplosionAsset explosion, Transform parent)
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

    private static void DespawnBy(this AnimationDespawner despawn, AnimationSettings animation)
    {
        if (animation == null) return;
        Traverse.Create(despawn).Field<AnimationEventHandler>("eventHandler").Value?
            .RegisterEndEvent(animation, despawn.CreateDelegate<Action>("Despawn"));
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
        _ = gameObject.GetComponentSafe<ExplosionEditor>();
        var linker = gameObject.GetComponentInParent<SignalReceiverLinker>()
                     ?? gameObject.GetComponentSafe<SignalReceiverLinker>();
        linker.ExcludedComponents ??= [];
        linker.ExcludedGameObjects ??= [];
        foreach (var child in gameObject.transform.Cast<Transform>())
        {
            if (linker.ExcludedGameObjects.Contains(child.gameObject)) continue;
            linker.ExcludedGameObjects.Add(child.gameObject);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MineBehaviour), "OnCreate")]
    public static void OnCreate(MineBehaviour __instance)
    {
        var prefab = Traverse.Create(__instance).Field<Transform>("explosionPrefab").Value;
        if (prefab is not null && prefab.IsChildOf(__instance.transform)) return;
        var explosion = Traverse.Create(__instance).Field<ExplosionAsset>("explosion").Value;
        var explode = explosion.CreatePrefab(parent: __instance.transform);
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

    #region TriggerAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TriggerAsset), "LoadFromAsset")]
    public static void LoadFromAsset(TriggerAsset __instance, GameObject gameObject)
    {
        // ReSharper disable once InvertIf
        if (__instance.Name is "Human")
        {
            var dialogue = gameObject.GetComponent<Trigger>().GetEffect<DialogueEffect>();
            dialogue.SetVisible(true);
            dialogue.Mode = DialogueEffect.DetectionMode.SignalOnEnter;
        }
    }

    #endregion

    #region MovingObjectAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MovingObjectAsset), "LoadFromAsset")]
    public static void LoadFromAsset(MovingObjectAsset __instance, GameObject gameObject)
    {
        var behaviour = gameObject.GetComponent<MovingObjectBehaviour>();
        var controller = (MovingObjectAnimationController)behaviour.AnimationController;
        if (__instance.StandAnimation.Contains('{') ||
            __instance.DisableAnimation.Contains('{') ||
            __instance.MoveAnimation.Contains('{') ||
            __instance.StopAnimation.Contains('{') ||
            __instance.HitAnimation.Contains('{') ||
            __instance.DestroyAnimation.Contains('{'))
        {
            controller.Asset = UnityEngine.Object.Instantiate(__instance);
        }

        behaviour.Orientation = behaviour.Orientation;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MovingObjectBehaviour), "OnSpawn")]
    [HarmonyPatch(typeof(MovingObjectBehaviour), "Orientation", MethodType.Setter)]
    public static void SetOrientation(MovingObjectBehaviour __instance)
    {
        var orientation = __instance.GetComponent<ObjectOrientation>();
        orientation?.CurrentOrientation = __instance.Orientation == Vector3.forward
            ? ObjectOrientation.Orientation.Right
            : ObjectOrientation.Orientation.Left;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ObjectOrientation), "orientation", MethodType.Setter)]
    public static void SetOrientation(ObjectOrientation __instance, ObjectOrientation.Orientation value)
    {
        if (__instance.TryGetComponent(out MovingObjectBehaviour moving))
        {
            Traverse.Create(moving).Field<Vector3>("orientation").Value =
                value == ObjectOrientation.Orientation.Right ? Vector3.forward : Vector3.back;
            var controller = (MovingObjectAnimationController)moving.AnimationController;
            if (controller.Asset.name.EndsWith("(Clone)"))
            {
                var asset = (MovingObjectAsset)__instance.GetComponent<AssetComponent>().Asset;
                var direction = value == ObjectOrientation.Orientation.Right ? "right" : "left";
                controller.Asset.StandAnimation = string.Format(asset.StandAnimation, direction);
                controller.Asset.DisableAnimation = string.Format(asset.DisableAnimation, direction);
                controller.Asset.MoveAnimation = string.Format(asset.MoveAnimation, direction);
                controller.Asset.StopAnimation = string.Format(asset.StopAnimation, direction);
                controller.Asset.HitAnimation = string.Format(asset.HitAnimation, direction);
                controller.Asset.DestroyAnimation = string.Format(asset.DestroyAnimation, direction);
            }

            if (Execution.SceneMode is Execution.Mode.Edition)
            {
                var frame = controller.Animator.GetAnimationClip(controller.Asset.StandAnimation).frames[0];
                controller.Animator.Sprite.SetSprite(frame.spriteCollection, frame.spriteId);
            }
        }

        var sprite = moving?.AnimationController.Animator.Sprite
                     ?? __instance.GetComponentInChildren<tk2dBaseSprite>();
        if (sprite is null) return;
        sprite.SortingOrder = value == ObjectOrientation.Orientation.Right ? 0 : -1;
        var properties = new MaterialPropertyBlock();
        properties.SetFloat("_UseFlip", (int)value);
        sprite.CachedRenderer.SetPropertyBlock(properties);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MovingObjectBehaviour), "OnDespawned")]
    public static void OnDespawned(MovingObjectBehaviour __instance)
    {
        var controller = (MovingObjectAnimationController)__instance.AnimationController;
        if (controller.Asset.name.EndsWith("(Clone)")) UnityEngine.Object.Destroy(controller.Asset);
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
                                         || (behaviour.ExplodeOn & ExplodeSurfaceConverter.IgnoreHuman) != 0;

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
        if (flag) Traverse.Create(__instance).Method("SendTargetDamage", other.gameObject).GetValue();
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

        var target = other.GetComponent<Character>()?.AnimationController switch
        {
            ZombieAnimationController => ExplodeSurfaceConverter.Zombie,
            ClimberAnimationController => ExplodeSurfaceConverter.Climber,
            BlockerAnimationController => ExplodeSurfaceConverter.Blocker,
            TankAnimationController => ExplodeSurfaceConverter.Tank,
            HumanAnimationController { gameObject.layer: 0x1D } => ExplodeSurfaceConverter.WorldEnemy,
            _ => ExplodeSurfaceConverter.None
        };
        if (target is ExplodeSurfaceConverter.None) return false;
        if (__instance.ExplodeOn.HasFlag(target)) __instance.OnDie(null);
        return false;
    }

    #endregion

    #region HumanAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HumanBehaviour), "Initialize")]
    public static void Initialize(HumanBehaviour __instance)
    {
        if (__instance.SharedAsset.CharacterType is CharacterType.Cultist
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
    [HarmonyPatch(typeof(CharacterBehaviour), "OnSpawn")]
    public static void OnSpawn(CharacterBehaviour __instance, Parameters param)
    {
        if (param == null) return;
        if (__instance is not HumanBehaviour human) return;
        // ReSharper disable once InvertIf
        if (param.ContainsKey("dialogue_text"))
        {
            var text = param.GetValue<LocalizableString>("dialogue_text");
            var duration = param.GetValue<float>("dialogue_duration");
            var voice = param.GetValue<Voice>("dialogue_voice");
            var patroller = human.Patroller;
            var dialogue = ComponentSingleton<GamePoolManager>.Instance
                .Spawn(nameof(Dialogue)).GetComponent<Dialogue>();
            dialogue.SetText(text, duration);
            dialogue.Show(patroller, patroller.DialogueOffset, voice);
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
    [HarmonyPatch(typeof(HumanBehaviour), "ResetVision")]
    public static bool ResetVision(HumanBehaviour __instance)
    {
        return __instance.VisionFollowTarget || __instance.SharedAsset.OverrideOnAim;
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
        var explode = value.CreatePrefab(parent: __instance.transform);
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
        __instance.Event<BoolEvent>("OnRage").Invoke(__instance.FreezeOnRage);
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
        var effect = detector.GetComponent<Trigger>().GetEffect<CharacterAllocationEffect>();
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
        if (despawn) despawn.DespawnBy(custom.animation);
        var animator = __result.GetComponent<SpriteAnimator>();
        if (animator) animator.ForcePlay(custom.animation);
    }

    #endregion

    #region BaseComponent

    [HarmonyPostfix]
    [HarmonyPatch(typeof(OneWayCollider), "Start")]
    private static IEnumerator Start(IEnumerator __result, OneWayCollider __instance)
    {
        if (__instance.TryGetComponent(out OneWayEditor _)) yield break;
        yield return __result;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(TutorialScreen), "SetNews")]
    private static bool SetNews(TutorialScreen __instance)
    {
        var settings = Traverse.Create(__instance).Field<TutorialSettings>("tutorialSettings").Value;
        if (!settings.ShowBreakingNews) return true;
        var news = settings.GetComponent<TutorialBreakingNews>();
        if (news is null) return true;
        var current = Traverse.Create(__instance).Field<Dictionary<string, TMPro.TextMeshProUGUI>>("currentNews").Value;
        var count = Traverse.Create(__instance).Field<int>("newsCount").Value;
        var prefab = Traverse.Create(__instance).Field<RectTransform>("newsPrefab").Value;
        var container = Traverse.Create(__instance).Field<RectTransform>("newsContainer").Value;
        current.Clear();
        foreach (var line in news.OrderBy(_ => UnityEngine.Random.value))
        {
            if (current.ContainsKey(line.Content)) continue;
            var target = ComponentSingleton<GamePoolManager>.Instance.Spawn(prefab);
            target.SetParent(container);
            target.localScale = Vector3.one;
            var tm = target.GetComponent<TMPro.TextMeshProUGUI>();
            tm.text = line.Content;
            Traverse.Create(__instance).Method("AddScrollRecycler", target).GetValue();
            current.Add(line.Content, tm);
            if (current.Count >= count) break;
        }

        if (current.Count == 0) return true;
        Timer.DelayedCall(0.2f, __instance.CreateDelegate<DG.Tweening.TweenCallback>("AddFiller"));
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PropMoveable), "Move", [])]
    [HarmonyPatch(typeof(PropMoveable), "MoveOpposite")]
    private static void Move(PropMoveable __instance)
    {
        if (__instance.StopAtNextStep) return;
        var editor = __instance.GetComponent<PropMoveableEditor>();
        editor?.SpeedTween(__instance, 0, __instance.Speed);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PropMoveable), "Stop")]
    private static void Stop(PropMoveable __instance)
    {
        if (__instance.StopAtNextStep) return;
        var editor = __instance.GetComponent<PropMoveableEditor>();
        editor?.SpeedTween(__instance, __instance.Speed, 0);
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