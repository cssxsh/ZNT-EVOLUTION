using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using ZNT.Evolution.Core.Editor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class AnimationEventHandlerPatch
{
    private static readonly ManualLogSource LogSource = Logger.CreateLogSource(nameof(AnimationEventHandler));

    private static readonly C5.HashedArrayList<MethodInfo> EventHandles = new();

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
                        LogSource.LogDebug($"RegisterTriggerEvent(name=\"{name}\") for {method.FullDescription()}");
                        __instance.EventHandler.RegisterTriggerEvent(name, frame => method.Invoke(null, new object[]
                        {
                            __instance,
                            frame
                        }));
                        break;
                    case nameof(AnimationEventHandler.RegisterEndEvent):
                        LogSource.LogDebug($"RegisterEndEvent(name=\"{name}\") for {method.FullDescription()}");
                        __instance.EventHandler.RegisterEndEvent(name, () => method.Invoke(null, new object[]
                        {
                            __instance,
                            __instance.AnimationLibrary.GetClipByName(name)
                        }));
                        break;
                }
            }
        }
    }

    [UsedImplicitly]
    public static void RegisterAnimationEvent(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes()) RegisterAnimationEvent(type);
    }

    [UsedImplicitly]
    public static void RegisterAnimationEvent(System.Type type)
    {
        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            var infos = method.GetParameters();
            if (infos.Length != 2) continue;
            if (!typeof(BaseAnimationController).IsAssignableFrom(infos[0].ParameterType)) continue;
            foreach (var description in method.GetCustomAttributes<DescriptionAttribute>())
            {
                var index = description.Description.IndexOf(':');
                if (index == -1) continue;
                switch (description.Description.Substring(0, index))
                {
                    case nameof(AnimationEventHandler.RegisterTriggerEvent)
                        when infos[1].ParameterType.IsAssignableFrom(typeof(tk2dSpriteAnimationFrame)):
                    case nameof(AnimationEventHandler.RegisterEndEvent)
                        when infos[1].ParameterType.IsAssignableFrom(typeof(tk2dSpriteAnimationClip)):
                        if (EventHandles.Add(method)) LogSource.LogInfo($"Cached {method.FullDescription()}");
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
        var point = definition.attachPoints.FirstOrDefault(point => point.name == "throw")
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
            if (count == 0) break;
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
        var behaviour = Traverse.Create(controller).Field<HumanBehaviour>("Behaviour").Value;
        var repulse = behaviour.Rage.GetRepulse();
        if (repulse) repulse.GetComponent<ExplosionEditor>().StartExplosion();
    }
}