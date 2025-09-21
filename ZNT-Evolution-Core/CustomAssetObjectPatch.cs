using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Editor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    public static class CustomAssetObjectPatch
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

        #region TriggerAsset

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TriggerAsset), "LoadFromAsset")]
        public static void LoadFromAsset(TriggerAsset __instance, GameObject gameObject)
        {
            if (gameObject.TryGetComponent<MineBehaviour>(out _))
            {
                gameObject.AddComponent<MineTrapEditor>();
            }
        }

        #endregion

        #region ExplosionAsset

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ExplosionAsset), "LoadFromAsset")]
        public static void LoadFromAsset(ExplosionAsset __instance, GameObject gameObject)
        {
            if (gameObject.TryGetComponent<ExplosionEditor>(out _)) return;
            gameObject.AddComponent<ExplosionEditor>();
            gameObject.AddComponent<SignalReceiverLinker>();
            gameObject.AddComponent<SignalSenderLinker>();
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
                    if (gameObject.TryGetComponent<HumanEditor>(out _)) return;
                    gameObject.AddComponent<HumanEditor>();
                    break;
            }
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
                case var _ when body.TryGetComponent<DoorBehaviour>(out var behaviour):
                    if (behaviour.TryGetComponent<Health>(out var health)) health.EditorVisibility = true;
                    body.AddComponent<LayerEditor>();
                    break;
                case var _ when body.TryGetComponent<BarricadeBehaviour>(out _):
                    body.AddComponent<BarricadeLayerEditor>();
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