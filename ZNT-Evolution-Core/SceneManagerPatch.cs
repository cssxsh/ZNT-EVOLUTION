using System;
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

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    internal static class SceneManagerPatch
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("SceneManager");

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
            var path = assembly.GetManifestResourceNames()
                           .FirstOrDefault(path => path.EndsWith(name))
                       ?? throw new FileNotFoundException(name);
            using var fs = assembly.GetManifestResourceStream(path)
                           ?? throw new FileNotFoundException(name);
            using var reader = new StreamReader(fs);
            return reader.ReadToEnd();
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
            var mod = menu.AddPanel("Mod");
            var impl = menu.transform
                .Find("Option Panels/Video/Scroll Area/ScrollView/Content/FullScreen Entry").gameObject;
            var content = mod.GetComponentInChildren<VerticalLayoutGroup>();

            foreach (var element in AssetElementBinder.LevelElements())
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

            var reload = mod.transform.Find("Reset Entry").GetComponentInChildren<Button>();
            reload.OnClick(() =>
            {
                Logger.LogInfo("Reload MOD ...");
                // TODO: ...
            });
        }

        private static void AddPlugin(this SettingsMenu menu)
        {
            var mod = menu.AddPanel("Plugin");
            var content = mod.GetComponentInChildren<VerticalLayoutGroup>();

            foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
            {
                var plugin = Traverse.Create(info.Instance);
                foreach (var name in plugin.Fields())
                {
                    var field = plugin.Field(name);
                    if (!typeof(ConfigEntryBase).IsAssignableFrom(field.GetValueType())) continue;
                    var entry = plugin.Field(name).GetValue<ConfigEntryBase>();
                    var term = _localization.GetTermData($"{info.Metadata.Name}/{name}")
                               ?? _localization.AddTerm($"{info.Metadata.Name}/{name}");
                    term.SetTranslation(0, $"[{info.Metadata.Name}] {entry.Definition.Key}");
                    term.SetTranslation(9, $"[{info.Metadata.Name}] {entry.Description.Description}");
                    if (entry.SettingType == typeof(bool))
                    {
                        var fullscreen = menu.transform
                            .Find("Option Panels/Video/Scroll Area/ScrollView/Content/FullScreen Entry").gameObject;
                        var item = UnityEngine.Object.Instantiate(original: fullscreen, parent: content.transform);
                        item.name = $"{info.Metadata.Name} {name} Entry";
                        item.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
                            .ForEach(localize => localize.Term = $"{info.Metadata.Name}/{name}");
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
                        item.name = $"{info.Metadata.Name} {name} Entry";
                        item.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
                            .ForEach(localize => localize.Term = $"{info.Metadata.Name}/{name}");
                        item.SetActive(true);
                        var input = item.GetComponentInChildren<InputField>(includeInactive: true);
                        input.OnValueChanged(value =>
                        {
                            if (value == "") value = "20";
                            entry.SetSerializedValue(value);
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

            var reload = mod.transform.Find("Reset Entry").GetComponentInChildren<Button>();
            reload.OnClick(() =>
            {
                foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
                {
                    var plugin = Traverse.Create(info.Instance);
                    foreach (var name in plugin.Fields())
                    {
                        var field = plugin.Field(name);
                        if (!typeof(ConfigEntryBase).IsAssignableFrom(field.GetValueType())) continue;
                        var entry = plugin.Field(name).GetValue<ConfigEntryBase>();
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
            container.Value = container.Value.AddItem(panel).ToArray();

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

        private static I2.Loc.TermData GetTermData(this LevelElement element)
        {
            var info = element.ElementType switch
            {
                LevelElement.Type.Brush => $"{element.Title} [{element.AllowedTileSystems}]",
                LevelElement.Type.Decor => $"{element.Title} [{element.AllowedDecorSystems}]",
                _ => throw new ArgumentOutOfRangeException(element.name)
            };
            var term = _localization.GetTermData($"Evolution/{element.name}")
                       ?? _localization.AddTerm($"Evolution/{element.name}");
            term.SetTranslation(0, info);

            return term;
        }
    }
}