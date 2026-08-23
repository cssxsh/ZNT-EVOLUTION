using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class AnimationEventHandlerPatch
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(AnimationEventHandler));

    private static readonly C5.HashedArrayList<MethodInfo> EventHandles = new();

    [UsedImplicitly]
    internal static bool ExistsTriggerEvent(this AnimationEventHandler handler, string name)
    {
        return Traverse.Create(handler)
                   .Field<Dictionary<string, System.Action>>("triggerEvents").Value
                   .ContainsKey(name)
               || Traverse.Create(handler)
                   .Field<Dictionary<string, AnimationEventHandler.EventAction>>("triggerEventsParams").Value
                   .ContainsKey(name);
    }

    [UsedImplicitly]
    internal static bool ExistsEndEvent(this AnimationEventHandler handler, tk2dSpriteAnimationClip clip)
    {
        return Traverse.Create(handler)
            .Field<Dictionary<tk2dSpriteAnimationClip, System.Action>>("endEvents").Value
            .ContainsKey(clip);
    }

    [UsedImplicitly]
    internal static T GetAsset<T>(this tk2dSpriteAnimationFrame frame) where T : CustomAsset
    {
        return CustomAssetUtility.DeserializeObject<T>(frame.soundParamName);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BaseAnimationController), "Initialize")]
    public static void Initialize(BaseAnimationController __instance)
    {
        if (__instance.EventHandler is null) return;
        foreach (var method in EventHandles)
        {
            if (!method.GetParameters()[0].ParameterType.IsInstanceOfType(__instance)) continue;
            foreach (var description in method.GetCustomAttributes<DescriptionAttribute>())
            {
                var index = description.Description.IndexOf(':');
                var name = description.Description.Substring(index + 1);
                switch (description.Description.Substring(0, index))
                {
                    case nameof(AnimationEventHandler.RegisterTriggerEvent):
                        __instance.EventHandler.RegisterTriggerEvent(name, frame => method.Invoke(null, [
                            __instance,
                            frame
                        ]));
                        break;
                    case nameof(AnimationEventHandler.RegisterEndEvent):
                        __instance.EventHandler.RegisterEndEvent(name, () => method.Invoke(null, [
                            __instance,
                            __instance.Animator.CurrentClip
                        ]));
                        break;
                }
            }
        }
    }

    [UsedImplicitly]
    public static void RegisterAnimationEvent(System.Type type)
    {
        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            var infos = method.GetParameters();
            if (infos.Length is not 2) continue;
            if (!typeof(BaseAnimationController).IsAssignableFrom(infos[0].ParameterType)) continue;
            foreach (var description in method.GetCustomAttributes<DescriptionAttribute>())
            {
                var index = description.Description.IndexOf(':');
                if (index is -1) continue;
                switch (description.Description.Substring(0, index))
                {
                    case nameof(AnimationEventHandler.RegisterTriggerEvent)
                        when infos[1].ParameterType.IsAssignableFrom(typeof(tk2dSpriteAnimationFrame)):
                    case nameof(AnimationEventHandler.RegisterEndEvent)
                        when infos[1].ParameterType.IsAssignableFrom(typeof(tk2dSpriteAnimationClip)):
                        if (EventHandles.Add(method)) Logger.LogInfo($"Cached {method.FullDescription()}");
                        break;
                }
            }
        }
    }

    [UsedImplicitly]
    [Description("RegisterTriggerEvent:throw")]
    public static void Throw(CorpseBehaviour controller, tk2dSpriteAnimationFrame frame)
    {
        var parameters = Traverse.Create(controller).Field<CorpseParameter>("parameters").Value;
        if (parameters.Character.Behaviour is not HumanBehaviour human) return;
        var definition = frame.spriteCollection.spriteDefinitions[frame.spriteId];
        var point = definition.attachPoints.FirstOrDefault(point => point.name is "throw")
                    ?? new tk2dSpriteDefinition.AttachPoint();
        human.PhysicObjectThrower.Throw(
            Traverse.Create(controller).Field<UnityEngine.BoxCollider2D>("boxCollider").Value,
            null,
            null,
            parameters.Position + point.position,
            parameters.Direction,
            frame.eventInt
        );
    }

    [UsedImplicitly]
    [Description("RegisterTriggerEvent:throw")]
    public static void Throw(MovingObjectAnimationController controller, tk2dSpriteAnimationFrame frame)
    {
        var asset = frame.GetAsset<PhysicObjectAsset>();
        if (asset is null) return;
        var definition = frame.spriteCollection.spriteDefinitions[frame.spriteId];
        var point = definition.attachPoints.FirstOrDefault(point => point.name is "throw")
                    ?? new tk2dSpriteDefinition.AttachPoint();
        var position = controller.transform.position + point.position;
        var orientation = controller.transform.forward;
        var physic = asset.CreateGameObject(position: position).GetComponent<PhysicObjectBehaviour>();
        physic.transform.localScale = orientation == UnityEngine.Vector3.back
            ? new UnityEngine.Vector3(-1f, 1f, 1f)
            : new UnityEngine.Vector3(1f, 1f, 1f);
        physic.ExplosionParent = asset.AttachToParent ? controller.transform : null;
        physic.IgnoreCollisions(controller.GetComponent<UnityEngine.Collider2D>(), true);
        var sign = UnityEngine.Mathf.Sign(orientation.z);
        var direction = physic.Physic.StartDirection;
        direction.x *= sign;
        physic.Physic.StartDirection = direction;
        physic.Physic.Body.angularVelocity = -asset.StartAngularVelocity * sign;
        physic.Physic.Throw();
    }

    [UsedImplicitly]
    [Description("RegisterTriggerEvent:weapon_fire")]
    public static void Fire(CorpseBehaviour controller, tk2dSpriteAnimationFrame frame)
    {
        var parameters = Traverse.Create(controller).Field<CorpseParameter>("parameters").Value;
        if (parameters.Character.Behaviour is not HumanBehaviour human) return;
        var detected = Traverse.Create(typeof(DetectionHelper))
            .Field<C5.HashedArrayList<UnityEngine.GameObject>>("Covered").Value;
        DetectionHelper.RayCastAll(
            detected,
            DetectionHelper.DistanceCheck,
            parameters.Position,
            controller.transform.right,
            parameters.CharacterAsset.DamageRange + 0.5f,
            human.Attacker.AttackTrigger.IgnoreLayers,
            human.Attacker.AttackTrigger.Layers,
            human.Attacker.AttackTrigger.IgnoreWithTags,
            human.Attacker.AttackTrigger.WithTags,
            human.Attacker.AttackTrigger.IgnoreWithoutTags,
            human.Attacker.AttackTrigger.WithoutTags,
            human.Attacker.AttackTrigger.WithAllTags,
            human.Attacker.AttackTrigger.WithoutAllTags,
            human.Attacker.AttackTrigger.InvertTagsMatch);
        var count = parameters.CharacterAsset.HitMultipleTargets ? parameters.CharacterAsset.MaxTargets : 1;
        var damage = parameters.CharacterAsset.Damage;
        foreach (var target in detected)
        {
            if (count is 0) break;
            if (!DetectionHelper.ObjectInRange(
                    parameters.Position,
                    target.transform,
                    parameters.CharacterAsset.DamageRange,
                    human.Attacker.BlockingView)) continue;
            var health = target.GetComponentInChildren<Health>();
            if (health)
            {
                var distance = DetectionHelper.GetObjectDistance(parameters.Position, target.transform);
                var time = UnityEngine.Mathf.Clamp01(distance / parameters.CharacterAsset.DamageRange);
                var amount = damage * parameters.CharacterAsset.DamageFalloff.Evaluate(time);
                health.ReceiveDamage(amount, controller.transform, parameters.CharacterAsset.DamageType);
            }

            damage *= parameters.CharacterAsset.NextTargetsDamageMultiplier;
            count--;
        }
    }

    [UsedImplicitly]
    [Description("RegisterTriggerEvent:repulse")]
    public static void Repulse(HumanAnimationController controller, tk2dSpriteAnimationFrame frame)
    {
        var human = Traverse.Create(controller).Field<HumanBehaviour>("Behaviour").Value;
        var repulse = Traverse.Create(human.Rage).Field<UnityEngine.GameObject>("repulse").Value;
        if (repulse) repulse.GetComponent<ExplosionEditor>().StartExplosion();
    }

    [UsedImplicitly]
    [Description("RegisterTriggerEvent:summon_human")]
    public static void Summon(BaseAnimationController controller, tk2dSpriteAnimationFrame frame)
    {
        var asset = frame.GetAsset<HumanAsset>();
        if (asset is null) return;
        var human = asset.CreateGameObject(position: controller.transform.position).GetComponent<HumanBehaviour>();
        human.Character.OnSpawn(new Parameters(id: frame.eventInfo)
            .Update("spawn_animations", human.HumanAnimation.AnimationExists("rise_2") ? new[] { "rise_2" } : null)
            .Update("move_on_start", frame.eventInt is not 0)
            .Update("orientation", controller.transform.forward));
    }

    [UsedImplicitly]
    [Description("RegisterTriggerEvent:alert")]
    public static void Alert(BaseAnimationController controller, tk2dSpriteAnimationFrame frame)
    {
        var effect = ComponentSingleton<GamePoolManager>.Instance
            .Spawn("AlertRelayerOneshot", controller.transform).GetComponent<AlertEffect>();
        effect.AlertRadius = frame.eventFloat;
        effect.Alerter = controller.gameObject;
    }
}