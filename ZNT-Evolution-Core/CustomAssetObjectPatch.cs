using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    internal static class CustomAssetObjectPatch
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("CustomAssetObject");

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomAssetObject), "LoadFromAsset")]
        public static void LoadFromAsset(CustomAssetObject __instance, GameObject gameObject)
        {
            Logger.LogDebug($"LoadFromAsset: {gameObject} for {__instance}");
        }
        
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhysicObjectBehaviour), "OnTriggerEnter2D")]
        public static bool OnTriggerEnter2D(PhysicObjectBehaviour __instance, Collider2D other)
        {
            if (!__instance.TargetLayers.ContainsLayer(other.gameObject.layer)) return false;
            var physic = __instance.Physic;
            var force = physic.StartDirection * physic.StartForce * physic.Body.mass;
            physic.Body.AddForce(force * physic.Collider.friction * -1, ForceMode2D.Impulse);
            if (physic.Body.velocity.magnitude <= physic.StartForce * 0.5) physic.Body.velocity = Vector2.zero;
            Traverse.Create(__instance).Field<bool>("checkStuck").Value = true;

            var targets = other.GetComponents<BaseAnimationController>()
                .Aggregate(ExplodeSurfaceConverter.None, (mask, controller) => mask | controller switch
                {
                    ZombieAnimationController { enabled: true } => ExplodeSurfaceConverter.Zombie,
                    ClimberAnimationController { enabled: true } => ExplodeSurfaceConverter.Climber,
                    BlockerAnimationController { enabled: true } => ExplodeSurfaceConverter.Blocker,
                    TankAnimationController { enabled: true } => ExplodeSurfaceConverter.Tank,
                    _ => ExplodeSurfaceConverter.None
                });
            if (targets == ExplodeSurfaceConverter.None) return true;
            if (__instance.SharedAsset.ExplodeOn.HasFlag(targets)) __instance.OnDie(null);
            return true;
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
                    if (gameObject.GetComponent<MineTrapEditor>() is null) gameObject.AddComponent<MineTrapEditor>();
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
                    if (gameObject.GetComponent<HumanEditor>() is null) gameObject.AddComponent<HumanEditor>();
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
            if (body is null) return;
            if (body.TryGetComponent<Health>(out var health)) health.EditorVisibility = true;
            switch (body)
            {
                case var _ when body.TryGetComponent<PropBehaviour>(out _):
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