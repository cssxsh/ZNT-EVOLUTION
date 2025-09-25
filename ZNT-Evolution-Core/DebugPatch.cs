using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    internal static class DebugPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneLoader), "LoadNextScene")]
        public static void LoadNextScene(ref string sceneName)
        {
            BepInEx.Logging.Logger.CreateLogSource("SceneLoader")
                .LogInfo($"LoadNextScene: {sceneName}");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomAssetObject), "LoadFromAsset")]
        public static void LoadFromAsset(CustomAssetObject __instance, GameObject gameObject)
        {
            BepInEx.Logging.Logger.CreateLogSource("CustomAssetObject")
                .LogDebug($"LoadFromAsset: {gameObject} for {__instance}");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Challenge), "IsFailed")]
        [HarmonyPatch(typeof(Challenge), "IsCompleted")]
        public static void IsCompleted(Challenge __instance)
        {
            if (Traverse.Create(__instance).Field<List<ChallengeRule>>("checkList").Value != null) return;
            __instance.Initialize();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Material), "GetTexture", typeof(string))]
        public static bool GetTexture(Material __instance, string name) => __instance.HasProperty(name);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(I2.Loc.LocalizationManager), "GetTermTranslation")]
        public static string GetTermTranslation(string __result, string Term) => __result ?? Term;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpawnCharacterChooser), "OnCreate")]
        public static void OnCreate(SpawnCharacterChooser __instance)
        {
            var spawn = Traverse.Create(__instance).Field("spawn").Field<Enum>("spawnType").Value;
            var characters = Traverse.Create(__instance).Field<List<CharacterAsset>>("selectableCharacters").Value;
            switch (spawn.ToString())
            {
                case "Human":
                    characters.AddRange(LevelElementIndex.Index.Values.Cast<LevelElement>()
                        .Where(element => element.Useable)
                        .Select(element => element.CustomAsset)
                        .OfType<HumanAsset>()
                        .Where(asset => !characters.Contains(asset))
                        .Distinct());
                    break;
                case "Zombie":
                    characters.AddRange(LevelElementIndex.Index.Values.Cast<LevelElement>()
                        .Where(element => element.Useable)
                        .Select(element => element.CustomAsset)
                        .OfType<ZombieAsset>()
                        .Where(asset => !characters.Contains(asset))
                        .Distinct());
                    break;
                default:
                    characters.AddRange(LevelElementIndex.Index.Values.Cast<LevelElement>()
                        .Where(element => element.Useable)
                        .Select(element => element.CustomAsset)
                        .OfType<CharacterAsset>()
                        .Where(asset => !characters.Contains(asset))
                        .Distinct());
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RayConeDetection), "UpdateAngles", typeof(bool))]
        public static void UpdateAngles(RayConeDetection __instance)
        {
            var rays = Traverse.Create(__instance).Field<Vector2[]>("rays").Value;
            rays.OrderBy(ray => Guid.NewGuid()).ToArray().CopyTo(rays);
            // if (__instance.GetComponentInParent<CharacterBehaviour>() is ZombieBehaviour) return;
            // var human = (HumanAsset)
            //     ((LevelElement)LevelElementIndex.Index["837e592ae7c270b4089d9222b1f72146"]).CustomAsset;
            // for (var i = __instance.transform.childCount; i < __instance.RayCount; i++)
            // {
            //     var laser = UnityEngine.Object.Instantiate(human.Attachments["attach_laser"], __instance.transform);
            //     laser.layer = LayerMask.NameToLayer("Renderer");
            // }
            //
            // for (var i = 0; i < __instance.transform.childCount; i++)
            // {
            //     __instance.transform.GetChild(i).gameObject.SetActive(false);
            // }
            //
            // var inverted = Traverse.Create(__instance).Field<int>("inverted").Value;
            // for (var i = 0; i < rays.Length; i++)
            // {
            //     var laser = __instance.transform.GetChild(i);
            //     laser.gameObject.SetActive(true);
            //     laser.right = rays[i] * inverted;
            //     laser.GetComponent<LaserAttachment>().MaxDistance = __instance.Distance;
            //     laser.GetComponent<LaserRenderer>().Color = Color.red;
            // }
        }
    }
}