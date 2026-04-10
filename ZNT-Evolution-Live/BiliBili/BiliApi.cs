using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using ZNT.Evolution.Live.BiliBili.Data;

namespace ZNT.Evolution.Live.BiliBili;

public class BiliApi : MonoBehaviour
{
    // ReSharper disable InconsistentNaming
    public string AccessKeyId;
    public string AccessKeySecret;
    public long AppId;
    public string Code;
    private GameInfo GameInfo;
    private WebsocketInfo WebsocketInfo;
    private AnchorInfo AnchorInfo;
    private long LastHeartBeat;
    private readonly ClientWebSocket WebSocketImpl = new();
    // ReSharper restore InconsistentNaming

    private static readonly ConcurrentBag<ArraySegment<byte>> BufferPooling = new();
    private ArraySegment<byte> AuthPacket => Packet(WsOperation.OP_AUTH, JObject.Parse(WebsocketInfo.AuthBody));
    private static readonly ArraySegment<byte> HeartBeatPacket = Packet(WsOperation.OP_HEARTBEAT, null);

    private static ArraySegment<byte> Buffer
    {
        get => BufferPooling.TryTake(out var buffer) ? buffer : WebSocket.CreateServerBuffer(0x00002000);
        set => BufferPooling.Add(value);
    }

    // ReSharper disable InconsistentNaming
    public event Action<JObject, RequestInfo> OnError;
    public event Action<JObject, AnchorInfo> OnStart;
    public event Action<JObject, AnchorInfo> OnEnd;
    public event Action<JObject, AnchorInfo, long> OnHeartBeat;
    public event Action<WebsocketInfo, string> OnWsLink;
    public event Action<WebsocketInfo, JObject> OnWsAuth;
    public event Action<WebsocketInfo> OnWsHeartBeat;
    public event Action<WebsocketInfo, Exception> OnWsError;
    public event Action<JObject, Enter> OnEnter;
    public event Action<JObject, Danmaku> OnDanmaku;
    public event Action<JObject, Gift> OnGift;
    public event Action<JObject, SuperChat> OnSuperChat;
    public event Action<JObject, SuperChatDelete> OnSuperChatDelete;
    public event Action<JObject, Guard> OnGuard;
    // ReSharper restore InconsistentNaming

    private void OnEnable() => StartCoroutine(nameof(AppStart));

    private void OnDisable() => StartCoroutine(nameof(AppEnd));

    private void FixedUpdate() => StartCoroutine(nameof(AppHeartBeat));

    [UsedImplicitly]
    protected IEnumerator AppStart()
    {
        if (string.IsNullOrEmpty(Code)) yield break;
        var post = Post("https://live-open.biliapi.com/v2/app/start", new JObject
        {
            ["app_id"] = AppId,
            ["code"] = Code
        });
        yield return post.SendWebRequest();
        var result = JObject.Parse(post.downloadHandler.text);
        if (result.Value<int>("code") != 0)
        {
            OnError?.Invoke(result, result.ToObject<RequestInfo>());
        }
        else
        {
            GameInfo = result["data"]["game_info"].ToObject<GameInfo>();
            WebsocketInfo = result["data"]["websocket_info"].ToObject<WebsocketInfo>();
            AnchorInfo = result["data"]["anchor_info"].ToObject<AnchorInfo>();
            StartCoroutine(nameof(WsLink));
            OnStart?.Invoke(result, AnchorInfo);
        }
    }

    [UsedImplicitly]
    protected IEnumerator AppEnd()
    {
        if (string.IsNullOrEmpty(GameInfo.GameId)) yield break;
        var post = Post("https://live-open.biliapi.com/v2/app/end", new JObject
        {
            ["app_id"] = AppId,
            ["game_id"] = GameInfo.GameId
        });
        yield return post.SendWebRequest();
        var result = JObject.Parse(post.downloadHandler.text);
        if (result.Value<int>("code") != 0)
        {
            OnError?.Invoke(result, result.ToObject<RequestInfo>());
        }
        else
        {
            OnEnd?.Invoke(result, AnchorInfo);
            GameInfo = default;
            WebsocketInfo = default;
            AnchorInfo = default;
        }
    }

