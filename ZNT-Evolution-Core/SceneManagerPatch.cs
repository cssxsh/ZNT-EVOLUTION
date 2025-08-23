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

namespace ZNT.Evolution.Core
{
    public static class SceneManagerPatch
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("SceneManager");

        private static I2.Loc.LanguageSourceData _localization;

        [HarmonyPatch(typeof(I2.Loc.LocalizationManager), "UpdateSources"), HarmonyPostfix]
        public static void UpdateSources()
        {
            if (I2.Loc.LocalizationManager.Sources
                .SelectMany(loaded => loaded.GetCategories())
                .Any(category => category == "Mod"))
            {
                return;
            }

            _localization ??= new I2.Loc.LanguageSourceData();
            try
            {
                _localization.Import_CSV(Category: "Mod", CSVstring: Resource("MOD.csv"));
            }
            catch (FileNotFoundException e)
            {
                Logger.LogError(e);
            }

            I2.Loc.LocalizationManager.Sources.Add(_localization);
            Logger.LogInfo("Mod LanguageSource Loaded.");
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

        // ReSharper disable Once InconsistentNaming
        [HarmonyPatch(typeof(SettingsMenu), "OnCreate"), HarmonyPostfix]
        public static void SettingsScene(SettingsMenu __instance)
        {
            Logger.LogInfo("Update SettingsScene");
            var menu = __instance;
            var tabs = menu.gameObject.GetChildren()
                .Find(body => body.name == "Option Menu")
                .GetChildren()
                .Find(body => body.name == "Tabs");
            var panels = menu.gameObject.GetChildren()
                .Find(body => body.name == "Option Panels");

            var container = Traverse.Create(menu).Field<GameObject[]>("settingsContainer");
            var panel = UnityEngine.Object.Instantiate(container.Value[1], panels.transform);
            panel.name = "Mod";
            panel.SetActive(false);
            container.Value = container.Value.AddItem(panel).ToArray();

            var content = panel.GetComponentInChildren<VerticalLayoutGroup>();
            content.gameObject.GetChildren()
                .ForEach(body => body.SetActive(false));
            var impl = content.gameObject.GetChildren()
                .Find(body => body.name == "FullScreen Entry");

            foreach (var element in AssetElementBinder.LevelElements())
            {
                var item = UnityEngine.Object.Instantiate(impl, content.transform);
                item.name = $"{element.Title} Entry";
                item.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
                    .ForEach(localize => localize.Term = element.GetTermData().Term);
                item.SetActive(true);
                var toggle = item.GetComponentInChildren<Toggle>(includeInactive: true);
                var enable = Traverse.Create(element).Field("useable");
                toggle.OnValueChanged(value => enable.SetValue(value));
                toggle.SetIsOnWithoutNotify(element.Useable);
            }

            var reset = panel.GetChildren()
                .Find(body => body.name == "Reset Entry");
            var reload = reset.GetComponentInChildren<Button>();
            reload.name = "ReloadButton";
            reload.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
                .ForEach(localize => localize.Term = "Mod/Reload");
            reload.OnClick(() =>
            {
                Logger.LogInfo("Reload MOD ...");
                // TODO: ...
            });

            var tab = UnityEngine.Object.Instantiate(tabs.GetChildren()[0], tabs.transform);
            tab.name = "Mod";
            tab.GetComponentInChildren<I2.Loc.Localize>()
                .Term = "Mod/Tab_Mod";
            tab.GetComponentInChildren<Toggle>()
                .OnValueChanged(value => menu.ShowSettings(group: value ? container.Value.Length - 1 : -1));
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
            var info = element.ElementType == LevelElement.Type.Brush
                ? $"{element.Title} [{element.AllowedTileSystems}]"
                : $"{element.Title} [{element.AllowedDecorSystems}]";
            var term = _localization.GetTermData($"Mod/{element.name}")
                       ?? _localization.AddTerm($"Mod/{element.name}");
            term.SetTranslation(0, info);

            return term;
        }
    }
}