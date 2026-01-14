using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using JetBrains.Annotations;
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
    internal static readonly C5.HashedArrayList<SpawnPoint> SpawnPoints = new();

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
        foreach (var element in LevelElementIndex.Index.Values.Cast<LevelElement>())
        {
            if (element.CustomAsset is not { Prefab.name: "Human" }) continue;
            switch (element.CustomAsset)
            {
                case HumanAsset { CharacterType: CharacterType.Cultist }:
                    break;
                case HumanAsset { CharacterType: CharacterType.Boss } boss:
                    if (boss.name is "BossChemistInvincible" or "BossGertrudeCinematic") continue;
                    Assets.Add(boss);
                    break;
                case HumanAsset { Animations.name: "DroneAnimations" } drone:
                    if (drone.name is not "Drone") continue;
                    Assets.Add(drone);
                    break;
                case HumanAsset { name: "SurvivorGunner" } rick:
                    Assets.Add(rick);
                    break;
                case HumanAsset { Attitude: HumanAttitude.Combative } human:
                    if (!human.AnimationLibrary.AnimationExists("rise")) continue;
                    if (human.Invincible) continue;
                    Assets.Add(human);
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
        Users.TryGetValue(dm.OpenId, out var character);
        if (character is null) yield break;
        if (dm.IsEmoji)
        {
            switch (dm.Message)
            {
                case "禁止套娃":
                    character.SpawnCopy(id: dm.OpenId);
                    break;
            }
        }
        else
        {
            character.ShowDialogue($"{dm.UserName}: {dm.Message}", 10);
            yield return Wait.ForEndOfFrame;
            switch (dm.Message)
            {
                case "[出窍]":
                    character.OnDie();
                    break;
                case "[加油]":
                    character.OnMagazineEmpty();
                    break;
            }
        }
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private IEnumerator OnGift(Gift gift)
    {
        UserIds[gift.OpenId] = gift.UserName;
        yield return Wait.ForEndOfFrame;
    }

    private void FixedUpdate()
    {
        if (Execution.SceneMode.HasFlag(Execution.Mode.Edition)) return;
        if (SpawnPoints.IsEmpty || Assets.IsEmpty) return;
        UnityEngine.Random.InitState((int)System.DateTimeOffset.UtcNow.Ticks);
        foreach (var (id, _) in UserIds)
        {
            if (Users.ContainsKey(id)) continue;
            var asset = Assets[UnityEngine.Random.Range(0, Assets.Count)];
            var character = SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Count)].Spawn(asset);
            character.name = id;
            Users[id] = character;
        }
    }
}