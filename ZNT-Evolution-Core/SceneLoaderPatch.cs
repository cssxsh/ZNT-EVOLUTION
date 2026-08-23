using System.Collections;
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
// ReSharper disable Unity.PerformanceCriticalCodeInvocation
namespace ZNT.Evolution.Core;

internal static class SceneLoaderPatch
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(SceneLoader));

    private static bool HasModChanged;

    private static IEnumerator ToCoroutine<T>(this T task, System.Action<T> @finally = null) where T : Task
    {
        while (!task.IsCompleted) yield return null;
        if (task.Exception is not null) Logger.LogError(task.Exception.GetBaseException());
        @finally?.Invoke(task);
    }

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
        if (_localization is not null) return;
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

    private static I2.Loc.TermData GetTermData(this ModContext context)
    {
        var term = _localization.AddTerm($"Evolution/{context.Metadata.Id}");
        term.SetTranslation(0, context.Title);
        return term;
    }

    #endregion

    #region MainMenuScene

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public static void MainMenuScene(MainMenu __instance)
    {
        Logger.LogInfo("Update MainMenuScene");
        var stats = (RectTransform)__instance.transform.Find("Canvas/Stats Button");
        var mod = Object.Instantiate(original: stats, parent: stats.parent);
        mod.name = "Mod Button";
        mod.localPosition = new Vector3(-77.0f, -208.0f, 0.0f);
        var image = mod.GetComponent<Image>();
        image.enabled = false;
        var tooltip = mod.GetComponent<TooltipReceiver>();
        Traverse.Create(tooltip).Field<string>("text").Value = "Evolution/Mod_Folder";
        var button = mod.GetComponent<Button>();
        button.OnClick(() => System.Diagnostics.Process.Start(ModManager.ModsPath));
    }

    #endregion

    #region SettingsScene

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SettingsMenu), "OnCreate")]
    public static void SettingsScene(SettingsMenu __instance)
    {
        Logger.LogInfo("Update SettingsScene");
        foreach (var reset in
                 from i in Enumerable.Range(1, 3)
                 let panel = __instance.transform.Find("Option Panels").GetChild(i)
                 select (RectTransform)panel.Find("Reset Entry")) reset.sizeDelta = new Vector2(-30, 80);
        __instance.AddModPanel();
        __instance.AddPluginPanel();
    }

    private static void AddModPanel(this SettingsMenu menu)
    {
        var panel = menu.AddPanel("Mod");
        var reset = panel.Find("Reset Entry/Container/ResetButton").GetComponent<Button>();
        reset.OnClick(() =>
        {
            Logger.LogInfo("Reloading Mods Folder");
            HasModChanged = true;
            reset.StartCoroutine(ModManager.ReloadAll().ToCoroutine(_ => menu.FlushModPanel()));
        });

        menu.FlushModPanel();
    }

    private static void FlushModPanel(this SettingsMenu menu)
    {
        var fullscreen = (RectTransform)menu.transform
            .Find("Option Panels/Video/Scroll Area/ScrollView/Content/FullScreen Entry");
        var content = (RectTransform)menu.transform
            .Find("Option Panels/Mod/Scroll Area/ScrollView/Content");
        content.DestroyChildren();
        foreach (var context in ModContext.Allocated())
        {
            var item = Object.Instantiate(original: fullscreen, parent: content);
            item.name = $"{context.Metadata.Name} Entry";
            item.gameObject.SetActive(false);
            var localize = item.Find("Text").GetComponent<I2.Loc.Localize>();
            localize.Term = context.GetTermData().Term;
            localize.gameObject.AddComponent<Button>().onClick.AddListener(() =>
            {
                switch (context.Metadata.Link)
                {
                    case null or "":
                        break;
                    case not null when context.Metadata.Link.StartsWith("steam://"):
                        Steamworks.SteamFriends.ActivateGameOverlayToWebPage(context.Metadata.Link);
                        break;
                    default:
                        Application.OpenURL(context.Metadata.Link);
                        break;
                }
            });
            var toggle = item.Find("Toggle").GetComponent<Toggle>();
            toggle.SetIsOnWithoutNotify(context.State is ModState.Loaded);
            toggle.OnValueChanged(value =>
            {
                switch (value)
                {
                    case true when context.IsLoadReady():
                        toggle.StartCoroutine(context.Load().ToCoroutine(_ =>
                        {
                            HasModChanged = true;
                            toggle.SetIsOnWithoutNotify(context.State is ModState.Loaded);
                        }));
                        break;
                    case false when context.IsUnloadReady():
                        toggle.StartCoroutine(context.Unload().ToCoroutine(_ =>
                        {
                            HasModChanged = true;
                            toggle.SetIsOnWithoutNotify(context.State is ModState.Loaded);
                        }));
                        break;
                    default:
                        toggle.SetIsOnWithoutNotify(context.State is ModState.Loaded);
                        break;
                }
            });
            item.gameObject.SetActive(true);
        }
    }

    private static void AddPluginPanel(this SettingsMenu menu)
    {
        var panel = menu.AddPanel("Plugin");
        var reset = (RectTransform)panel.Find("Reset Entry/Container/ResetButton");
        reset.GetComponent<Button>().OnClick(menu.ResetPluginPanel);

        var content = (RectTransform)menu.transform
            .Find("Option Panels/Plugin/Scroll Area/ScrollView/Content");
        var fullscreen = (RectTransform)menu.transform
            .Find("Option Panels/Video/Scroll Area/ScrollView/Content/FullScreen Entry");
        var fps = (RectTransform)menu.transform
            .Find("Option Panels/Video/Scroll Area/ScrollView/Content/Max FPS Entry");
        foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
        {
            if (!info.Metadata.GUID.Contains("znt")) continue;
            foreach (var (_, entry) in info.Instance.Config)
            {
                var term = _localization.AddTerm($"{info.Metadata.Name}/{entry.Definition}");
                term.SetTranslation(0, $"[{info.Metadata.Name}] {entry.Description.Description}");
                if (entry.SettingType == typeof(bool))
                {
                    var item = Object.Instantiate(original: fullscreen, parent: content);
                    item.name = $"{info.Metadata.Name} {entry.Definition} Entry";
                    item.gameObject.SetActive(false);
                    var localize = item.Find("Text").GetComponent<I2.Loc.Localize>();
                    localize.Term = term.Term;
                    var toggle = item.Find("Toggle").GetComponent<Toggle>();
                    toggle.OnValueChanged(value => entry.BoxedValue = value);
                    toggle.SetIsOnWithoutNotify((bool)entry.BoxedValue);
                    item.gameObject.SetActive(true);
                }
                else if (entry.SettingType == typeof(int))
                {
                    var item = Object.Instantiate(original: fps, parent: content);
                    item.name = $"{info.Metadata.Name} {entry.Definition} Entry";
                    item.gameObject.SetActive(false);
                    var localize = item.Find("Text").GetComponent<I2.Loc.Localize>();
                    localize.Term = term.Term;
                    var canvas = item.Find("Container").GetComponent<CanvasGroup>();
                    var toggle = item.Find("Container/Toggle").GetComponent<Toggle>();
                    var input = item.Find("Container/InputField").GetComponent<InputField>();
                    canvas.interactable = true;
                    toggle.OnValueChanged(value =>
                    {
                        entry.BoxedValue = (value ? 0 : int.MinValue) | ((int)entry.BoxedValue & int.MaxValue);
                        input.interactable = value;
                    });
                    toggle.SetIsOnWithoutNotify((int)entry.BoxedValue >= 0);
                    input.OnEndEdit(value =>
                    {
                        if (value is null or "") entry.BoxedValue = entry.DefaultValue;
                        else entry.SetSerializedValue(value);
                    });
                    input.SetTextWithoutNotify(((int)entry.BoxedValue & int.MaxValue).ToString());
                    input.interactable = toggle.isOn;
                    item.gameObject.SetActive(true);
                }
            }
        }
    }

    private static void ResetPluginPanel(this SettingsMenu menu)
    {
        var content = (RectTransform)menu.transform
            .Find("Option Panels/Plugin/Scroll Area/ScrollView/Content");
        foreach (var (_, info) in BepInEx.Bootstrap.Chainloader.PluginInfos)
        {
            if (!info.Metadata.GUID.Contains("znt")) continue;
            foreach (var (definition, entry) in info.Instance.Config)
            {
                entry.BoxedValue = entry.DefaultValue;
                var item = content.Find($"{info.Metadata.Name} {definition} Entry");
                switch (entry.BoxedValue)
                {
                    case bool value:
                    {
                        var toggle = item.Find("Toggle").GetComponent<Toggle>();
                        toggle.SetIsOnWithoutNotify(value);
                    }
                        break;
                    case int value:
                    {
                        var toggle = item.Find("Container/Toggle").GetComponent<Toggle>();
                        toggle.SetIsOnWithoutNotify(value >= 0);
                        var input = item.Find("Container/InputField").GetComponent<InputField>();
                        input.SetTextWithoutNotify((value & int.MaxValue).ToString());
                        input.interactable = toggle.isOn;
                    }
                        break;
                }
            }
        }
    }

    private static RectTransform AddPanel(this SettingsMenu menu, string name)
    {
        var panels = (RectTransform)menu.transform.Find("Option Panels");
        var panel = (RectTransform)Object.Instantiate(original: panels.GetChild(0), parent: panels);
        panel.name = name;
        panel.gameObject.SetActive(false);
        panel.Find("Scroll Area/ScrollView/Content").DestroyChildren();

        var container = Traverse.Create(menu).Field<GameObject[]>("settingsContainer");
        var index = container.Value.Length;
        container.Value = container.Value.AddToArray(panel.gameObject);

        var tabs = (RectTransform)menu.transform.Find("Option Menu/Tabs");
        var tab = (RectTransform)Object.Instantiate(original: tabs.GetChild(0), parent: tabs);
        tab.name = name;
        tab.GetComponent<Toggle>().OnValueChanged(value => menu.ShowSettings(value ? index : -1));
        tab.Find("Label").GetComponent<I2.Loc.Localize>().Term = $"Evolution/{name}_Tab";

        var reset = (RectTransform)panel.Find("Reset Entry/Container/ResetButton");
        reset.GetComponent<Button>().OnClick(() => Logger.LogWarning($"{name} reset no define"));
        reset.Find("Text Hilight").GetComponent<I2.Loc.Localize>().Term = $"Evolution/{name}_Reset";
        reset.Find("Text Pressed").GetComponent<I2.Loc.Localize>().Term = $"Evolution/{name}_Reset";
        reset.Find("Text Default").GetComponent<I2.Loc.Localize>().Term = $"Evolution/{name}_Reset";

        return panel;
    }

    #endregion

    #region EditorStartScene

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PublishChapterMenu), "SelectDestination")]
    public static void SelectDestination(PublishChapterMenu __instance, int value)
    {
        Traverse.Create(__instance)
            .Field<CanvasGroup>("chapterGroup").Value
            .transform.Find("Visible").gameObject.SetActive(value is 0);
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
        {
            var prefabs = Traverse.Create(menu).Field<SupportedTypePrefabs>("typePrefabs").Value;
            var binder = prefabs[EditorComponent.SupportedType.Vector4];
            var fields = binder.GetComponentsInChildren<InputField>();
            // ReSharper disable once CoVariantArrayConversion
            Traverse.Create(binder).Field<UIBehaviour[]>("uiComponents").Value = fields;
        }
        {
            var prefabs = Traverse.Create(menu).Field<SupportedTypePrefabs>("typePrefabs").Value;
            var binder = prefabs[EditorComponent.SupportedType.LocalizableString];
            var components = Traverse.Create(binder).Field<UIBehaviour[]>("uiComponents").Value;
            var localizable = (LocalizableStringMenu)components[0];
            var placeholder = (Text)Traverse.Create(localizable).Field<InputField>("contentField").Value.placeholder;
            placeholder.text = "Enter text...";
        }
        {
            var prefabs = Traverse.Create(menu).Field<SupportedTypePrefabs>("typePrefabs").Value;
            var binder = prefabs[EditorComponent.SupportedType.TutorialPageList];
            var components = Traverse.Create(binder).Field<UIBehaviour[]>("uiComponents").Value;
            var tutorial = (TutorialPageMenu)components[0];
            var title = Traverse.Create(tutorial).Field<LocalizableStringMenu>("titleMenu").Value;
            var title_placeholder = (Text)Traverse.Create(title).Field<InputField>("contentField").Value.placeholder;
            title_placeholder.text = "Enter title...";
            var text = Traverse.Create(tutorial).Field<LocalizableStringMenu>("textMenu").Value;
            var text_placeholder = (Text)Traverse.Create(text).Field<InputField>("contentField").Value.placeholder;
            text_placeholder.text = "Enter text...";
        }
    }

    private static void CopyObject(this SelectionMenu menu, bool active)
    {
        var target = Traverse.Create(menu).Field<EditorGameObject>("serializeGameObject").Value;
        target?.ObjectSettings.Activate(active, active ? ObjectSettings.Control.Copy : ObjectSettings.Control.None);
    }

    private static void OnObjectMoved(this SelectionMenu menu, GameObject go, bool isBrush)
    {
        var move = Traverse.Create(menu).Field<Toggle>("moveButton").Value;
        var copy = move.transform.parent.Find("Copy Button").GetComponent<Toggle>();
        copy.isOn = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SelectionMenu), "SelectObject")]
    public static void SelectObject(SelectionMenu __instance)
    {
        var target = Traverse.Create(__instance).Field<EditorGameObject>("serializeGameObject").Value;
        target?.ObjectSettings.OnCopy -= __instance.OnObjectMoved;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SelectionMenu), "UpdateCommonMenu")]
    public static void UpdateCommonMenu(SelectionMenu __instance)
    {
        var target = Traverse.Create(__instance).Field<EditorGameObject>("serializeGameObject").Value;
        target?.ObjectSettings.OnCopy += __instance.OnObjectMoved;
        var move = Traverse.Create(__instance).Field<Toggle>("moveButton").Value;
        var copy = move.transform.parent.Find("Copy Button").GetComponent<Toggle>();
        copy.isOn = false;
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

        foreach (var transform in container.Cast<RectTransform>())
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
                    if (overrider is not null && overrider.OverrideMemberUi(__instance, component, member)) continue;
                    __instance.SetDefaultUi(component, member);
                }
            }
            finally
            {
                Traverse.Create(__instance).Field<RectTransform>("mainContainer").Value = container;
                header.AddComponent<Button>().onClick.AddListener(panel.ToggleActivation);
                header.SetActive(panel.childCount is not 0);
                panel.gameObject.SetActive(panel.childCount is not 0 && Activated.Contains(panel.name));
            }
        }

        scroll.Rebuild(CanvasUpdate.PostLayout);
        foreach (var updater in updaters) updater.OnEditorOpen();

        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EditorComponent), "FromComponent")]
    [HarmonyPatch(typeof(SerializableComponent), "FromComponent")]
    public static void FromComponent(BaseComponent component, object __result)
    {
        switch (__result, component)
        {
            case (EditorComponent editor, Patroller patroller):
            {
                var editing = typeof(Patroller).GetTypeInfo().GetDeclaredField("editing");
                editor.Fields.Remove(editing);
                editor.Fields[typeof(Patroller).GetField(nameof(Patroller.Voice))] = patroller.Voice;
                editor.Fields[editing] = editing.GetValue(patroller);
            }
                break;
            case (SerializableComponent serializable, Patroller patroller):
            {
                serializable.Fields[nameof(Patroller.Voice)] = patroller.Voice;
            }
                break;
            case (EditorComponent editor, HumanBehaviour human):
            {
                editor.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.ResistScream))] = human.ResistScream;
                editor.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.AllowMultipleAttackers))] = human.AllowMultipleAttackers;
                editor.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.GrabbedOnAttacked))] = human.GrabbedOnAttacked;
                editor.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.IgnoreDamages))] = human.IgnoreDamages;
                editor.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.InvincibleOnAttack))] = human.InvincibleOnAttack;
                editor.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.FleeBeforeZombieExplode))] = human.FleeBeforeZombieExplode;
                editor.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.MoveTowardStaticTargets))] = human.MoveTowardStaticTargets;
                editor.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.VisionFollowTarget))] = human.VisionFollowTarget;
                editor.Fields[typeof(HumanBehaviour)
                    .GetField(nameof(HumanBehaviour.Attitude))] = human.Attitude;
            }
                break;
            case (SerializableComponent serializable, HumanBehaviour human):
            {
                serializable.Fields[nameof(HumanBehaviour.ResistScream)] = human.ResistScream;
                serializable.Fields[nameof(HumanBehaviour.AllowMultipleAttackers)] = human.AllowMultipleAttackers;
                serializable.Fields[nameof(HumanBehaviour.GrabbedOnAttacked)] = human.GrabbedOnAttacked;
                serializable.Fields[nameof(HumanBehaviour.IgnoreDamages)] = human.IgnoreDamages;
                serializable.Fields[nameof(HumanBehaviour.InvincibleOnAttack)] = human.InvincibleOnAttack;
                serializable.Fields[nameof(HumanBehaviour.FleeBeforeZombieExplode)] = human.FleeBeforeZombieExplode;
                serializable.Fields[nameof(HumanBehaviour.MoveTowardStaticTargets)] = human.MoveTowardStaticTargets;
                serializable.Fields[nameof(HumanBehaviour.VisionFollowTarget)] = human.VisionFollowTarget;
                serializable.Fields[nameof(HumanBehaviour.Attitude)] = human.Attitude;
            }
                break;
            case (EditorComponent editor, TrapEffect trap):
            {
                editor.Fields.Clear();
                editor.Fields[typeof(TrapEffect).GetField(nameof(TrapEffect.Mode))] = trap.Mode;
                editor.Fields[typeof(TrapEffect).GetField(nameof(TrapEffect.KillDelay))] = trap.KillDelay;
                editor.Fields[typeof(TrapEffect).GetField(nameof(TrapEffect.Damage))] = trap.Damage;
                editor.Fields[typeof(TrapEffect).GetField(nameof(TrapEffect.DamageRate))] = trap.DamageRate;
                editor.Fields[typeof(TrapEffect).GetField(nameof(TrapEffect.DamageType))] = trap.DamageType;
            }
                break;
            case (SerializableComponent serializable, TrapEffect trap):
            {
                serializable.Fields[nameof(TrapEffect.Mode)] = trap.Mode;
                serializable.Fields[nameof(TrapEffect.KillDelay)] = trap.KillDelay;
                serializable.Fields[nameof(TrapEffect.Damage)] = trap.Damage;
                serializable.Fields[nameof(TrapEffect.DamageRate)] = trap.DamageRate;
                serializable.Fields[nameof(TrapEffect.DamageType)] = trap.DamageType;
            }
                break;
        }
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

        // ReSharper disable once InvertIf
        if (member.GetMemberType() == typeof(Color))
        {
            // HexColorField
            __instance.TextBinder().BindString(component, member);
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SupportedTypeBinder), "SetName")]
    public static bool SetName(this SupportedTypeBinder __instance, MemberInfo member)
    {
        var attribute = member.GetCustomAttribute<SerializeInEditorAttribute>();
        var name = attribute?.Name is null or "" ? member.Name.SplitCamelCase() : attribute.Name;
        var text = Traverse.Create(__instance).Field<Text>("text").Value;
        text.text = name;
        text.transform.parent.name = $"{name} Input";
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SupportedTypeBinder), "BindString")]
    public static bool BindString(SupportedTypeBinder __instance, EditorComponent component, MemberInfo member)
    {
        if (member.GetMemberType() != typeof(Color)) return true;
        __instance.SetName(member);
        var components = Traverse.Create(__instance).Field<UIBehaviour[]>("uiComponents").Value;
        var input = (InputField)components[0];
        var normal = input.colors.normalColor;
        input.onEndEdit.RemoveAllListeners();
        input.onEndEdit.AddListener(text =>
        {
            if (ColorUtility.TryParseHtmlString(text, out var color))
            {
                input.colors = input.colors with { normalColor = normal };
                input.text = "#" + ColorUtility.ToHtmlStringRGBA(color);
                member.SetMemberValue(component.Data, color);
            }
            else
            {
                input.colors = input.colors with { normalColor = Color.red };
            }
        });
        var color = member.GetMemberValue<Color>(component.Data);
        input.text = "#" + ColorUtility.ToHtmlStringRGBA(color);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SupportedTypeBinder), "BindLocalizableString")]
    public static void BindLocalizableString(
        SupportedTypeBinder __instance, EditorComponent component, MemberInfo member)
    {
        var components = Traverse.Create(__instance).Field<UIBehaviour[]>("uiComponents").Value;
        var value = member.GetMemberValue<LocalizableString>(component.Data);
        var localizable = (LocalizableStringMenu)components[0];
        if (member.DeclaringType == typeof(TutorialSettings)) value.Category ??= "Tutorials";
        if (value.Category is null or "")
        {
            Traverse.Create(localizable).Field<CanvasGroup>("toggleGroup").Value.interactable = false;
        }
        else
        {
            Traverse.Create(localizable).Field<bool>("useStringCategory").Value = false;
            Traverse.Create(localizable).Field<string>("category").Value = value.Category;
        }
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
            input.Find("Container/Z Text").gameObject.SetActive(false);
            input.Find("Container/Z Input").gameObject.SetActive(false);
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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SignalReceiverLinker), "Start")]
    public static void Start(SignalReceiverLinker __instance)
    {
        switch (__instance.GetComponentInParent<BaseBehaviour>())
        {
            case MovingObjectBehaviour moving:
                __instance.AddReceiver(new ReceiverLink
                {
                    GameObject = moving.gameObject,
                    Component = moving,
                    Name = nameof(MovingObjectBehaviour.OnHitCharacter),
                    Title = "Hit"
                });
                break;
            case SentryGunBehaviour sentry:
                __instance.AddReceiver(new ReceiverLink
                {
                    GameObject = sentry.gameObject,
                    Component = sentry,
                    Name = nameof(SentryGunBehaviour.OnDamage),
                    Title = "Hit"
                });
                __instance.AddReceiver(new ReceiverLink
                {
                    GameObject = __instance.gameObject,
                    Component = sentry,
                    Name = nameof(SentryGunBehaviour.OnDie),
                    Title = "Break"
                });
                break;
            case CharacterBehaviour { Character: { } character }:
                __instance.AddReceiver(new ReceiverLink
                {
                    GameObject = __instance.gameObject,
                    Component = character,
                    Name = nameof(Character.OnDamage),
                    Title = "Hit"
                });
                __instance.AddReceiver(new ReceiverLink
                {
                    GameObject = __instance.gameObject,
                    Component = character,
                    Name = nameof(Character.OnDie),
                    Title = "Kill"
                });
                break;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SignalSenderLinker), "Start")]
    public static void Start(SignalSenderLinker __instance)
    {
        // ReSharper disable once InvertIf
        // ReSharper disable once RedundantBoolCompare
        if (__instance.GetComponentInChildren<Health>() is { } health &&
            __instance.ExcludedGameObjects.Contains(health.gameObject) is false)
        {
            __instance.AddSender(new SenderLink
            {
                GameObject = health.gameObject,
                Component = health,
                SignalSender = health.OnDamage,
                Name = nameof(health.OnDamage),
                Title = nameof(health.OnDamage)
            });
            __instance.AddSender(new SenderLink
            {
                GameObject = health.gameObject,
                Component = health,
                SignalSender = health.OnDie,
                Name = nameof(health.OnDie),
                Title = nameof(health.OnDie)
            });
        }
    }

    private static void AddReceiver(this SignalReceiverLinker linker, ReceiverLink link)
    {
        Traverse.Create(linker).Method("AddReceiver", link.Component, link).GetValue();
    }

    private static void AddSender(this SignalSenderLinker linker, SenderLink link)
    {
        Traverse.Create(linker).Method("AddSender", link.Component, link).GetValue();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PaintMenu), "FilterOnFirstLoad")]
    public static void FilterOnFirstLoad(PaintMenu __instance)
    {
        if (!__instance.gameObject.activeInHierarchy || !HasModChanged) return;
        __instance.StopAllCoroutines();
        Traverse.Create(__instance)
            .Field<Dictionary<int, List<ListViewIconsItemDescription>>>("elements").Value.Clear();
        Traverse.Create(__instance)
            .Method("FillAccordion").GetValue();
        var input = __instance.GetComponentInChildren<InputField>();
        __instance.FilterList(input.text);
        HasModChanged = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LevelSettingsMenu), "InitGeneralSettings")]
    public static void InitGeneralSettings(LevelSettingsMenu __instance)
    {
        Traverse.Create(__instance).Field<Spinner>("maxZombieSpinner").Value.Max = short.MaxValue;
        Traverse.Create(__instance).Field<Spinner>("maxEnemySpinner").Value.Max = short.MaxValue;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(LevelSettingsMenu), "InitCameraSettings")]
    public static IEnumerable<CodeInstruction> InitCameraSettings(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.OperandIs(5f))
            {
                yield return instruction.Clone(1f);
            }
            else if (instruction.OperandIs(50f))
            {
                yield return instruction.Clone(1000f);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    #endregion
}