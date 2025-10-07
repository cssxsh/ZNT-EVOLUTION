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
            Logger.LogDebug($"LoadFromAsset: {gameObject} {gameObject.transform.position} for {__instance}");
        }

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
        public static void CurrentOrientation(ObjectOrientation __instance, ObjectOrientation.Orientation value)
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

        #region Rotorz.Tile.Brush

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Rotorz.Tile.OrientedBrush), "Awake")]
        public static void Awake(Rotorz.Tile.OrientedBrush __instance)
        {
            // ReSharper disable once UseNegatedPatternMatching
            var gameObject = __instance.DefaultOrientation?.GetVariation(0) as GameObject;
            if (gameObject is null) return;
            if (gameObject.TryGetComponent<Health>(out var health)) health.EditorVisibility = true;
            switch (gameObject.GetComponent<BaseBehaviour>())
            {
                case MineBehaviour _:
                    _ = gameObject.GetComponent<LayerEditor>() ?? gameObject.AddComponent<LayerEditor>();
                    _ = gameObject.GetComponent<MineTrapEditor>() ?? gameObject.AddComponent<MineTrapEditor>();
                    break;
                case PropBehaviour _:
                    _ = gameObject.GetComponent<LayerEditor>() ?? gameObject.AddComponent<LayerEditor>();
                    break;
                case HumanBehaviour _:
                    _ = gameObject.GetComponent<HumanEditor>() ?? gameObject.AddComponent<HumanEditor>();
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