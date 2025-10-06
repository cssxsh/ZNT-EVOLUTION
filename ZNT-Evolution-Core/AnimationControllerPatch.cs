using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    internal static class AnimationControllerPatch
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("AnimationController");

        private const string FrameEventPrefix = "RegisterTriggerEvent:";

        private static readonly List<MethodInfo> FrameEvents = new List<MethodInfo>();

        private const string ClipEventPrefix = "RegisterEndEvent:";

        private static readonly List<MethodInfo> ClipEvents = new List<MethodInfo>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BaseAnimationController), "Initialize")]
        public static void Initialize(BaseAnimationController __instance)
        {
            if (__instance.EventHandler is null) return;
            foreach (var method in FrameEvents)
            {
                if (!method.GetParameters()[0].ParameterType.IsInstanceOfType(__instance)) continue;
                foreach (var description in method.GetCustomAttributes<DescriptionAttribute>())
                {
                    var name = description.Description.Substring(FrameEventPrefix.Length);
                    Logger.LogDebug($"RegisterTriggerEvent(name='{name}') for {method.FullDescription()}");
                    __instance.EventHandler.RegisterTriggerEvent(name, frame => method.Invoke(null, new object[]
                    {
                        __instance,
                        frame
                    }));
                }
            }

            foreach (var method in ClipEvents)
            {
                if (!method.GetParameters()[0].ParameterType.IsInstanceOfType(__instance)) continue;
                foreach (var description in method.GetCustomAttributes<DescriptionAttribute>())
                {
                    var name = description.Description.Substring(ClipEventPrefix.Length);
                    Logger.LogDebug($"RegisterEndEvent(name='{name}') for {method.FullDescription()}");
                    __instance.EventHandler.RegisterEndEvent(name, () => method.Invoke(null, new object[]
                    {
                        __instance,
                        __instance.AnimationLibrary.GetClipByName(name)
                    }));
                }
            }
        }

        public static void RegisterAnimationEvent(Assembly assembly)
        {
            foreach (var type in assembly.ExportedTypes)
            {
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (var method in methods)
                {
                    var infos = method.GetParameters();
                    foreach (var description in method.GetCustomAttributes<DescriptionAttribute>())
                    {
                        switch (description)
                        {
                            case { } when description.Description.StartsWith(FrameEventPrefix):
                                if (infos.Length != 2) continue;
                                if (typeof(tk2dSpriteAnimationFrame) != infos[1].ParameterType) continue;
                                if (!typeof(BaseAnimationController).IsAssignableFrom(infos[0].ParameterType)) continue;
                                FrameEvents.Add(method);
                                Logger.LogInfo($"Cached {method.FullDescription()}");
                                break;
                            case { } when description.Description.StartsWith(ClipEventPrefix):
                                if (infos.Length != 2) continue;
                                if (typeof(tk2dSpriteAnimationClip) != infos[1].ParameterType) continue;
                                if (!typeof(BaseAnimationController).IsAssignableFrom(infos[0].ParameterType)) continue;
                                ClipEvents.Add(method);
                                Logger.LogInfo($"Cached {method.FullDescription()}");
                                break;
                        }
                    }
                }
            }
        }
    }
}