    [UsedImplicitly]
    protected IEnumerator AppHeartBeat()
    {
        if (string.IsNullOrEmpty(GameInfo.GameId)) yield break;
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - LastHeartBeat < 20) yield break;
        LastHeartBeat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var post = Post("https://live-open.biliapi.com/v2/app/heartbeat", new JObject
        {
            ["game_id"] = GameInfo.GameId
        });
        yield return post.SendWebRequest();
        var result = JObject.Parse(post.downloadHandler.text);
        if (result.Value<int>("code") != 0)
        {
            OnError?.Invoke(result, result.ToObject<RequestInfo>());
        }
        else
        {
            yield return WsHeartBeat();
            LastHeartBeat = long.Parse(post.GetRequestHeader("x-bili-timestamp"));
            OnHeartBeat?.Invoke(result, AnchorInfo, LastHeartBeat);
        }
    }

    [UsedImplicitly]
    protected IEnumerator WsLink()
    {
        foreach (var link in WebsocketInfo.WssLink)
        {
            var connect = WebSocketImpl.ConnectAsync(new Uri(link), CancellationToken.None);
            yield return new WaitUntil(() => connect.IsCompleted);
            if (connect.IsFaulted) continue;
            StartCoroutine(nameof(WsHandle));
            yield return Wait.ForEndOfFrame;
            StartCoroutine(nameof(WsAuth));
            yield return Wait.ForEndOfFrame;
            OnWsLink?.Invoke(WebsocketInfo, link);
            break;
        }
    }

    [UsedImplicitly]
    protected IEnumerator WsAuth()
    {
        if (WebSocketImpl.State != WebSocketState.Open) yield break;
        var auth = WebSocketImpl.SendAsync(
            AuthPacket,
            WebSocketMessageType.Binary,
            true,
            CancellationToken.None);
        yield return new WaitUntil(() => auth.IsCompleted);
    }

    [UsedImplicitly]
    protected IEnumerator WsHeartBeat()
    {
        if (WebSocketImpl.State != WebSocketState.Open) yield break;
        var hb = WebSocketImpl.SendAsync(
            HeartBeatPacket,
            WebSocketMessageType.Binary,
            true,
            CancellationToken.None);
        yield return new WaitUntil(() => hb.IsCompleted);
    }

    [UsedImplicitly]
    protected IEnumerator WsHandle()
    {
        while (WebSocketImpl.State is WebSocketState.Open)
        {
            var buffer = Buffer;
            var receive = WebSocketImpl.ReceiveAsync(buffer, CancellationToken.None);
            yield return new WaitUntil(() => receive.IsCompleted);
            try
            {
                if (receive.IsFaulted) yield break;
                if (receive.Result.MessageType != WebSocketMessageType.Binary) continue;
                HandlePacket(buffer);
            }
            catch (Exception e)
            {
                Buffer = buffer;
                OnWsError?.Invoke(WebsocketInfo, e);
            }
        }
    }

    [UsedImplicitly]
    public static Dictionary<string, string> YellowFace()
    {
        using var fs = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("ZNT.Evolution.Live.Resources.100.json");
        using var reader = new StreamReader(fs ?? throw new FileNotFoundException("100.json"));
        var json = JObject.Load(new JsonTextReader(reader));
        var emote = new Dictionary<string, string>();
        foreach (var element in json["emoticons"])
        {
            var text = element["emoji"].ToString();
            var url = element["url"].ToString();
            emote[text] = url;
        }

        return emote;
    }

    private UnityWebRequest Post(string url, JObject body)
    {
        var content = body.ToString();
        var request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("x-bili-accesskeyid", AccessKeyId);
        request.SetRequestHeader("x-bili-content-md5", Md5(content));
        request.SetRequestHeader("x-bili-signature-method", "HMAC-SHA256");
        request.SetRequestHeader("x-bili-signature-nonce", Guid.NewGuid().ToString());
        request.SetRequestHeader("x-bili-signature-version", "1.0");
        request.SetRequestHeader("x-bili-timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        var headers = new[]
        {
            "x-bili-accesskeyid",
            "x-bili-content-md5",
            "x-bili-signature-method",
            "x-bili-signature-nonce",
            "x-bili-signature-version",
            "x-bili-timestamp",
        }.Join(converter: n => $"{n}:{request.GetRequestHeader(n)}", delimiter: "\n");
        request.SetRequestHeader("Authorization", Sha256(headers, AccessKeySecret));
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(content));
        request.downloadHandler = new DownloadHandlerBuffer();
        return request;
    }

    private void HandlePacket(ArraySegment<byte> buffer)
    {
        var length = BinaryPrimitives.ReadInt32BigEndian(buffer.AsReadOnlySpan().Slice(0x00));
        var version = BinaryPrimitives.ReadInt16BigEndian(buffer.AsReadOnlySpan().Slice(0x06));
        if (version != 0x00) throw new NotSupportedException($"Packet Version: {version}");
        var operation = (WsOperation)BinaryPrimitives.ReadInt32BigEndian(buffer.AsReadOnlySpan().Slice(0x08));
        var body = Encoding.UTF8.GetString(buffer.Array!, 0x10, length - 0x10);
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (operation)
        {
            case WsOperation.OP_AUTH_REPLY:
                OnWsAuth?.Invoke(WebsocketInfo, JObject.Parse(WebsocketInfo.AuthBody));
                break;
            case WsOperation.OP_HEARTBEAT_REPLY:
                OnWsHeartBeat?.Invoke(WebsocketInfo);
                break;
            case WsOperation.OP_SEND_SMS_REPLY:
                HandleMessage(JObject.Parse(body));
                break;
            default:
                throw new NotSupportedException($"Packet Operation: {operation}");
        }
    }

    private void HandleMessage(JObject content)
    {
        var command = content.Value<string>("cmd");
        switch (command)
        {
            case "LIVE_OPEN_PLATFORM_LIVE_ROOM_ENTER":
                OnEnter?.Invoke(content, content["data"].ToObject<Enter>());
                break;
            case "LIVE_OPEN_PLATFORM_DM":
                OnDanmaku?.Invoke(content, content["data"].ToObject<Danmaku>());
                break;
            case "LIVE_OPEN_PLATFORM_SEND_GIFT":
                OnGift?.Invoke(content, content["data"].ToObject<Gift>());
                break;
            case "LIVE_OPEN_PLATFORM_SUPER_CHAT":
                OnSuperChat?.Invoke(content, content["data"].ToObject<SuperChat>());
                break;
            case "LIVE_OPEN_PLATFORM_SUPER_CHAT_DEL":
                OnSuperChatDelete?.Invoke(content, content["data"].ToObject<SuperChatDelete>());
                break;
            case "LIVE_OPEN_PLATFORM_GUARD":
                OnGuard?.Invoke(content, content["data"].ToObject<Guard>());
                break;
            case "LIVE_OPEN_PLATFORM_LIKE":
                // TODO LIVE_OPEN_PLATFORM_LIKE
                break;
            case "LIVE_OPEN_PLATFORM_LIVE_START":
                // TODO LIVE_OPEN_PLATFORM_LIVE_START
                break;
            case "LIVE_OPEN_PLATFORM_LIVE_END":
                // TODO LIVE_OPEN_PLATFORM_LIVE_END
                break;
            case "LIVE_OPEN_PLATFORM_INTERACTION_END":
                WebSocketImpl.CloseAsync(WebSocketCloseStatus.NormalClosure, command, CancellationToken.None);
                break;
        }
    }

    private static ArraySegment<byte> Packet(WsOperation operation, JObject body)
    {
        var content = body?.ToString() ?? string.Empty;
        var bytes = new byte[0x10 + Encoding.UTF8.GetByteCount(content)];
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan().Slice(0x00), bytes.Length);
        BinaryPrimitives.WriteInt16BigEndian(bytes.AsSpan().Slice(0x04), 0x10);
        BinaryPrimitives.WriteInt16BigEndian(bytes.AsSpan().Slice(0x06), 0x00);
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan().Slice(0x08), (int)operation);
        Encoding.UTF8.GetBytes(content, 0, content.Length, bytes, 0x10);
        return new ArraySegment<byte>(bytes);
    }

    private static string Md5(string input)
    {
        using var md5 = MD5.Create();
        return md5
            .ComputeHash(Encoding.UTF8.GetBytes(input))
            .Aggregate(new StringBuilder(), (sb, b) => sb.Append($"{b:x2}"))
            .ToString();
    }

    private static string Sha256(string input, string secret)
    {
        using var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return sha256
            .ComputeHash(Encoding.UTF8.GetBytes(input))
            .Aggregate(new StringBuilder(), (sb, b) => sb.Append($"{b:x2}"))
            .ToString();
    }

    private enum WsOperation
    {
        // ReSharper disable InconsistentNaming
        OP_HEARTBEAT = 2,
        OP_HEARTBEAT_REPLY = 3,
        OP_SEND_SMS_REPLY = 5,
        OP_AUTH = 7,
        OP_AUTH_REPLY = 8,
        // ReSharper restore InconsistentNaming
    }
}