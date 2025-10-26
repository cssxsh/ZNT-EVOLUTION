using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UIWidgets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZNT.Evolution.Core.Asset;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class SceneLoaderPatch
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(SceneLoader));

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

    private static void OnValueChanged(this InputField input, UnityAction<string> call)
    {
        input.onValueChanged = new InputField.OnChangeEvent();
        input.onValueChanged.AddListener(call);
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
        if (I2.Loc.LocalizationManager.Sources
            .SelectMany(loaded => loaded.GetCategories())
            .Any(category => category == "Evolution"))
        {
            return;
        }

        _localization ??= new I2.Loc.LanguageSourceData();
        try
        {
            _localization.Import_CSV(Category: "Evolution", CSVstring: Resource("Evolution.csv"));
        }
        catch (FileNotFoundException e)
        {
            Logger.LogError(e);
        }

        I2.Loc.LocalizationManager.Sources.Add(_localization);
        Logger.LogInfo("Evolution LanguageSource Loaded.");
    }

    private static string Resource(string name)
    {
        var assembly = typeof(EvolutionCorePlugin).Assembly;
        var path = assembly.GetManifestResourceNames().FirstOrDefault(path => path.EndsWith(name));
        using var fs = assembly.GetManifestResourceStream(path) ?? throw new FileNotFoundException(name);
        using var reader = new StreamReader(fs);
        return reader.ReadToEnd();
    }

    private static I2.Loc.TermData GetTermData(this LevelElement element)
    {
        var term = _localization.GetTermData($"Evolution/{element.name}");
        if (term != null) return term;
        term = _localization.AddTerm($"Evolution/{element.name}");
        var info = element.ElementType switch
        {
            LevelElement.Type.Brush => $"{element.Title} [{element.AllowedTileSystems}]",
            LevelElement.Type.Decor => $"{element.Title} [{element.AllowedDecorSystems}]",
            _ => throw new ArgumentOutOfRangeException(element.name)
        };
        term.SetTranslation(0, info);

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

        foreach (var element in AssetElementBinder.BoundLevelElements())
        {
            var item = UnityEngine.Object.Instantiate(original: impl, parent: content.transform);
            item.name = $"{element.Title} Entry";
            item.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
                .ForEach(localize => localize.Term = element.GetTermData().Term);
            item.SetActive(true);
            var toggle = item.GetComponentInChildren<Toggle>(includeInactive: true);
            var enable = Traverse.Create(element).Field<bool>("useable");
            toggle.OnValueChanged(value => enable.Value = value);
            toggle.SetIsOnWithoutNotify(enable.Value);
        }

        var reload = panel.transform.Find("Reset Entry").GetComponentInChildren<Button>();
        reload.OnClick(() =>
        {
            Logger.LogInfo("Reload MOD ...");
            // reload.interactable = false;
        });
    }

    private static void AddPlugin(this SettingsMenu menu)
    {
        var panel = menu.AddPanel("Plugin");
        var content = panel.GetComponentInChildren<VerticalLayoutGroup>();

        foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
        {
            if (info.Metadata.Name == "UnityExplorer") continue;
            foreach (var (definition, entry) in info.Instance.Config)
            {
                var term = _localization.GetTermData($"{info.Metadata.Name}/{definition}")
                           ?? _localization.AddTerm($"{info.Metadata.Name}/{definition}");
                term.SetTranslation(0, $"[{info.Metadata.Name}] {definition.Key}");
                term.SetTranslation(9, $"[{info.Metadata.Name}] {entry.Description.Description}");
                if (entry.SettingType == typeof(bool))
                {
                    var fullscreen = menu.transform
                        .Find("Option Panels/Video/Scroll Area/ScrollView/Content/FullScreen Entry").gameObject;
                    var item = UnityEngine.Object.Instantiate(original: fullscreen, parent: content.transform);
                    item.name = $"{info.Metadata.Name} {definition} Entry";
                    item.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
                        .ForEach(localize => localize.Term = $"{info.Metadata.Name}/{definition}");
                    item.SetActive(true);
                    var toggle = item.GetComponentInChildren<Toggle>(includeInactive: true);
                    toggle.OnValueChanged(value => entry.BoxedValue = value);
                    toggle.SetIsOnWithoutNotify((bool)entry.BoxedValue);
                }
                else if (entry.SettingType == typeof(int))
                {
                    var fps = menu.transform
                        .Find("Option Panels/Video/Scroll Area/ScrollView/Content/Max FPS Entry").gameObject;
                    var item = UnityEngine.Object.Instantiate(original: fps, parent: content.transform);
                    item.name = $"{info.Metadata.Name} {definition} Entry";
                    item.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
                        .ForEach(localize => localize.Term = $"{info.Metadata.Name}/{definition}");
                    item.SetActive(true);
                    var input = item.GetComponentInChildren<InputField>(includeInactive: true);
                    input.OnValueChanged(value =>
                    {
                        if (string.IsNullOrEmpty(value)) entry.BoxedValue = entry.DefaultValue;
                        else entry.SetSerializedValue(value);
                    });
                    input.SetTextWithoutNotify(((int)entry.BoxedValue & int.MaxValue).ToString());
                    input.interactable = (int)entry.BoxedValue >= 0;
                    var toggle = item.GetComponentInChildren<Toggle>(includeInactive: true);
                    toggle.OnValueChanged(value =>
                    {
                        entry.BoxedValue = (value ? int.MaxValue : -1) | ((int)entry.BoxedValue & int.MaxValue);
                        input.interactable = value;
                    });
                    toggle.SetIsOnWithoutNotify(input.interactable);
                }
            }
        }

        var reload = panel.transform.Find("Reset Entry").GetComponentInChildren<Button>();
        reload.OnClick(() =>
        {
            foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
            {
                var plugin = Traverse.Create(info.Instance);
                foreach (var name in plugin.Fields())
                {
                    var field = plugin.Field(name);
                    if (!typeof(ConfigEntryBase).IsAssignableFrom(field.GetValueType())) continue;
                    var entry = field.GetValue<ConfigEntryBase>();
                    entry.BoxedValue = entry.DefaultValue;
                    var item = content.transform.Find($"{info.Metadata.Name} {name} Entry");
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

        var panel = UnityEngine.Object.Instantiate(original: panels.GetChild(0).gameObject, parent: panels);
        panel.name = name;
        var content = panel.GetComponentInChildren<VerticalLayoutGroup>();
        content.transform.DestroyChildren();
        panel.SetActive(false);

        var container = Traverse.Create(menu).Field<GameObject[]>("settingsContainer");
        var index = container.Value.Length;
        container.Value = container.Value.AddToArray(panel);

        var tab = UnityEngine.Object.Instantiate(original: tabs.GetChild(0).gameObject, parent: tabs);
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
        __instance.AddCopy();
        __instance.AddEmpty();
    }

    private static void AddCopy(this SelectionMenu menu)
    {
        var move = Traverse.Create(menu).Field<Toggle>("moveButton").Value;
        var target = Traverse.Create(menu).Field<EditorGameObject>("serializeGameObject");
        var copy = UnityEngine.Object.Instantiate(original: move, parent: move.transform.parent);
        copy.name = "Copy Button";
        copy.OnValueChanged(value => target.Value.ObjectSettings.Activate(value, ObjectSettings.Control.Copy));
        var icon = copy.transform.Find("Icon").GetComponent<Image>();
        icon.sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(sprite => sprite.name == "icon_plus");
    }

    private static void AddEmpty(this SelectionMenu menu)
    {
        if (menu.transform.Find("Empty")) return;
        var container = Traverse.Create(menu).Field<RectTransform>("mainContainer").Value;
        var empty = UnityEngine.Object.Instantiate(original: container, parent: menu.transform);
        empty.name = "Empty";
        empty.gameObject.SetActive(false);
    }

    private static readonly HashSet<string> Activated = new();

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
            UnityEngine.Object.Destroy(transform.gameObject);
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
            var panel = UnityEngine.Object.Instantiate(original: empty, parent: container);
            panel.name = $"{component.Name} Panel";
            Traverse.Create(__instance).Field<RectTransform>("mainContainer").Value = panel;
            foreach (var (member, _) in component.Fields)
            {
                if (overrider != null && overrider.OverrideMemberUi(__instance, component, member)) continue;
                __instance.SetDefaultUi(component, member);
            }

            Traverse.Create(__instance).Field<RectTransform>("mainContainer").Value = container;
            header.AddComponent<Button>().onClick.AddListener(panel.ToggleActivation);
            header.SetActive(panel.childCount != 0);
            panel.gameObject.SetActive(panel.childCount != 0 && Activated.Contains(panel.name));
        }

        scroll.Rebuild(CanvasUpdate.PostLayout);
        foreach (var updater in updaters) updater?.OnEditorOpen();

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

    #endregion
}