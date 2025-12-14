using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UIWidgets;
using UnityEngine;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal class SnakeFeetPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(HumanBehaviour), "OnAttackHit")]
    public static void OnAttackHit(HumanBehaviour __instance)
    {
        if (__instance.Frozen || !__instance.Vision.enabled) return;
        if (__instance.Weapon.Attack.HasTarget()) return;
        var frequency = __instance.Vision.Frequency;
        __instance.Vision.Frequency = 1748;
        __instance.Vision.Update();
        __instance.Vision.Frequency = frequency;
    }

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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpawnPoint), "OverrideMemberUi")]
    public static bool OverrideMemberUi(SpawnPoint __instance) => __instance is CharacterSpawnPoint;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RayConeDetection), "FindGameObjects")]
    public static void FindGameObjects(RayConeDetection __instance, C5.HashedArrayList<GameObject> __result)
    {
        if (__instance.CastAll) return;
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
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LevelSettingsMenu), "InitGeneralSettings")]
    public static void InitGeneralSettings(LevelSettingsMenu __instance)
    {
        Traverse.Create(__instance).Field<Spinner>("maxZombieSpinner").Value.Max = 1024;
        Traverse.Create(__instance).Field<Spinner>("maxEnemySpinner").Value.Max = 1024;
        Traverse.Create(__instance).Field<SpinnerFloat>("maxZoomSpinner").Value.Max = 1024f;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EditorComponent), "FromComponent")]
    public static void FromComponent(BaseComponent component, EditorComponent __result)
    {
        if (__result == null) return;
        switch (component)
        {
            case Patroller patroller:
            {
                var editing = typeof(Patroller).GetTypeInfo().GetDeclaredField("editing");
                __result.Fields.Remove(editing);
                __result.Fields[typeof(Patroller).GetField(nameof(Patroller.Voice))] = patroller.Voice;
                __result.Fields[editing] = editing.GetValue(component);
            }
                break;
            case HumanBehaviour human:
            {
                __result.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.ResistScream))] = human.ResistScream;
                __result.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.AllowMultipleAttackers))] = human.AllowMultipleAttackers;
                __result.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.GrabbedOnAttacked))] = human.GrabbedOnAttacked;
                __result.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.IgnoreDamages))] = human.IgnoreDamages;
                __result.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.InvincibleOnAttack))] = human.InvincibleOnAttack;
                __result.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.FleeBeforeZombieExplode))] = human.FleeBeforeZombieExplode;
                __result.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.MoveTowardStaticTargets))] = human.MoveTowardStaticTargets;
                __result.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.VisionFollowTarget))] = human.VisionFollowTarget;
                __result.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.Attitude))] = human.Attitude;
            }
                break;
        }
    }
}