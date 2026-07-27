using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using UIWidgets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZNT.Evolution.Core.Editor;
using ZNT.Evolution.Core.Mod;
using ZNT.LevelEditor;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class SceneLoaderPatch
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(SceneLoader));

    private static void ToggleActivation(this RectTransform transform)
    {
        transform.gameObject.SetActive(!transform.gameObject.activeSelf);
    }

    private static void OnClick(this Button button, UnityAction call)
    {
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(call);
    }

    private static void OnValueChanged(this Toggle toggle, UnityAction<bool> call)
    {
        toggle.onValueChanged = new Toggle.ToggleEvent();
        toggle.onValueChanged.AddListener(call);
    }

    private static void OnEndEdit(this InputField input, UnityAction<string> call)
    {
        input.onEndEdit = new InputField.SubmitEvent();
        input.onEndEdit.AddListener(call);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SceneLoader), "LoadNextScene")]
    public static void LoadNextScene(string sceneName)
    {
        Logger.LogInfo($"LoadNextScene: {sceneName}");
    }

    #region LanguageSource

    private static I2.Loc.LanguageSourceData _localization;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(I2.Loc.LocalizationManager), "UpdateSources")]
    public static void UpdateSources()
    {
        if (_localization != null) return;
        var localization = new I2.Loc.LanguageSourceData
        {
            GoogleUpdateFrequency = I2.Loc.LanguageSourceData.eGoogleUpdateFrequency.Never,
            GoogleInEditorCheckFrequency = I2.Loc.LanguageSourceData.eGoogleUpdateFrequency.Never
        };
        try
        {
            using var fs = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ZNT.Evolution.Core.Resources.Evolution.csv");
            using var reader = new StreamReader(fs ?? throw new FileNotFoundException("Evolution.csv"));
            localization.Import_CSV(Category: "Evolution", CSVstring: reader.ReadToEnd());
        }
        catch (FileNotFoundException e)
        {
            Logger.LogError(e);
        }

        I2.Loc.LocalizationManager.Sources.Add(_localization = localization);
        Logger.LogInfo("Evolution LanguageSource Loaded.");
    }

    private static I2.Loc.TermData GetTermData(this ModMetadata metadata)
    {
        var term = _localization.GetTermData($"Evolution/{metadata.Id}");
        if (term != null) return term;
        term = _localization.AddTerm($"Evolution/{metadata.Id}");
        term.SetTranslation(0, $"{metadata.Name} {metadata.Version}");
        return term;
    }

    #endregion

    #region SettingsScene

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SettingsMenu), "OnCreate")]
    public static void SettingsScene(SettingsMenu __instance)
    {
        Logger.LogInfo("Update SettingsScene");
        __instance.AddMod();
        __instance.AddPlugin();
    }

    private static void AddMod(this SettingsMenu menu)
    {
        var panel = menu.AddPanel("Mod");
        var impl = menu.transform
            .Find("Option Panels/Video/Scroll Area/ScrollView/Content/FullScreen Entry").gameObject;
        var content = panel.GetComponentInChildren<VerticalLayoutGroup>();

        var push = (ModContext context) =>
        {
            var item = Object.Instantiate(original: impl, parent: content.transform);
            item.name = $"{context.Metadata.Name} Entry";
            item.SetActive(false);
            var localize = item.GetComponentInChildren<I2.Loc.Localize>(includeInactive: true);
            localize.Term = context.Metadata.GetTermData().Term;
            var toggle = item.GetComponentInChildren<Toggle>(includeInactive: true);
            toggle.SetIsOnWithoutNotify(context.Loaded);
            toggle.OnValueChanged(value =>
            {
                switch (value)
                {
                    case true when context.IsLoadReady():
                        context.Load().ContinueWith(task =>
                        {
                            if (task.Exception != null) Logger.LogError(task.Exception);
                            toggle.SetIsOnWithoutNotify(context.Loaded);
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                        break;
                    case false when context.IsUnloadReady():
                        context.Unload().ContinueWith(task =>
                        {
                            if (task.Exception != null) Logger.LogError(task.Exception);
                            toggle.SetIsOnWithoutNotify(context.Loaded);
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                        break;
                    default:
                        toggle.SetIsOnWithoutNotify(context.Loaded);
                        break;
                }
            });
            item.SetActive(true);
        };
        foreach (var context in ModContext.Allocated()) push.Invoke(context);

        var reload = panel.transform.Find("Reset Entry").GetComponentInChildren<Button>();
        reload.OnClick(() =>
        {
            Logger.LogInfo("Reloading Mods Folder");
            content.transform.DestroyChildren();
            ModManager.ReloadAll().ContinueWith(task =>
            {
                if (task.Exception != null) Logger.LogError(task.Exception);
                foreach (var context in ModContext.Allocated()) push.Invoke(context);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        });
    }

    private static void AddPlugin(this SettingsMenu menu)
    {
        var panel = menu.AddPanel("Plugin");
        var content = panel.GetComponentInChildren<VerticalLayoutGroup>();

        foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
        {
            if (!info.Metadata.GUID.Contains("znt")) continue;
            foreach (var (definition, entry) in info.Instance.Config)
            {
                var term = _localization.AddTerm($"{info.Metadata.Name}/{definition}");
                term.SetTranslation(0, $"[{info.Metadata.Name}] {definition.Key}");
                term.SetTranslation(9, $"[{info.Metadata.Name}] {entry.Description.Description}");
                if (entry.SettingType == typeof(bool))
                {
                    var fullscreen = menu.transform
                        .Find("Option Panels/Video/Scroll Area/ScrollView/Content/FullScreen Entry").gameObject;
                    var item = Object.Instantiate(original: fullscreen, parent: content.transform);
                    item.name = $"{info.Metadata.Name} {definition} Entry";
                    item.SetActive(false);
                    var localize = item.GetComponentInChildren<I2.Loc.Localize>(includeInactive: true);
                    localize.Term = term.Term;
                    var toggle = item.GetComponentInChildren<Toggle>(includeInactive: true);
                    toggle.OnValueChanged(value => entry.BoxedValue = value);
                    toggle.SetIsOnWithoutNotify((bool)entry.BoxedValue);
                    item.SetActive(true);
                }
                else if (entry.SettingType == typeof(int))
                {
                    var fps = menu.transform
                        .Find("Option Panels/Video/Scroll Area/ScrollView/Content/Max FPS Entry").gameObject;
                    var item = Object.Instantiate(original: fps, parent: content.transform);
                    item.name = $"{info.Metadata.Name} {definition} Entry";
                    item.SetActive(false);
                    var localize = item.GetComponentInChildren<I2.Loc.Localize>(includeInactive: true);
                    localize.Term = term.Term;
                    var canvas = item.GetComponentInChildren<CanvasGroup>(includeInactive: true);
                    var toggle = item.GetComponentInChildren<Toggle>(includeInactive: true);
                    var input = item.GetComponentInChildren<InputField>(includeInactive: true);
                    canvas.interactable = true;
                    toggle.OnValueChanged(value =>
                    {
                        entry.BoxedValue = (value ? 0 : int.MinValue) | ((int)entry.BoxedValue & int.MaxValue);
                        input.interactable = value;
                    });
                    toggle.SetIsOnWithoutNotify((int)entry.BoxedValue >= 0);
                    input.OnEndEdit(value =>
                    {
                        if (string.IsNullOrEmpty(value)) entry.BoxedValue = entry.DefaultValue;
                        else entry.SetSerializedValue(value);
                    });
                    input.SetTextWithoutNotify(((int)entry.BoxedValue & int.MaxValue).ToString());
                    input.interactable = toggle.isOn;
                    item.SetActive(true);
                }
            }
        }

        var reset = panel.transform.Find("Reset Entry").GetComponentInChildren<Button>();
        reset.OnClick(() =>
        {
            foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
            {
                if (!info.Metadata.GUID.Contains("znt")) continue;
                foreach (var (definition, entry) in info.Instance.Config)
                {
                    entry.BoxedValue = entry.DefaultValue;
                    var item = content.transform.Find($"{info.Metadata.Name} {definition} Entry");
                    switch (entry.BoxedValue)
                    {
                        case bool value:
                            item.GetComponentInChildren<Toggle>(includeInactive: true)
                                .SetIsOnWithoutNotify(value);
                            break;
                        case int value:
                            item.GetComponentInChildren<InputField>(includeInactive: true)
                                .SetTextWithoutNotify((value & int.MaxValue).ToString());
                            item.GetComponentInChildren<InputField>(includeInactive: true)
                                .interactable = value >= 0;
                            item.GetComponentInChildren<Toggle>(includeInactive: true)
                                .SetIsOnWithoutNotify(value >= 0);
                            break;
                    }
                }
            }
        });
    }

    private static GameObject AddPanel(this SettingsMenu menu, string name)
    {
        var panels = menu.transform.Find("Option Panels");
        var tabs = menu.transform.Find("Option Menu/Tabs");

        var panel = Object.Instantiate(original: panels.GetChild(0).gameObject, parent: panels);
        panel.name = name;
        var content = panel.GetComponentInChildren<VerticalLayoutGroup>();
        content.transform.DestroyChildren();
        panel.SetActive(false);

        var container = Traverse.Create(menu).Field<GameObject[]>("settingsContainer");
        var index = container.Value.Length;
        container.Value = container.Value.AddToArray(panel);

        var tab = Object.Instantiate(original: tabs.GetChild(0).gameObject, parent: tabs);
        tab.name = name;
        tab.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
            .ForEach(localize => localize.Term = $"Evolution/{name}_Tab");
        tab.GetComponentInChildren<Toggle>()
            .OnValueChanged(value => menu.ShowSettings(group: value ? index : -1));

        var reset = panel.transform.Find("Reset Entry");
        var reload = reset.GetComponentInChildren<Button>();
        reload.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
            .ForEach(localize => localize.Term = $"Evolution/{name}_Reset");
        reload.OnClick(() => Logger.LogWarning($"{name} reset no define"));

        return panel;
    }

    #endregion

    #region EditorMainScene

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SelectionMenu), "OnAwake")]
    public static void EditorMainScene(SelectionMenu __instance)
    {
        Logger.LogInfo("Update EditorMainScene");
        __instance.FixBinder();
        __instance.AddCopy();
        __instance.AddEmpty();
    }

    private static void AddCopy(this SelectionMenu menu)
    {
        var move = Traverse.Create(menu).Field<Toggle>("moveButton").Value;
        var plus = Traverse.Create(menu).Field<Button>("decorPlusButton").Value;
        var icon = plus.transform.Find("Icon").GetComponent<Image>().sprite;
        var copy = Object.Instantiate(original: move, parent: move.transform.parent);
        copy.name = "Copy Button";
        copy.transform.SetSiblingIndex(3);
        copy.transform.Find("Icon").GetComponent<Image>().sprite = icon;
        // copy.transform.Find("Selected").GetComponent<Image>().sprite = icon;
        copy.OnValueChanged(menu.CopyObject);
    }

    private static void AddEmpty(this SelectionMenu menu)
    {
        if (menu.transform.Find("Empty")) return;
        var container = Traverse.Create(menu).Field<RectTransform>("mainContainer").Value;
        var empty = Object.Instantiate(original: container, parent: menu.transform);
        empty.name = "Empty";
        empty.gameObject.SetActive(false);
    }

    private static void FixBinder(this SelectionMenu menu)
    {
        var prefabs = Traverse.Create(menu).Field<SupportedTypePrefabs>("typePrefabs").Value;
        var binder = prefabs[EditorComponent.SupportedType.Vector4];
        var fields = binder.GetComponentsInChildren<InputField>();
        // ReSharper disable once CoVariantArrayConversion
        Traverse.Create(binder).Field<UIBehaviour[]>("uiComponents").Value = fields;
    }

    private static void CopyObject(this SelectionMenu menu, bool active)
    {
        var target = Traverse.Create(menu).Field<EditorGameObject>("serializeGameObject").Value;
        target.ObjectSettings.Activate(active, active ? ObjectSettings.Control.Copy : ObjectSettings.Control.None);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SelectionMenu), "UpdateCommonMenu")]
    public static void UpdateCommonMenu(SelectionMenu __instance)
    {
        var move = Traverse.Create(__instance).Field<Toggle>("moveButton").Value;
        var copy = move.transform.parent.Find("Copy Button")?.GetComponent<Toggle>();
        copy?.isOn = false;
    }

    private static readonly HashSet<string> Activated = [];

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SelectionMenu), "UpdateComponentMenu")]
    public static bool UpdateComponentMenu(SelectionMenu __instance)
    {
        var container = Traverse.Create(__instance).Field<RectTransform>("mainContainer").Value;
        var target = Traverse.Create(__instance).Field<EditorGameObject>("serializeGameObject").Value;
        var updaters = Traverse.Create(__instance).Field<List<IEditorUpdate>>("componentsUpdate").Value;
        var scroll = Traverse.Create(__instance).Field<ScrollRect>("scrollRect").Value;
        var empty = __instance.transform.Find("Empty") as RectTransform;

        foreach (var transform in container.Cast<Transform>())
        {
            _ = transform.gameObject.activeSelf ? Activated.Add(transform.name) : Activated.Remove(transform.name);
            Object.Destroy(transform.gameObject);
        }

        container.anchoredPosition = Vector2.zero;
        updaters.Clear();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var component in target.Components)
        {
            if (component?.Data is null or ObjectSettings) continue;
            if (component.Data is IEditorUpdate updater) updaters.Add(updater);
            var overrider = component.Data as IEditorOverride;

            var header = __instance.SetComponentHeader(component).gameObject;
            header.name = $"{component.Name} Header";
            var panel = Object.Instantiate(original: empty, parent: container);
            panel.name = $"{component.Name} Panel";
            Traverse.Create(__instance).Field<RectTransform>("mainContainer").Value = panel;
            try
            {
                foreach (var (member, _) in component.Fields)
                {
                    if (overrider != null && overrider.OverrideMemberUi(__instance, component, member)) continue;
                    __instance.SetDefaultUi(component, member);
                }
            }
            finally
            {
                Traverse.Create(__instance).Field<RectTransform>("mainContainer").Value = container;
                header.AddComponent<Button>().onClick.AddListener(panel.ToggleActivation);
                header.SetActive(panel.childCount != 0);
                panel.gameObject.SetActive(panel.childCount != 0 && Activated.Contains(panel.name));
            }
        }

        scroll.Rebuild(CanvasUpdate.PostLayout);
        foreach (var updater in updaters) updater.OnEditorOpen();

        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SelectionMenu), "LateUpdate")]
    public static void LateUpdate(SelectionMenu __instance)
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            var move = Traverse.Create(__instance).Field<Toggle>("moveButton").Value;
            var panel = move.transform.parent;
            foreach (var trigger in panel.GetComponentsInChildren<Toggle>()) trigger.isOn = false;
        }
        else if (Input.GetKey(KeyCode.Delete))
        {
            var delete = Traverse.Create(__instance).Field<Button>("deleteButton").Value;
            delete.onClick.Invoke();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SelectionMenu), "SetDefaultUi")]
    public static bool SetDefaultUi(SelectionMenu __instance, EditorComponent component, MemberInfo member)
    {
        // ReSharper disable once InvertIf
        if (member.DeclaringType == typeof(MovingObjectBehaviour) &&
            member.Name is nameof(MovingObjectBehaviour.Orientation))
        {
            // Hide for ObjectOrientation.Orientation
            // __instance.DirectionBinder().BindDirection(component, member);
            return false;
        }

        // ReSharper disable once InvertIf
        if (member.DeclaringType == typeof(ObjectOrientation) &&
            member.Name is "orientation")
        {
            __instance.DirectionBinder().BindDirection(component, member);
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SupportedTypeBinder), "SetName")]
    public static bool SetName(this SupportedTypeBinder __instance, MemberInfo member)
    {
        var attribute = member.GetCustomAttribute<SerializeInEditorAttribute>();
        var name = string.IsNullOrEmpty(attribute?.Name) ? member.Name : attribute.Name;
        var text = Traverse.Create(__instance).Field<Text>("text").Value;
        text.text = name;
        text.transform.parent.name = $"{name} Input";
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SupportedTypeBinder), "BindVector4Field")]
    public static void BindVector4Field(SupportedTypeBinder __instance, EditorComponent component, MemberInfo member)
    {
        var value = member.GetMemberValue<Vector4>(component.Data);
        var components = Traverse.Create(__instance).Field<UIBehaviour[]>("uiComponents").Value;
        ((InputField)components[2]).text = $"{value.z}";
        ((InputField)components[3]).text = $"{value.w}";
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SupportedTypeBinder), "BindVector3Field")]
    public static void BindVector3Field(SupportedTypeBinder __instance, EditorComponent component, MemberInfo member)
    {
        // ReSharper disable once InvertIf
        if (member.DeclaringType == typeof(RayConeDetection) &&
            member.Name is nameof(RayConeDetection.GeneralDirection))
        {
            var input = Traverse.Create(__instance).Field<Text>("text").Value.transform.parent;
            // Hide UnityEngine.Vector3.z
            input.Find("Container/X Text").gameObject.SetActive(false);
            input.Find("Container/X Input").gameObject.SetActive(false);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SupportedTypeBinder), "BindDirection")]
    public static bool BindDirection(SupportedTypeBinder __instance, EditorComponent component, MemberInfo member)
    {
        if (member.GetMemberType() == typeof(Vector3)) return true;
        __instance.SetName(member);
        var components = Traverse.Create(__instance).Field<UIBehaviour[]>("uiComponents").Value;
        var l = (Toggle)components[0];
        var r = (Toggle)components[1];
        l.onValueChanged.RemoveAllListeners();
        r.onValueChanged.RemoveAllListeners();
        var a = Vector3.left as object;
        var b = Vector3.right as object;

        if (member.GetMemberType() == typeof(Vector2))
        {
            a = Vector2.left;
            b = Vector2.right;
        }
        else if (member.GetMemberType() == typeof(ObjectOrientation.Orientation))
        {
            a = ObjectOrientation.Orientation.Left;
            b = ObjectOrientation.Orientation.Right;
        }

        l.onValueChanged.AddListener(value => member.SetMemberValue(component.Data, value ? a : b));
        (member.GetMemberValue<object>(component.Data).Equals(a) ? l : r).isOn = true;
        return false;
    }

    #endregion
}