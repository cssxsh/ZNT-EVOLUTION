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

        [HarmonyPatch(typeof(MovingObjectAsset), methodName: "LoadFromAsset"), HarmonyPostfix]
        public static void LoadFromAsset(MovingObjectAsset __instance, GameObject gameObject)
        {
            var controller = gameObject.GetComponent<MovingObjectAnimationController>();
            if (controller == null) return;
            var orientation = gameObject.GetComponent<ObjectOrientation>();
            if (orientation == null) return;
            var current = orientation.CurrentOrientation.ToString().ToLower();
            var clip = string.Format(__instance.StandAnimation, current);
            if (!controller.Animator.AnimationExists(clip)) return;
            var frame = controller.Animator.GetAnimationClip(clip).frames[0];
            controller.Animator.Sprite.SetSprite(frame.spriteCollection, frame.spriteId);
        }

        [HarmonyPatch(typeof(ObjectOrientation), methodName: "orientation", MethodType.Setter), HarmonyPostfix]
        public static void CurrentOrientation(ObjectOrientation __instance, ObjectOrientation.Orientation value)
        {
            var controller = __instance.GetComponentInParent<MovingObjectAnimationController>();
            if (controller == null) return;
            var current = value.ToString().ToLower();
            var clip = string.Format(controller.Asset.StandAnimation, current);
            if (!controller.Animator.AnimationExists(clip)) return;
            controller.Animator.Sprite.SetSprite(clip);
        }

        [HarmonyPatch(typeof(SpriteAnimator), methodName: "ForcePlay", typeof(string), typeof(bool), typeof(bool)),
         HarmonyPrefix]
        public static void ForcePlay(SpriteAnimator __instance, ref string animName)
        {
            var controller = __instance.GetComponentInParent<MovingObjectAnimationController>();
            if (controller == null) return;
            var orientation = __instance.GetComponentInParent<ObjectOrientation>();
            if (orientation == null) return;
            var current = orientation.CurrentOrientation.ToString().ToLower();
            animName = string.Format(animName, current);
        }

        #endregion

        #region TriggerAsset

        [HarmonyPatch(typeof(TriggerAsset), methodName: "LoadFromAsset"), HarmonyPostfix]
        public static void LoadFromAsset(TriggerAsset __instance, GameObject gameObject)
        {
            if (gameObject.TryGetComponent<MineBehaviour>(out _))
            {
                gameObject.AddComponent<MineTrapEditor>();
            }
        }

        #endregion

        #region ExplosionAsset

        [HarmonyPatch(typeof(ExplosionAsset), methodName: "LoadFromAsset"), HarmonyPostfix]
        public static void LoadFromAsset(ExplosionAsset __instance, GameObject gameObject)
        {
            gameObject.AddComponent<ExplosionEditor>();
            gameObject.AddComponent<SignalReceiverLinker>();
            gameObject.AddComponent<SignalSenderLinker>();
        }

        #endregion

        #region CharacterAsset

        [HarmonyPatch(typeof(CharacterAsset), methodName: "LoadFromAsset"), HarmonyPostfix]
        public static void LoadFromAsset(CharacterAsset __instance, GameObject gameObject)
        {
            if (gameObject.TryGetComponent<HumanBehaviour>(out _))
            {
                gameObject.AddComponent<HumanEditor>();
            }
        }

        #endregion

        #region Rotorz.Tile.Brush

        [HarmonyPatch(typeof(Rotorz.Tile.OrientedBrush), methodName: "Awake"), HarmonyPostfix]
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

        [HarmonyPatch(typeof(SignalReceiverLinker), methodName: "OnAwake"), HarmonyPrefix]
        public static void OnAwake(SignalReceiverLinker __instance)
        {
            __instance.ExcludedComponents ??= __instance.GetComponentsInChildren<BaseComponent>(true)
                .Where(component => !component.EditorVisibility).ToList();
            __instance.ExcludedGameObjects ??= new List<GameObject>();
        }

        [HarmonyPatch(typeof(SignalSenderLinker), methodName: "OnAwake"), HarmonyPrefix]
        public static void OnAwake(SignalSenderLinker __instance)
        {
            __instance.ExcludedComponents ??= __instance.GetComponentsInChildren<BaseComponent>(true)
                .Where(component => !component.EditorVisibility).ToList();
            __instance.ExcludedGameObjects ??= new List<GameObject>();
        }

        #endregion
    }
}