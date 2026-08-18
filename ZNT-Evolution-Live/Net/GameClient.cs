using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ZNT.Evolution.Live.Net;

public class GameClient : MonoBehaviour
{
    // ReSharper disable InconsistentNaming
    public const uint Version = 0x00080000u;
    public Uri Link = new("ws://localhost:8080/game");
    private readonly ClientWebSocket WebSocketImpl = new();
    // ReSharper restore InconsistentNaming

    private static readonly ConcurrentBag<ArraySegment<byte>> BufferPooling = new();

    private static ArraySegment<byte> Buffer
    {
        get => BufferPooling.TryTake(out var buffer) ? buffer : WebSocket.CreateServerBuffer(0x00002000);
        set => BufferPooling.Add(value);
    }

    // ReSharper disable InconsistentNaming
    public event Action<Uri> OnWsLink;
    public event Action<Uri, Exception> OnWsError;
    // ReSharper restore InconsistentNaming

    private void OnEnable() => StartCoroutine(nameof(WsLink));

    private void OnDisable() => StartCoroutine(nameof(WsClose));

    [UsedImplicitly]
    protected IEnumerator WsLink()
    {
        while (enabled)
        {
            var connect = WebSocketImpl.ConnectAsync(Link, CancellationToken.None);
            yield return new WaitUntil(() => connect.IsCompleted);
            if (connect.IsFaulted) continue;
            StartCoroutine(nameof(WsHandle));
            yield return Wait.ForEndOfFrame;
            OnWsLink?.Invoke(Link);
            break;
        }
    }

    [UsedImplicitly]
    protected IEnumerator WsClose()
    {
        if (WebSocketImpl.State != WebSocketState.Open) yield break;
        var close = WebSocketImpl.SendAsync(
            Packet(WsOperation.WS_CLOSE, null),
            WebSocketMessageType.Binary,
            true,
            CancellationToken.None);
        yield return new WaitUntil(() => close.IsCompleted);
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
                OnWsError?.Invoke(Link, e);
            }
        }
    }

    [UsedImplicitly]
    protected IEnumerator WsHumanList()
    {
        if (WebSocketImpl.State != WebSocketState.Open) yield break;
        var list = new JArray();
        // LevelManager.Mode
        // Console.WriteLine(Execution.SceneMode);
        if (EndGameCondition.HasInstance())
        {
            var humans = Traverse.Create(EndGameCondition.Instance.KillHumanCondition)
                .Field<HashSet<HumanBehaviour>>("humans").Value;
            foreach (var human in humans)
            {
                var magazine = Traverse.Create(human.Weapon).Field<Magazine>("currentMagazine").Value;
                var position = human.transform.position;
                list.Add(new JObject
                {
                    ["id"] = human.gameObject.GetInstanceID(),
                    ["name"] = human.name,
                    ["layer"] = LayerMask.LayerToName(human.gameObject.layer),
                    ["position"] = new JObject
                    {
                        ["x"] = position.x,
                        ["y"] = position.y,
                        ["z"] = position.z,
                    },
                    ["health_hp"] = human.Health.Hp,
                    ["health_max_hp"] = human.Health.MaxHp,
                    ["attacker_damage"] = human.Attacker.Damage,
                    ["magazine_count"] = magazine.Count,
                    ["magazine_size"] = magazine.Size
                });
            }
        }

        var auth = WebSocketImpl.SendAsync(
            Packet(WsOperation.HUMAN_LIST, new JObject { ["total"] = 0, ["humans"] = list }),
            WebSocketMessageType.Binary,
            true,
            CancellationToken.None);
        yield return new WaitUntil(() => auth.IsCompleted);
    }

    private void HandlePacket(ArraySegment<byte> packet)
    {
        using var buffer = new MemoryStream(packet.Array!, packet.Offset, packet.Count);
        using var binary = new BinaryReader(buffer);
        var version = binary.ReadUInt32();
        if (version > Version) throw new NotSupportedException($"Packet Version: {version}");
        // ReSharper disable once UnusedVariable
        var timestamp = binary.ReadInt64();
        var operation = (WsOperation)binary.ReadInt32();
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (operation)
        {
            case WsOperation.HUMAN_LIST:
            {
                // using var bson = new BsonReader(buffer);
                // _ = JObject.Load(bson);
                StartCoroutine(nameof(WsHumanList));
            }
                break;
            case WsOperation.X:
            {
                using var bson = new BsonDataReader(buffer);
                _ = JObject.Load(bson);
            }
                break;
            default:
                throw new NotSupportedException($"Packet Operation: {operation}");
        }
    }

    private static ArraySegment<byte> Packet(WsOperation operation, JObject body)
    {
        using var buffer = new MemoryStream();
        using var binary = new BinaryWriter(buffer);
        binary.Write(Version);
        binary.Write(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        binary.Write((int)operation);
        binary.Flush();
        if (body is null) return new ArraySegment<byte>(buffer.ToArray());
        using var bson = new BsonDataWriter(buffer);
        body.WriteTo(bson);
        bson.Flush();
        return new ArraySegment<byte>(buffer.ToArray());
    }

    private enum WsOperation
    {
        // ReSharper disable InconsistentNaming
        WS_CLOSE = 0x0000_0000,
        LEVEL_STATUS = 0x0000_0001,
        HUMAN = 0x0001_0001,
        HUMAN_LIST = 0x0001_0002,
        ZOMBIE = 0x0002_0001,
        SENTRY_GUN = 0x0003_0001,
        SPAWN_POINT = 0x0004_0001,
        X = 1748,
        // ReSharper restore InconsistentNaming
    }

    // UnityEngine.Object.FindObjectFromInstanceID(instanceID)
}