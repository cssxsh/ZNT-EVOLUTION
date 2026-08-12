using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
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
    private static ConfigFile Config => EvolutionCorePlugin.Instance.Config;

    [Harmony]
    [HarmonyPrepare]
    private static void Init()
    {
        Config.SettingChanged += OnSettingChanged;
        CorpsesCountMax = Config.Bind("config", nameof(CorpsesCountMax), GameConf.MaxAliveCorpses, "尸体数量上限");
        VisionMaterialization = Config.Bind("config", nameof(VisionMaterialization), false, "视觉射线渲染");
        NoEraseElement = Config.Bind("config", nameof(NoEraseElement), false, "禁止擦除元件");
        DialogueRichText = Config.Bind("config", nameof(DialogueRichText), true, "对话框富文本");
        ShowAllElement = Config.Bind("config", nameof(ShowAllElement), false, "显示所有元件");
        ShowAllAnimationClip = Config.Bind("config", nameof(ShowAllAnimationClip), false, "显示所有动画");
        ShowDevComponent = Config.Bind("config", nameof(ShowDevComponent), false, "显示实验组件");
        BepInExToUnityLog = Config.Bind("config", nameof(BepInExToUnityLog), false, "写入内部日志");
    }

    private static void OnSettingChanged(object sender, SettingChangedEventArgs e)
    {
        switch (e.ChangedSetting)
        {
            case { Definition.Key: nameof(BepInExToUnityLog) }:
                UnityLog = BepInExToUnityLog.Value;
                break;
            case { Definition.Key: nameof(ShowDevComponent) }:
                IsUserDev = ShowDevComponent.Value;
                break;
        }
    }

    #region UnityLogListener

    internal static ConfigEntry<bool> BepInExToUnityLog;

    private static BepInEx.Logging.UnityLogListener UnityLogListener =>
        field ??= BepInEx.Logging.Logger.Listeners.OfType<BepInEx.Logging.UnityLogListener>().Single();

    [UsedImplicitly]
    internal static bool UnityLog
    {
        get => BepInEx.Logging.Logger.Listeners.Contains(UnityLogListener);
        set
        {
            if (value == BepInEx.Logging.Logger.Listeners.Contains(UnityLogListener)) return;
            if (value) BepInEx.Logging.Logger.Listeners.Add(UnityLogListener);
            else BepInEx.Logging.Logger.Listeners.Remove(UnityLogListener);
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
        var parameters = Traverse.Create(__instance).Field<CorpseParameter>("parameters").Value;
        if (parameters.Rise) yield break;
        yield return Wait.ForFiveSeconds;
        var corpses = Traverse.Create<CorpseBehaviour>().Field<Queue<CorpseBehaviour>>("aliveCorpses").Value;
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
                  || Traverse.Create(__instance).Field<bool>("needUpdate").Value
                  || Traverse.Create(__instance).Field<Vector3>("previousFoward").Value != __instance.transform.forward;
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
        var rays = Traverse.Create(__instance).Field<Vector2[]>("rays").Value;
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
        var mask = (LayerMask)LayerMask.GetMask("Stairs Top", "Gameplay", "Crate");
        foreach (var attachment in __instance.Origin.GetComponentsInChildren<LaserAttachment>())
        {
            Traverse.Create(attachment).Field<LayerMask>("obstacleLayers").Value = mask;
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

    private static readonly Regex EmoteRegex = new("""\[[^]]+\]""", RegexOptions.Compiled);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Dialogue), "SetText")]
    private static void SetText(Dialogue __instance)
    {
        var tm = Traverse.Create(__instance).Field<TMPro.TextMeshProUGUI>("text").Value;
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

    private static HashSet<ulong> DevIds => Traverse.Create(typeof(UserManager)).Field<HashSet<ulong>>("DevIds").Value;

    [UsedImplicitly]
    internal static bool IsUserDev
    {
        get => DevIds.Contains(ComponentSingleton<SteamManager>.Instance.GetUserIdentifier().m_SteamID);
        set => Traverse.Create(typeof(UserManager)).Field<bool>(nameof(UserManager.IsUserDev)).Value = value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EditChapterMenu), "OnCreate")]
    [HarmonyPatch(typeof(LoadLevelMenu), "OnCreate")]
    [HarmonyPatch(typeof(NewLevelMenu), "Start")]
    public static void OnCreate(BaseComponent __instance)
    {
        var dropdown = __instance switch
        {
            EditChapterMenu => Traverse.Create(__instance).Field<Dropdown>("sourceDropdown").Value,
            LoadLevelMenu => Traverse.Create(__instance).Field<Dropdown>("sourceDropdown").Value,
            NewLevelMenu => Traverse.Create(__instance).Field<Dropdown>("levelSource").Value,
            _ => null
        };
        dropdown?.value = 1;
    }

    #endregion

    #region PatrolAnimationUi

    internal static ConfigEntry<bool> ShowAllAnimationClip;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PatrolAnimationUi), "Clips", MethodType.Getter)]
    public static void GetClips(PatrolAnimationUi __instance, List<Dropdown.OptionData> __result)
    {
        if (!ShowAllAnimationClip.Value) return;
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