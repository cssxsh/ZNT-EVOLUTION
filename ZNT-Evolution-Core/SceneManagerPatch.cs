using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public static class SceneManagerPatch
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("SceneManager");

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

        #region SettingsScene

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SettingsMenu), "OnCreate")]
        public static void SettingsScene(SettingsMenu __instance)
        {
            Logger.LogInfo("Update SettingsScene");
            __instance.AddMod();
        }

        private static void AddMod(this SettingsMenu menu)
        {
            var mod = menu.AddPanel("Mod");
            var impl = menu.transform.Find("Option Panels/Video/Scroll Area/ScrollView/Content/FullScreen Entry").gameObject;
            var content = mod.GetComponentInChildren<VerticalLayoutGroup>();

            foreach (var element in AssetElementBinder.LevelElements())
            {
                var item = UnityEngine.Object.Instantiate(impl, content.transform);
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

        private static GameObject AddPanel(this SettingsMenu menu, string name)
        {
            var panels = menu.transform.Find("Option Panels");
            var tabs = menu.transform.Find("Option Menu/Tabs");

            var panel = UnityEngine.Object.Instantiate(panels.GetChild(0).gameObject, panels);
            panel.name = name;
            var content = panel.GetComponentInChildren<VerticalLayoutGroup>();
            content.transform.DestroyChildren();
            panel.SetActive(false);

            var container = Traverse.Create(menu).Field<GameObject[]>("settingsContainer");
            var index = container.Value.Length;
            container.Value = container.Value.AddItem(panel).ToArray();

            var tab = UnityEngine.Object.Instantiate(tabs.GetChild(0).gameObject, tabs);
            tab.name = name;
            tab.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
                .ForEach(localize => localize.Term = $"Evolution/{name}_Tab");
            tab.GetComponentInChildren<Toggle>()
                .OnValueChanged(value => menu.ShowSettings(group: value ? index : -1));

            var reset = panel.transform.Find("Reset Entry");
            var reload = reset.GetComponentInChildren<Button>();
            reload.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
                .ForEach(localize => localize.Term = $"Evolution/{name}_Reset");
            reload.OnClick(() => {});

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

        private static I2.Loc.TermData GetTermData(this LevelElement element)
        {
            var info = element.ElementType switch
            {
                LevelElement.Type.Brush => $"{element.Title} [{element.AllowedTileSystems}]",
                LevelElement.Type.Decor => $"{element.Title} [{element.AllowedDecorSystems}]",
                _ => throw new ArgumentOutOfRangeException(element.name)
            };
            var term = _localization.GetTermData($"Mod/{element.name}")
                       ?? _localization.AddTerm($"Mod/{element.name}");
            term.SetTranslation(0, info);

            return term;
        }
    }
}