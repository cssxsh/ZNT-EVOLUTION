using System.Collections;
using BepInEx.Logging;
using ZNT.Evolution.Live.BiliBili;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Live;

public class LiveManager : ComponentSingleton<LiveManager>, IActivable
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(LiveManager));

    internal BiliApi BiliApi;

    protected override void OnAwake()
    {
        base.OnAwake();
        DontDestroyOnLoad();
        BiliApi = gameObject.AddComponent<BiliApi>();
        BiliApi.enabled = false;
        // ReSharper disable UnusedParameter.Local
        BiliApi.OnError += (raw, code, message) => Logger.LogWarning($"OnError {raw}");
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
        BiliApi.OnError += (_, code, message) => StartCoroutine(OnError(code, message));
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

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator OnError(int code, string _)
    {
        yield return Wait.ForEndOfFrame;
        switch (code)
        {
            case 7001:
            case 7002:
            case 7003:
                yield return new UnityEngine.WaitForSeconds(60);
                BiliApi.SendMessage(methodName: "AppStart");
                break;
        }
    }
}