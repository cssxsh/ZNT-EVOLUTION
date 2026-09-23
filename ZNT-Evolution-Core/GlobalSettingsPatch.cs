using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using Rotorz.Tile;
using UnityEngine;
using UnityEngine.UI;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class GlobalSettingsPatch
{
    private static BaseUnityPlugin Plugin;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(InitManager), "Start")]
    public static IEnumerator Start(IEnumerator __result, InitManager __instance)
    {
        yield return __result;
        if (Plugin is not null) yield break;
        Plugin = EvolutionCorePlugin.Instance;
        Plugin.Config.SettingChanged += OnSettingChanged;
        CorpsesCountMax = Plugin.Config.Bind("config", nameof(CorpsesCountMax), GameConf.MaxAliveCorpses, "尸体数量上限");
        VisionMaterialization = Plugin.Config.Bind("config", nameof(VisionMaterialization), false, "视觉射线渲染");
        NoEraseElement = Plugin.Config.Bind("config", nameof(NoEraseElement), false, "禁止擦除元件");
        DialogueRichText = Plugin.Config.Bind("config", nameof(DialogueRichText), true, "对话框富文本");
        ShowAllElement = Plugin.Config.Bind("config", nameof(ShowAllElement), false, "显示所有元件");
        ShowAllAnimationClip = Plugin.Config.Bind("config", nameof(ShowAllAnimationClip), false, "显示所有动画");
        ShowDevComponent = Plugin.Config.Bind("config", nameof(ShowDevComponent), false, "显示实验组件");
        BepInExToUnityLog = Plugin.Config.Bind("config", nameof(BepInExToUnityLog), false, "写入内部日志");
    }

    private static void OnSettingChanged(object sender, SettingChangedEventArgs e)
    {
        switch (e.ChangedSetting)
        {
            case ConfigEntry<bool> { Definition.Key: nameof(BepInExToUnityLog) } log:
                BepInEx.Logging.Logger.UnityLog = log.Value;
                break;
            case ConfigEntry<bool> { Definition.Key: nameof(ShowDevComponent) } dev:
                UserManager.UserDev = dev.Value;
                break;
        }
    }

    #region UnityLogListener

    internal static ConfigEntry<bool> BepInExToUnityLog;

    extension(BepInEx.Logging.Logger)
    {
        private static Action<string> WriteStringToUnityLog
        {
            get => Traverse.Create(typeof(UnityLogListener))
                .Field<Action<string>>("WriteStringToUnityLog").Value;
            set => Traverse.Create(typeof(UnityLogListener))
                .Field<Action<string>>("WriteStringToUnityLog").Value = value;
        }

        [UsedImplicitly]
        internal static bool UnityLog
        {
            get => BepInEx.Logging.Logger.WriteStringToUnityLog is not null;
            set
            {
                if (value)
                {
                    var WriteStringToUnityLogImpl = Type
                        .GetType("UnityEngine.UnityLogWriter, UnityEngine.CoreModule").GetTypeInfo()
                        .GetDeclaredMethod("WriteStringToUnityLogImpl");
                    BepInEx.Logging.Logger.WriteStringToUnityLog =
                        (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), WriteStringToUnityLogImpl);
                }
                else
                {
                    BepInEx.Logging.Logger.WriteStringToUnityLog = null;
                }
            }
        }
    }

    #endregion

    #region CorpseBehaviour

    internal static ConfigEntry<int> CorpsesCountMax;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CorpseBehaviour), "AddAliveCorpse")]
    public static IEnumerator AddAliveCorpse(IEnumerator __result, CorpseBehaviour __instance)
    {
        if (CorpsesCountMax.Value < 0) yield break;
        var parameters = __instance.Parameters;
        if (parameters.Rise) yield break;
        yield return Wait.ForFiveSeconds;
        var corpses = CorpseBehaviour.AliveCorpses;
        corpses.Enqueue(__instance);
        if (corpses.Count <= CorpsesCountMax.Value) yield break;
        corpses.Dequeue().Dissolve();
    }

    #endregion

    #region RayConeDetection

    internal static ConfigEntry<bool> VisionMaterialization;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RayConeDetection), "UpdateAngles")]
    public static void UpdateAngles(RayConeDetection __instance, out bool __state, bool force)
    {
        __state = force
                  || __instance.NeedUpdate
                  || __instance.PreviousForward != __instance.transform.forward;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RayConeDetection), "UpdateAngles")]
    public static void UpdateAngles(RayConeDetection __instance, bool __state)
    {
        if (!__state) return;
        if (!VisionMaterialization.Value) return;
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
        var rays = __instance.Rays;
        var inverted = __instance.Inverted;
        for (var i = 0; i < __instance.RayCount; i++)
        {
            var laser = __instance.Origin.GetChild(i);
            laser.right = rays[i] * inverted;
            var attachment = laser.GetComponent<LaserAttachment>();
            attachment.MaxDistance = __instance.Distance;
            attachment.ObstacleLayers = __instance.Trigger.Layers;
            laser.gameObject.SetActive(true);
            laser.BroadcastMessage(methodName: "Update");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RayConeDetection), "ResetDeviatonAngle")]
    public static void OnDespawned(RayConeDetection __instance)
    {
        var mask = (LayerMask)LayerMask.GetMask("Stairs Top", "Gameplay", "Crate");
        foreach (var attachment in __instance.Origin.GetComponentsInChildren<LaserAttachment>())
        {
            attachment.ObstacleLayers = mask;
            ComponentSingleton<GamePoolManager>.Instance.Despawn(attachment);
        }
    }

    #endregion

    #region LevelElement

    internal static ConfigEntry<bool> ShowAllElement;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LevelElement), "Useable", MethodType.Getter)]
    public static bool Usable(bool __result) => ShowAllElement.Value || __result;

    internal static ConfigEntry<bool> NoEraseElement;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LevelEditorManager), "PaintTile")]
    public static bool PaintTile(TileSystem system, LevelElement element, TileIndex index)
    {
        var a = new TileIndex(
            row: index.row - (int)element.Pivot.y,
            column: index.column - (int)element.Pivot.x);
        var b = new TileIndex(
            row: index.row + (int)element.Size.y - (int)element.Pivot.y - 1,
            column: index.column + (int)element.Size.x - (int)element.Pivot.x - 1);
        system.ClampIndex(ref a);
        system.ClampIndex(ref b);
        for (var row = a.row; row <= b.row; row++)
        {
            for (var column = a.column; column <= b.column; column++)
            {
                if (EraseTile(system, new TileIndex(row, column))) continue;
                return false;
            }
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LevelEditorManager), "EraseTile")]
    public static bool EraseTile(TileSystem tileSystem, TileIndex index)
    {
        if (tileSystem.GetTileOrNull(index) is not { HasGameObject: true } tile) return true;
        var o = tile.brush == LevelElement.ExtentBrush
            ? tile.GetGameObject().GetComponent<TileExtent>().ParentObject
            : tile.GetGameObject();
        var settings = o?.GetComponent<ObjectSettings>();
        if (settings is null) return true;
        // TODO check lock
        return !NoEraseElement.Value;
    }

    #endregion

    #region Dialogue

    internal static ConfigEntry<bool> DialogueRichText;

    private static readonly Regex EmoteRegex = new(@"\[[^]]+\]", RegexOptions.Compiled);

    extension(Dialogue dialogue)
    {
        private TMPro.TextMeshProUGUI Text =>
            Traverse.Create(dialogue).Field<TMPro.TextMeshProUGUI>("text").Value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Dialogue), "SetText")]
    private static void SetText(Dialogue __instance)
    {
        var tm = __instance.Text;
        tm.richText = DialogueRichText.Value;
        if (!tm.richText) return;
        tm.text = EmoteRegex.Replace(tm.text, EmoteEvaluator);
    }

    private static string EmoteEvaluator(Match emote)
    {
        // ReSharper disable once InvertIf
        if (TMPro.MaterialReferenceManager.TryGetSpriteAsset(160120832, out var bilibili))
        {
            var index = bilibili.GetSpriteIndexFromName(emote.Value);
            if (index is not -1) return $"""<sprite="bilibili" index={index}>""";
        }

        // ReSharper disable once InvertIf
        if (TMPro.MaterialReferenceManager.TryGetSpriteAsset(-2023423273, out var arknights))
        {
            var index = arknights.GetSpriteIndexFromName(emote.Value);
            if (index is not -1) return $"""<sprite="arknights" index={index}>""";
        }

        return emote.Value;
    }

    #endregion

    #region UserManager

    internal static ConfigEntry<bool> ShowDevComponent;

    extension(UserManager)
    {
        [UsedImplicitly]
        internal static bool UserDev
        {
            get => UserManager.IsUserDev;
            set => Traverse.Create(typeof(UserManager)).Field<bool>(nameof(UserManager.IsUserDev)).Value = value;
        }
    }

    extension(EditChapterMenu menu)
    {
        private Dropdown SourceDropdown => Traverse.Create(menu).Field<Dropdown>("sourceDropdown").Value;
    }

    extension(LoadLevelMenu menu)
    {
        private Dropdown SourceDropdown => Traverse.Create(menu).Field<Dropdown>("sourceDropdown").Value;
    }

    extension(NewLevelMenu menu)
    {
        private Dropdown LevelSource => Traverse.Create(menu).Field<Dropdown>("levelSource").Value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EditChapterMenu), "OnCreate")]
    [HarmonyPatch(typeof(LoadLevelMenu), "OnCreate")]
    [HarmonyPatch(typeof(NewLevelMenu), "Start")]
    public static void OnCreate(BaseComponent __instance)
    {
        // Custom Levels
        var dropdown = __instance switch
        {
            EditChapterMenu menu => menu.SourceDropdown,
            LoadLevelMenu menu => menu.SourceDropdown,
            NewLevelMenu menu => menu.LevelSource,
            _ => throw new ArgumentException(__instance?.name, nameof(__instance))
        };
        dropdown.value = 1;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LocalizableString), MethodType.Constructor)]
    public static void LocalizableString(LocalizableString __instance)
    {
        __instance.Localize = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LocalizableStringMenu), "UpdateMenu")]
    public static void UpdateMenu(LocalizableStringMenu __instance)
    {
        if (UserManager.UserDev) return;
        __instance.LocalizeToggle.isOn = false;
        __instance.ToggleGroup.interactable = false;
        __instance.ToggleGroup.alpha = 0.0f;
    }

    #endregion

    #region PatrolAnimationUi

    internal static ConfigEntry<bool> ShowAllAnimationClip;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PatrolAnimationUi), "Clips", MethodType.Getter)]
    public static void GetClips(PatrolAnimationUi __instance, List<Dropdown.OptionData> __result)
    {
        if (!ShowAllAnimationClip.Value) return;
        __result.Clear();
        var options = __instance.Action.Patroller.Animator.AnimationLibrary.clips
            .Where(clip => !clip.Empty)
            .OrderBy(clip => clip.name)
            .Select(clip => new Dropdown.OptionData(text: clip.name));
        __result.AddRange(options);
    }

    #endregion
}