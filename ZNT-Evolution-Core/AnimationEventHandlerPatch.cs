using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;

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
            controller.transform.position + point.position,
            controller.transform.forward,
            frame.eventInt
        );
    }
}