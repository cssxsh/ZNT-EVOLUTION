using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine.Networking;
using ZNT.Evolution.Live.BiliBili;
using ZNT.Evolution.Live.BiliBili.Data;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Live;

public class LiveManager : ComponentSingleton<LiveManager>, IActivable, I2.Loc.ILanguageSource
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(LiveManager));

    [UsedImplicitly]
    internal BiliApi BiliApi;

    [UsedImplicitly]
    internal I2.Loc.TermData LiveState => SourceData.AddTerm("Live/State");

    [UsedImplicitly]
    internal static readonly Dictionary<string, string> UserIds = new();

    [UsedImplicitly]
    internal static readonly Dictionary<string, Character> Users = new();

    [UsedImplicitly]
    internal static readonly Dictionary<string, SpawnPoint> SpawnPoints = new();

    [UsedImplicitly]
    internal static readonly C5.HashedArrayList<CharacterAsset> Assets = new();

    public I2.Loc.LanguageSourceData SourceData { get; set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        DontDestroyOnLoad();
        SourceData = new I2.Loc.LanguageSourceData
        {
            GoogleUpdateFrequency = I2.Loc.LanguageSourceData.eGoogleUpdateFrequency.Never,
            GoogleInEditorCheckFrequency = I2.Loc.LanguageSourceData.eGoogleUpdateFrequency.Never
        };
        {
            using var fs = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ZNT.Evolution.Live.Resources.Live.csv");
            using var reader = new StreamReader(fs ?? throw new FileNotFoundException("Live.csv"));
            SourceData.Import_CSV(Category: "Live", CSVstring: reader.ReadToEnd());
        }
        SourceData.Awake();
        SceneLoader.BeforeLoadScene += Users.Clear;
        BiliApi = gameObject.AddComponent<BiliApi>();
        BiliApi.enabled = false;
        // ReSharper disable UnusedParameter.Local
        BiliApi.OnError += (raw, request) => Logger.LogWarning($"OnError {raw}");
        BiliApi.OnStart += (raw, anchor) => Logger.LogDebug($"OnStart {raw}");
        BiliApi.OnEnd += (raw, anchor) => Logger.LogDebug($"OnEnd {raw}");
        BiliApi.OnWsLink += (ws, link) => Logger.LogDebug($"OnWsLink {link}");
        BiliApi.OnWsAuth += (ws, auth) => Logger.LogDebug($"OnWsAuth {auth}");
        BiliApi.OnWsError += (ws, exception) => Logger.LogWarning($"OnWsError {exception}");
        BiliApi.OnEnter += (raw, enter) => Logger.LogDebug($"OnEnter {raw}");
        BiliApi.OnDanmaku += (raw, dm) => Logger.LogDebug($"OnDanmaku {raw}");
        BiliApi.OnGift += (raw, gift) => Logger.LogDebug($"OnGift {raw}");
        BiliApi.OnSuperChat += (raw, sc) => Logger.LogInfo($"OnSuperChat {raw}");
        BiliApi.OnSuperChatDelete += (raw, del) => Logger.LogInfo($"OnSuperChatDelete {raw}");
        BiliApi.OnGuard += (raw, guard) => Logger.LogInfo($"OnGuard {raw}");
        // ReSharper restore UnusedParameter.Local
        BiliApi.OnError += (_, request) => StartCoroutine(nameof(OnError), request);
        BiliApi.OnStart += (_, anchor) => StartCoroutine(nameof(OnStart), anchor);
        BiliApi.OnEnd += (_, anchor) => StartCoroutine(nameof(OnEnd), anchor);
        BiliApi.OnEnter += (_, enter) => StartCoroutine(nameof(OnEnter), enter);
        BiliApi.OnDanmaku += (_, dm) => StartCoroutine(nameof(OnDanmaku), dm);
        BiliApi.OnGift += (_, gift) => StartCoroutine(nameof(OnGift), gift);
        StartCoroutine(nameof(LoadYellowFace));
        foreach (var element in LevelElementIndex.Index.Values.Cast<LevelElement>())
        {
            if (element is not { Useable: true, DevOnly: false }) continue;
            if (element.CustomAsset is not HumanAsset { Prefab.name: "Human", Invincible: false } human) continue;
            if (!human.AnimationLibrary.AnimationExists("rise")) continue;
            switch (human)
            {
                case { Animations.name: "ArmedAnimations" }:
                    // if (human.name is "BossChemist" or "BossDrugLord" or "Astrogoliath" or "Clown") continue;
                    if (human.name is "BossChemist" or "BossGertrudeCinematic") continue;
                    Assets.Add(human);
                    break;
                case { Animations.name: "BulkyMeleeAnimations" }:
                    // if (human.name is "MachineGunner") continue;
                    Assets.Add(human);
                    break;
                case { Animations.name: "DroneAnimations" }:
                    if (human.name is "DroneInvincible" or "DroneInvisible") continue;
                    Assets.Add(human);
                    break;
                case { Animations.name: "MeleeAnimations" }:
                    if (human.name is "Priest" or "Virgin") continue;
                    // if (human.name is "Lumberjack") continue;
                    Assets.Add(human);
                    break;
                case { Animations.name: "UnarmedAnimations" }:
                    // if (human.name is "DaftPunk1" or "DaftPunk2") continue;
                    // Assets.Add(human);
                    break;
            }
        }
    }

    public bool IsActive => BiliApi.enabled;

    public void SetActive(bool state)
    {
        if (state)
        {
            if (BiliApi.enabled) return;
            BiliApi.AccessKeyId = EvolutionLivePlugin.AccessKeyId.Value;
            BiliApi.AccessKeySecret = EvolutionLivePlugin.AccessKeySecret.Value;
            BiliApi.AppId = EvolutionLivePlugin.AppId.Value;
            BiliApi.Code = EvolutionLivePlugin.Code.Value;
            BiliApi.enabled = true;
        }
        else
        {
            BiliApi.enabled = false;
        }
    }

    public void SetActive() => SetActive(true);

    public void SetInactive() => SetActive(false);

    public void ToggleActivation() => SetActive(!IsActive);

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private IEnumerator OnStart(AnchorInfo anchor)
    {
        LiveState.SetTranslation(0, $"Linked {anchor.RoomId}");
        LiveState.SetTranslation(9, $"已连接 {anchor.RoomId}");
        yield return Wait.ForEndOfFrame;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private IEnumerator OnEnd(AnchorInfo _)
    {
        LiveState.SetTranslation(0, "NoLink");
        LiveState.SetTranslation(9, "未连接");
        yield return Wait.ForEndOfFrame;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator OnError(RequestInfo request)
    {
        LiveState.SetTranslation(0, $"Error {request.Message}");
        LiveState.SetTranslation(9, $"错误  {request.Message}");
        yield return Wait.ForEndOfFrame;
        switch (request.Code)
        {
            case 7001:
            case 7002:
            case 7003:
                yield return new UnityEngine.WaitForSeconds(60);
                BiliApi.StartCoroutine(methodName: "AppStart");
                break;
        }
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private IEnumerator OnEnter(Enter enter)
    {
        UserIds[enter.OpenId] = enter.UserName;
        yield return Wait.ForEndOfFrame;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private IEnumerator OnDanmaku(Danmaku dm)
    {
        UserIds[dm.OpenId] = dm.UserName;
        yield return Wait.ForEndOfFrame;
        if (Users.TryGetValue(dm.OpenId, out var character))
        {
            character.ShowMessage(dm.Message, 10);
            yield return Wait.ForEndOfFrame;
        }
        else if (Regex.IsMatch(dm.Message, @"^(\d+)$"))
        {
            var point = SpawnPoints[dm.Message];
            var asset = Assets[UnityEngine.Random.Range(0, Assets.Count)];
            Users[dm.OpenId] = point.Spawn(asset);
            Users[dm.OpenId].name = dm.OpenId;
        }
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private IEnumerator OnGift(Gift gift)
    {
        UserIds[gift.OpenId] = gift.UserName;
        yield return Wait.ForEndOfFrame;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private IEnumerator LoadYellowFace()
    {
        var icons = UnityEngine.Resources.FindObjectsOfTypeAll<tk2dSpriteAnimation>()
            .First(animation => animation.name == "anim_human_icons");
        foreach (var (text, url) in BiliApi.YellowFace())
        {
            var id = icons.GetClipIdByName(text);
            if (id != -1) Logger.LogWarning($"{icons.name} already exists clip {text} at {id}");
            var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            var texture = DownloadHandlerTexture.GetContent(request);
            var sprite = tk2dSpriteCollectionData.CreateFromTexture(
                texture: texture,
                size: tk2dSpriteCollectionSize.Explicit(0.5f, texture.height),
                names: new[] { url },
                regions: new[] { new UnityEngine.Rect(0, 0, texture.width, texture.height) },
                anchors: new[] { new UnityEngine.Vector2(texture.width, texture.height) / 2 }
            );
            UnityEngine.Object.DontDestroyOnLoad(sprite);
            var clip = new tk2dSpriteAnimationClip
            {
                name = text,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Once,
                frames = new[]
                {
                    new tk2dSpriteAnimationFrame
                    {
                        spriteCollection = sprite,
                        triggerEvent = false,
                        eventInfo = "hide_icon"
                    }
                }
            };
            icons.clips = icons.clips.AddToArray(clip);
            Traverse.Create(icons)
                .Field<Dictionary<string, tk2dSpriteAnimationClip>>("clipNameCache").Value = null;
            Traverse.Create(icons)
                .Field<Dictionary<string, int>>("idNameCache").Value = null;
            icons.InitializeClipCache();
        }
    }
}