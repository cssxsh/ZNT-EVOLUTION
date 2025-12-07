using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class GlobalSettingsPatch
{
    #region CorpseBehaviour

    private static int CorpsesCountMax => EvolutionCorePlugin.CorpsesCountMax.Value;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CorpseBehaviour), "AddAliveCorpse")]
    public static IEnumerator AddAliveCorpse(IEnumerator __result, CorpseBehaviour __instance)
    {
        if (CorpsesCountMax < 0) yield break;
        var parameters = Traverse.Create(__instance).Field<CorpseParameter>("parameters").Value;
        if (parameters.Rise) yield break;
        yield return Wait.ForFiveSeconds;
        var corpses = Traverse.Create<CorpseBehaviour>().Field<Queue<CorpseBehaviour>>("aliveCorpses").Value;
        corpses.Enqueue(__instance);
        if (corpses.Count <= CorpsesCountMax) yield break;
        corpses.Dequeue().Dissolve();
    }

    #endregion

    #region RayConeDetection

    private static bool VisionMaterialization => EvolutionCorePlugin.VisionMaterialization.Value;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RayConeDetection), "UpdateAngles")]
    public static void UpdateAngles(RayConeDetection __instance, out bool __state, bool force)
    {
        __state = force
                  || Traverse.Create(__instance).Field<bool>("needUpdate").Value
                  || __instance.transform.forward != Traverse.Create(__instance).Field<Vector3>("previousFoward").Value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RayConeDetection), "UpdateAngles")]
    public static void UpdateAngles(RayConeDetection __instance, bool __state)
    {
        if (!__state) return;
        if (!VisionMaterialization) return;
        var rays = Traverse.Create(__instance).Field<Vector2[]>("rays").Value;
        for (var i = __instance.Origin.childCount; i < __instance.RayCount; i++)
        {
            var laser = ComponentSingleton<GamePoolManager>.Instance
                .Spawn(nameof(LaserAttachment), __instance.Origin);
            var renderer = laser.GetComponentInChildren<LaserRenderer>();
            renderer.Color = __instance.GetComponentInParent<BaseBehaviour>() switch
            {
                HumanBehaviour => Color.white,
                ZombieBehaviour => Color.yellow,
                PropBehaviour => Color.red,
                _ => Color.gray
            };
        }

        for (var i = 0; i < __instance.Origin.childCount; i++)
        {
            __instance.Origin.GetChild(i).gameObject.SetActive(false);
        }

        if (!__instance.Trigger.enabled) return;
        var inverted = Traverse.Create(__instance).Field<int>("inverted").Value;
        for (var i = 0; i < __instance.RayCount; i++)
        {
            var laser = __instance.Origin.GetChild(i);
            laser.right = rays[i] * inverted;
            var attachment = laser.GetComponent<LaserAttachment>();
            attachment.MaxDistance = __instance.Distance;
            Traverse.Create(attachment).Field<LayerMask>("obstacleLayers").Value = __instance.Trigger.Layers;
            laser.gameObject.SetActive(true);
            laser.BroadcastMessage(methodName: "Update");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RayConeDetection), "ResetDeviatonAngle")]
    public static void OnDespawned(RayConeDetection __instance)
    {
        foreach (var attachment in __instance.Origin.GetComponentsInChildren<LaserAttachment>())
        {
            ComponentSingleton<GamePoolManager>.Instance.Despawn(attachment);
        }
    }

    #endregion

    #region LevelElement

    private static bool ShowAllElement => EvolutionCorePlugin.ShowAllElement.Value;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelElement), "Useable", MethodType.Getter)]
    public static bool Usable(bool __result) => ShowAllElement || __result;

    #endregion

    #region UserManager

    private static bool ShowDevComponent => EvolutionCorePlugin.ShowDevComponent.Value;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SignalSenderLinker), "Start")]
    [HarmonyPatch(typeof(SignalReceiverLinker), "OnAwake")]
    [HarmonyPatch(typeof(EditorComponent), "FromComponent")]
    [HarmonyPatch(typeof(SerializableComponent), "SetComponentValues")]
    public static void IsUserDev(out bool __state)
    {
        __state = UserManager.IsUserDev;
        Traverse.Create(typeof(UserManager)).Field<bool>(nameof(UserManager.IsUserDev)).Value |= ShowDevComponent;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SignalSenderLinker), "Start")]
    [HarmonyPatch(typeof(SignalReceiverLinker), "OnAwake")]
    [HarmonyPatch(typeof(EditorComponent), "FromComponent")]
    [HarmonyPatch(typeof(SerializableComponent), "SetComponentValues")]
    public static void IsUserDev(bool __state)
    {
        Traverse.Create(typeof(UserManager)).Field<bool>(nameof(UserManager.IsUserDev)).Value = __state;
    }

    #endregion

    #region PatrolAnimationUi

    private static bool ShowAllAnimationClip => EvolutionCorePlugin.ShowAllAnimationClip.Value;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PatrolAnimationUi), "Clips", MethodType.Getter)]
    public static void GetClips(PatrolAnimationUi __instance, List<Dropdown.OptionData> __result)
    {
        if (!ShowAllAnimationClip) return;
        var action = Traverse.Create(__instance).Field<PatrolAction>("Action").Value;
        var animation = action.Patroller.Animator.AnimationLibrary;
        if (__result.Count == animation.clips.Count(clip => !string.IsNullOrEmpty(clip.name))) return;
        __result.Clear();
        __result.AddRange(animation.clips
            .Where(clip => !string.IsNullOrEmpty(clip.name))
            .OrderBy(clip => clip.name)
            .Select(clip => new Dropdown.OptionData(text: clip.name))
        );
    }

    #endregion
}