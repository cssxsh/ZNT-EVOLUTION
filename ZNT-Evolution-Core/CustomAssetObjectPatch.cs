using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
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
        var auto = explosion.AutoExplode;
        try
        {
            explosion.AutoExplode = false;
            explosion.LoadFromAsset(prefab.gameObject);
            return prefab;
        }
        finally
        {
            explosion.AutoExplode = auto;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CustomAssetObject), "LoadFromAsset")]
    public static void LoadFromAsset(CustomAssetObject __instance, GameObject gameObject)
    {
        if (UserManager.IsUserDev)
        {
            Logger.LogDebug($"LoadFromAsset: {gameObject.name} {gameObject.transform.position} for {__instance}");
        }
    }

    #region ExplosionAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ExplosionAsset), "LoadFromAsset")]
    public static void LoadFromAsset(ExplosionAsset __instance, GameObject gameObject)
    {
        if (__instance.AutoExplode) return;
        var editor = gameObject.GetComponentSafe<ExplosionEditor>();
        editor.Delay = __instance.Delay;
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
        var prefab = __instance.ExplosionPrefab;
        if (prefab is not null && prefab.IsChildOf(__instance.transform)) return;
        __instance.ExplosionPrefab = __instance.Explosion.CreatePrefab(parent: __instance.transform);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MineBehaviour), "Explode")]
    public static bool Explode(MineBehaviour __instance)
    {
        var prefab = __instance.ExplosionPrefab;
        if (!(prefab is not null && prefab.IsChildOf(__instance.transform))) return true;
        __instance.Trigger.enabled = false;
        prefab.GetComponent<ExplosionEditor>().StartExplosion();
        __instance.Animation.PlayExplosion();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MineBehaviour), "Destroy")]
    public static void Destroy(MineBehaviour __instance)
    {
        var prefab = __instance.ExplosionPrefab;
        ComponentSingleton<GamePoolManager>.Instance.Despawn(prefab);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ExplosionEffect), "OnApplyOnGameObject")]
    public static void OnApplyOnGameObject(ExplosionEffect __instance, GameObject target, out float __state)
    {
        __state = __instance.Damage;
        if (!target.HasAnyTags(Tag.Human)) return;
        var proof = EvolutionSettings.Instance.ExplosionProof;
        if (proof is 0) return;
        var count = Physics2D.LinecastNonAlloc(
            start: __instance.Trigger.Detection.Origin.position,
            end: target.transform.position,
            results: DetectionHelper.DistanceCheck,
            layerMask: LayerMask.GetMask("Zombie Stopper"));
        var total = 0;
        for (var i = 0; i < count; i++)
        {
            var hit = DetectionHelper.DistanceCheck[i];
            var opponents = Opponents.GetValueOrDefault(hit.collider, 0);
            total += opponents;
        }

        __instance.Damage -= total * proof;
    }

    [HarmonyFinalizer]
    [HarmonyPatch(typeof(ExplosionEffect), "OnApplyOnGameObject")]
    public static void OnApplyOnGameObject(ExplosionEffect __instance, float __state)
    {
        __instance.Damage = __state;
    }

    #endregion

    #region DecorAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DecorAsset), "LoadFromAsset")]
    public static void LoadFromAsset(DecorAsset __instance, GameObject gameObject)
    {
        // ReSharper disable once InvertIf
        if (gameObject.TryGetComponent(out SceneVisualEffect behaviour))
        {
            behaviour.Tk2dAnimator.Library = __instance.Animation;
            behaviour.Tk2dAnimator.DefaultClipId = __instance.Animation.GetClipIdByName(__instance.ActiveAnimation);
            behaviour.Event<UnityEvent>("OnActivate").AddListener(behaviour.PlayActivate);
            behaviour.Event<UnityEvent>("OnDeactivate").AddListener(behaviour.PlayDeactivate);
        }
    }

    #endregion

    #region TriggerAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TriggerAsset), "LoadFromAsset")]
    public static void LoadFromAsset(TriggerAsset __instance, GameObject gameObject)
    {
        // ReSharper disable once InvertIf
        if (__instance.Prefab.GetComponent<Trigger>() is { name: "InvisibleTrigger" } trigger)
        {
            var dialogue = trigger.GetEffect<DialogueEffect>();
            dialogue.EditorVisibility.CustomName = __instance.Name + " Dialogue Effect";
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
        behaviour.ActivateColliders(false);
        var controller = (MovingObjectAnimationController)behaviour.AnimationController;
        if (__instance.StandAnimation.Contains('{') ||
            __instance.DisableAnimation.Contains('{') ||
            __instance.MoveAnimation.Contains('{') ||
            __instance.StopAnimation.Contains('{') ||
            __instance.HitAnimation.Contains('{') ||
            __instance.DestroyAnimation.Contains('{'))
        {
            controller.Asset = Object.Instantiate(__instance);
        }

        behaviour.Orientation = behaviour.Orientation;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MovingObjectBehaviour), "OnSpawn")]
    public static void OnSpawn(MovingObjectBehaviour __instance, Parameters param)
    {
        if (param is null) return;
        // ReSharper disable once InvertIf
        if (param.ContainsKey("speed_ease"))
        {
            var editor = __instance.GetComponent<MovingObjectEditor>();
            editor.SpeedEase = param.GetValue<DG.Tweening.Ease>("speed_ease");
            editor.Duration = param.GetValue<float>("speed_ease_duration");
        }
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
            moving.SetOrientation(value);
            var controller = (MovingObjectAnimationController)moving.AnimationController;
            if (controller.Asset.name.EndsWith("(Clone)"))
            {
                var asset = (MovingObjectAsset)__instance.GetComponent<AssetComponent>().Asset;
                var direction = value is ObjectOrientation.Orientation.Right ? "right" : "left";
                controller.Asset.StandAnimation = string.Format(asset.StandAnimation, direction);
                controller.Asset.DisableAnimation = string.Format(asset.DisableAnimation, direction);
                controller.Asset.MoveAnimation = string.Format(asset.MoveAnimation, direction);
                controller.Asset.StopAnimation = string.Format(asset.StopAnimation, direction);
                controller.Asset.HitAnimation = string.Format(asset.HitAnimation, direction);
                controller.Asset.DestroyAnimation = string.Format(asset.DestroyAnimation, direction);
            }

            if (Execution.SceneMode is Execution.Mode.Edition &&
                controller.Animator.GetAnimationClip(controller.Asset.StandAnimation) is { Empty: false } clip)
            {
                var frame = clip.frames[0];
                controller.Animator.Sprite.SetSprite(frame.spriteCollection, frame.spriteId);
            }
        }

        var sprite = moving?.AnimationController.Animator.Sprite
                     ?? __instance.GetComponentInChildren<tk2dBaseSprite>();
        if (sprite is null) return;
        sprite.SortingOrder = value is ObjectOrientation.Orientation.Right ? 0 : -1;
        var properties = new MaterialPropertyBlock();
        properties.SetFloat("_UseFlip", (int)value);
        sprite.CachedRenderer.SetPropertyBlock(properties);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MovingObjectBehaviour), "OnDespawned")]
    public static void OnDespawned(MovingObjectBehaviour __instance)
    {
        var controller = (MovingObjectAnimationController)__instance.AnimationController;
        if (controller.Asset.name.EndsWith("(Clone)")) Object.Destroy(controller.Asset);
    }

    #endregion

    #region PhysicObjectAsset

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PhysicObjectAsset), "LoadFromAsset")]
    public static void LoadFromAsset(PhysicObjectAsset __instance, GameObject gameObject)
    {
        var behaviour = gameObject.GetComponent<PhysicObjectBehaviour>();
        if (behaviour.Physic.StartDirection.IsZero()
            && behaviour.Physic.StartForce is not 0) Logger.LogWarning($"{__instance} StartDirection is zero");
        behaviour.DamageTriger.enabled = behaviour.DamageCharacterOnTrigger
                                         || (behaviour.ExplodeOn & ExplodeSurfaceConverter.IgnoreHuman) is not 0;

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
        if (flag) __instance.SendTargetDamage(other.gameObject);
        // TODO param by EvolutionSettings
        if (flag && __instance.Physic.GravityScale is 0.0f)
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
        if (param is null) return;
        // ReSharper disable once InvertIf
        if (param.ContainsKey("dialogue_text"))
        {
            var text = param.GetValue<LocalizableString>("dialogue_text");
            var duration = param.GetValue<float>("dialogue_duration");
            var voice = param.GetValue<Voice>("dialogue_voice");
            __instance.Dialogue(text, duration, voice);
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
        var repulse = __instance.Repulse;
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
        __instance.Repulse = explode.gameObject;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Rage), "OnHit")]
    public static bool OnHit(Rage __instance, Parameters param)
    {
        if (!__instance.enabled) return false;
        var repulse = __instance.Repulse;
        if (repulse is null) return false;
        var flags = DamageFlagsConverter.GetDamageFlags(__instance.DamageType);
        var damage = DamageFlagsConverter.GetDamageFlags(param.GetDamageType());
        if (!(flags is 0 || flags.HasFlag(damage))) return false;
        var timer = __instance.Timer;
        var hits = __instance.Hits;
        if (--hits > 0)
        {
            timer.Start();
            __instance.Timer = timer;
            __instance.Hits = hits;
            return false;
        }

        hits = __instance.RefillOnEnraged ? __instance.TargetHitCount : 0;
        timer.Stop();
        __instance.Timer = timer;
        __instance.Hits = hits;
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
        var detector = __instance.Detector;
        var collider = detector.GetComponent<Collider2D>();
        Opponents[collider] = block ? maxOpponents : 0;
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
        var detector = __instance.Detector;
        var collider = detector.GetComponent<Collider2D>();
        Opponents.Remove(collider);
        var effect = detector.GetComponent<CharacterAllocationEffect>();
        effect.StopEffect();
    }

    private static readonly Dictionary<Character, SphereBuffEffect> CultistBuff = new();

    private static readonly Dictionary<Collider2D, int> Opponents = new();

    #endregion

    #region VisualEffect

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EffectManager), "GetEffect")]
    public static void GetEffect(EffectManager __instance, VisualEffect effect, Transform __result)
    {
        if (effect is CustomVisualEffect custom) custom.Populate(__result);
    }

    #endregion

    #region BaseComponent

    [HarmonyPostfix]
    [HarmonyPatch(typeof(OneWayCollider), "Start")]
    public static IEnumerator Start(IEnumerator __result, OneWayCollider __instance)
    {
        if (__instance.TryGetComponent(out OneWayEditor _)) yield break;
        yield return __result;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(TutorialScreen), "SetNews")]
    public static bool SetNews(TutorialScreen __instance)
    {
        var settings = __instance.TutorialSettings;
        if (!settings.ShowBreakingNews) return true;
        var news = settings.GetComponent<TutorialBreakingNews>();
        if (news is null) return true;
        var current = __instance.CurrentNews;
        var count = __instance.NewsCount;
        var prefab = __instance.NewsPrefab;
        var container = __instance.NewsContainer;
        current.Clear();
        foreach (var line in news.OrderBy(_ => Random.value))
        {
            if (current.ContainsKey(line.Content)) continue;
            var target = ComponentSingleton<GamePoolManager>.Instance.Spawn(prefab);
            target.SetParent(container);
            target.localScale = Vector3.one;
            var tm = target.GetComponent<TMPro.TextMeshProUGUI>();
            tm.text = line.Content;
            __instance.AddScrollRecycler(target);
            current.Add(line.Content, tm);
            if (current.Count >= count) break;
        }

        if (current.Count is 0) return true;
        Timer.DelayedCall(0.2f, __instance.Delegate<DG.Tweening.TweenCallback>("AddFiller"));
        return false;
    }

    #endregion
}