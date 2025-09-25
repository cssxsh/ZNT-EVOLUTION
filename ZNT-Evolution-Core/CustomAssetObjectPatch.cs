using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    internal static class CustomAssetObjectPatch
    {
        #region MovingObjectAsset

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MovingObjectAsset), "LoadFromAsset")]
        public static void LoadFromAsset(MovingObjectAsset __instance, GameObject gameObject)
        {
            var controller = gameObject.GetComponent<MovingObjectAnimationController>();
            if (controller is null) return;
            var orientation = gameObject.GetComponent<ObjectOrientation>();
            if (orientation is null) return;
            var current = orientation.CurrentOrientation.ToString().ToLower();
            var clip = string.Format(__instance.StandAnimation, current);
            if (!controller.Animator.AnimationExists(clip)) return;
            var frame = controller.Animator.GetAnimationClip(clip).frames[0];
            controller.Animator.Sprite.SetSprite(frame.spriteCollection, frame.spriteId);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ObjectOrientation), "orientation", MethodType.Setter)]
        public static void CurrentOrientation(ObjectOrientation __instance, ObjectOrientation.Orientation value)
        {
            var controller = __instance.GetComponentInParent<MovingObjectAnimationController>();
            if (controller is null) return;
            var current = value.ToString().ToLower();
            var clip = string.Format(controller.Asset.StandAnimation, current);
            if (!controller.Animator.AnimationExists(clip)) return;
            controller.Animator.Sprite.SetSprite(clip);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpriteAnimator), "ForcePlay", typeof(string), typeof(bool), typeof(bool))]
        public static void ForcePlay(SpriteAnimator __instance, ref string animName)
        {
            var controller = __instance.GetComponentInParent<MovingObjectAnimationController>();
            if (controller is null) return;
            var orientation = __instance.GetComponentInParent<ObjectOrientation>();
            if (orientation is null) return;
            var current = orientation.CurrentOrientation.ToString().ToLower();
            animName = string.Format(animName, current);
        }

        #endregion

        #region PhysicObjectAsset

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PhysicObjectBehaviour), "OnTriggerEnter2D", typeof(Collider2D))]
        public static void OnTriggerEnter2D(PhysicObjectBehaviour __instance, Collider2D other)
        {
            var mask = (int)__instance.SharedAsset.ExplodeOn;
            var target = 0x00;
            if (other.TryGetComponent<ZombieAnimationController>(out var zombie) && zombie.enabled) target |= 0x10;
            if (other.TryGetComponent<ClimberAnimationController>(out var climber) && climber.enabled) target |= 0x20;
            if (other.TryGetComponent<BlockerAnimationController>(out var blocker) && blocker.enabled) target |= 0x40;
            if (other.TryGetComponent<TankAnimationController>(out var tank) && tank.enabled) target |= 0x80;
            if (!BitMask.HasAny(mask, target)) return;
            __instance.OnDie(null);
        }

        #endregion

        #region TriggerAsset

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TriggerAsset), "LoadFromAsset")]
        public static void LoadFromAsset(TriggerAsset __instance, GameObject gameObject)
        {
            switch (gameObject)
            {
                case var _ when gameObject.TryGetComponent<MineBehaviour>(out _):
                    gameObject.AddComponent<MineTrapEditor>();
                    break;
            }
        }

        #endregion

        #region CharacterAsset

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterAsset), "LoadFromAsset")]
        public static void LoadFromAsset(CharacterAsset __instance, GameObject gameObject)
        {
            switch (__instance)
            {
                case HumanAsset _:
                    gameObject.AddComponent<HumanEditor>();
                    break;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HumanAnimationController), "Initialize")]
        private static void Initialize(HumanAnimationController __instance)
        {
            var key = $"{__instance.SharedAsset.name}Animations : CharacterAnimationAsset";
            var animations = CustomAssetUtility.Cache[key] as CharacterAnimationAsset;
            if (animations) __instance.SharedAsset.Animations = animations;
        }

        #endregion

        #region Rotorz.Tile.Brush

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Rotorz.Tile.OrientedBrush), "Awake")]
        public static void Awake(Rotorz.Tile.OrientedBrush __instance)
        {
            var body = __instance.DefaultOrientation?.GetVariation(0) as GameObject;
            switch (body)
            {
                case null:
                    return;
                case var _ when body.TryGetComponent<PropBehaviour>(out _):
                    if (body.TryGetComponent<Health>(out var health)) health.EditorVisibility = true;
                    if (body.GetComponent<LayerEditor>() is null) body.AddComponent<LayerEditor>();
                    break;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SignalReceiverLinker), "OnAwake")]
        public static void OnAwake(SignalReceiverLinker __instance)
        {
            __instance.ExcludedComponents ??= __instance.GetComponentsInChildren<BaseComponent>(includeInactive: true)
                .Where(component => !component.EditorVisibility).ToList();
            __instance.ExcludedGameObjects ??= new List<GameObject>();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SignalSenderLinker), "OnAwake")]
        public static void OnAwake(SignalSenderLinker __instance)
        {
            __instance.ExcludedComponents ??= __instance.GetComponentsInChildren<BaseComponent>(includeInactive: true)
                .Where(component => !component.EditorVisibility).ToList();
            __instance.ExcludedGameObjects ??= new List<GameObject>();
        }

        #endregion
    }
}