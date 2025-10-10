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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CorpseBehaviour), "AddAliveCorpse")]
    public static bool AddAliveCorpse(CorpseBehaviour __instance, ref IEnumerator __result)
    {
        if (Traverse.Create(__instance).Field<CorpseParameter>("parameters").Value.Rise) return true;
        __result = __instance.AddAliveCorpse();
        return false;
    }

    private static IEnumerator AddAliveCorpse(this CorpseBehaviour corpseBehaviour)
    {
        yield return Wait.ForFiveSeconds;
        var aliveCorpses = Traverse.Create<CorpseBehaviour>()
            .Field<Queue<CorpseBehaviour>>("aliveCorpses").Value;
        aliveCorpses.Enqueue(corpseBehaviour);
        if (CorpsesCountMax < 0) yield break;
        if (aliveCorpses.Count <= CorpsesCountMax) yield break;
        aliveCorpses.Dequeue().Dissolve();
    }

    #endregion

    #region RayConeDetection

    private static bool VisionMaterialization => EvolutionCorePlugin.VisionMaterialization.Value;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RayConeDetection), "UpdateAngles")]
    public static void UpdateAngles(RayConeDetection __instance)
    {
        if (!VisionMaterialization) return;
        var rays = Traverse.Create(__instance).Field<Vector2[]>("rays").Value;
        for (var i = __instance.Origin.childCount; i < __instance.RayCount; i++)
        {
            var laser = ComponentSingleton<GamePoolManager>.Instance
                .Spawn("LaserAttachment", __instance.Origin);
            laser.gameObject.layer = LayerMask.NameToLayer("Renderer");
        }

        for (var i = 0; i < __instance.Origin.childCount; i++)
        {
            __instance.Origin.GetChild(i).gameObject.SetActive(false);
        }

        if (!__instance.Trigger.enabled) return;
        var inverted = Traverse.Create(__instance).Field<int>("inverted").Value;
        for (var i = 0; i < rays.Length; i++)
        {
            var laser = __instance.Origin.GetChild(i);
            laser.right = rays[i] * inverted;
            var attachment = laser.GetComponent<LaserAttachment>();
            attachment.MaxDistance = __instance.Distance;
            Traverse.Create(attachment).Field<LayerMask>("obstacleLayers").Value = __instance.Trigger.Layers;
            var renderer = laser.GetComponentInChildren<LaserRenderer>();
            renderer.Color = Color.white;
            laser.gameObject.SetActive(true);
            laser.BroadcastMessage("Update");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TriggerDetection), "OnDespawned")]
    public static void OnDespawned(TriggerDetection __instance)
    {
        if (__instance is not RayConeDetection) return;
        for (var i = 0; i < __instance.Origin.childCount; i++)
        {
            ComponentSingleton<GamePoolManager>.Instance
                .Despawn(__instance.Origin.GetChild(i));
        }
    }

    #endregion

    #region LevelElement

    private static bool ShowAllElement => EvolutionCorePlugin.ShowAllElement.Value;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelElement), "Useable", MethodType.Getter)]
    public static void Usable(LevelElement __instance, ref bool __result) => __result = ShowAllElement || __result;

    #endregion

    #region PatrolAnimationUi

    private static bool ShowAllAnimationClip => EvolutionCorePlugin.ShowAllAnimationClip.Value;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PatrolAnimationUi), "Clips", MethodType.Getter)]
    public static void Clips(PatrolAnimationUi __instance, List<Dropdown.OptionData> __result)
    {
        if (!ShowAllAnimationClip) return;
        __result.Clear();
        var action = Traverse.Create(__instance).Field<PatrolAction>("parameters").Value;
        var animation = action.Patroller.Animator.AnimationLibrary;
        __result.AddRange(animation.clips
            .Where(clip => !string.IsNullOrEmpty(clip.name))
            .OrderBy(clip => clip.name)
            .Select(clip => new Dropdown.OptionData(text: clip.name))
        );
    }

    #endregion
}