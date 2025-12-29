using System.Collections;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using ZNT.Evolution.Live.BiliBili;
using ZNT.Evolution.Live.BiliBili.Data;

// ReSharper disable once InconsistentNaming
namespace ZNT.Evolution.Live;

public class LiveManager : ComponentSingleton<LiveManager>, IActivable
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(LiveManager));

    private BiliApi BiliApi;

    protected override void OnAwake()
    {
        base.OnAwake();
        DontDestroyOnLoad();
        BiliApi = gameObject.AddComponent<BiliApi>();
        BiliApi.enabled = false;
        // log
        BiliApi.OnError += (raw, code, message) => Logger.LogWarning($"OnError {raw}");
        BiliApi.OnStart += (raw, anchor) => Logger.LogDebug($"OnStart {raw}");
        BiliApi.OnEnd += (raw, anchor) => Logger.LogDebug($"OnEnd {raw}");
        BiliApi.OnWsLink += (ws, link) => Logger.LogDebug($"OnWsLink {link}");
        BiliApi.OnWsAuth += (ws, auth) => Logger.LogDebug($"OnWsAuth {auth}");
        BiliApi.OnWsError += (ws, exception) => Logger.LogWarning($"OnWsError {exception}");
        BiliApi.OnDanmaku += (raw, dm) => Logger.LogDebug($"OnDanmaku {raw}");
        BiliApi.OnGift += (raw, gift) => Logger.LogInfo($"OnGift {raw}");
        BiliApi.OnSuperChat += (raw, sc) => Logger.LogInfo($"OnSuperChat {raw}");
        BiliApi.OnSuperChatDelete += (raw, del) => Logger.LogInfo($"OnSuperChatDelete {raw}");
        BiliApi.OnGuard += (raw, guard) => Logger.LogInfo($"OnGuard {raw}");
        // handle
        BiliApi.OnDanmaku += (_, dm) => StartCoroutine(OnDanmaku(dm));
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

    private static IEnumerator OnDanmaku(Danmaku dm)
    {
        foreach (var element in LevelElementIndex.Index.Values.Cast<LevelElement>())
        {
            if (element.Title != dm.Message && element.name != dm.Message) continue;
            switch (element.CustomAsset)
            {
                case HumanAsset human:
                    yield return Wait.ForEndOfFrame;
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    foreach (var point in FindObjectsOfType<CharacterSpawnPoint>())
                    {
                        if (point.Active) continue;
                        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                        human.CreateGameObject(position: point.transform.position)
                            .GetComponent<ISpawnable>()
                            .OnSpawn(Traverse.Create(point).Field<Parameters>("sendParams").Value);
                    }

                    break;
            }
        }
    }
}