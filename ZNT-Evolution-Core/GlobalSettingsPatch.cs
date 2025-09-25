using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
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
            var aliveCorpses = Traverse.Create<CorpseBehaviour>().Field<Queue<CorpseBehaviour>>("aliveCorpses").Value;
            aliveCorpses.Enqueue(corpseBehaviour);
            if (CorpsesCountMax < 0) yield break;
            if (aliveCorpses.Count <= CorpsesCountMax) yield break;
            aliveCorpses.Dequeue().Dissolve();
        }

        #endregion

        #region CharacterBehaviour

        private static bool RayConeFindNearest => EvolutionCorePlugin.RayConeFindNearest.Value;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RayConeDetection), "FindGameObjects")]
        public static void FindGameObjects(RayConeDetection __instance, C5.HashedArrayList<GameObject> __result)
        {
            if (__instance.CastAll) return;
            if (!RayConeFindNearest) return;

            var rays = Traverse.Create(__instance).Field<Vector2[]>("rays").Value;
            var inverted = Traverse.Create(__instance).Field<int>("inverted").Value;
            __result.Clear();
            foreach (var ray in rays)
            {
                DetectionHelper.RayCast(
                    __result,
                    __instance.Origin.position,
                    ray * inverted,
                    __instance.Distance,
                    __instance.Trigger.IgnoreLayers,
                    __instance.Trigger.Layers,
                    __instance.Trigger.IgnoreWithTags,
                    __instance.Trigger.WithTags,
                    __instance.Trigger.IgnoreWithoutTags,
                    __instance.Trigger.WithoutTags,
                    __instance.Trigger.WithAllTags,
                    __instance.Trigger.WithoutAllTags,
                    __instance.Trigger.InvertTagsMatch);
            }

            if (__result.IsEmpty) return;
            var nearest = __result
                .OrderBy(target => Vector3.Distance(__instance.Origin.position, target.transform.position))
                .First();
            __result.Clear();
            __result.Add(nearest);
        }

        #endregion

        #region LevelElement

        private static bool ShowAllElement => EvolutionCorePlugin.ShowAllElement.Value;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelElement), "Useable", MethodType.Getter)]
        public static void Usable(LevelElement __instance, ref bool __result) => __result = ShowAllElement || __result;

        #endregion
    }
}