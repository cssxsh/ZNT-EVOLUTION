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
            if (!controller.AnimationExists(__instance.StandAnimation)) return;
            var frame = controller.GetAnimationClip(__instance.StandAnimation).frames[0];
            if (controller.Animator.Sprite.Collection != frame.spriteCollection)
            {
                controller.Animator.Sprite.SetSprite(frame.spriteCollection, frame.spriteId);
            }
        }

        [HarmonyPatch(typeof(ObjectOrientation), methodName: "orientation", MethodType.Setter), HarmonyPostfix]
        public static void CurrentOrientation(ObjectOrientation __instance, ObjectOrientation.Orientation value)
        {
            var controller = __instance.GetComponentInParent<MovingObjectAnimationController>();
            if (controller == null) return;
            var asset = controller.Asset;
            var orientation = value.ToString().ToLower();
            if (!controller.AnimationExists(orientation)) return;
            asset.MoveAnimation = asset.StandAnimation = asset.StopAnimation = orientation;
            controller.Animator.Sprite.SetSprite(orientation);
        }

        #endregion

        #region TriggerAsset

        [HarmonyPatch(typeof(TriggerAsset), methodName: "LoadFromAsset"), HarmonyPostfix]
        public static void LoadFromAsset(TriggerAsset __instance, GameObject gameObject)
        {
            if (gameObject.GetComponents<MineBehaviour>().Length != 0)
            {
                gameObject.AddComponent<MineTrapEditor>();
            }
        }

        #endregion
    }
}