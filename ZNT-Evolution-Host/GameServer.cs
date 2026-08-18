using System;
using System.IO;
using System.Threading.Tasks;
using EmbedIO.WebSockets;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace ZNT.Evolution.Host;

public class GameServer : WebSocketModule
{
    // ReSharper disable InconsistentNaming
    public const uint Version = 0x00080000u;
    // ReSharper restore InconsistentNaming

    public GameServer(string urlPath) : base(urlPath, true)
    {
        // ...
    }

    protected override Task OnMessageReceivedAsync(
        IWebSocketContext context,
        byte[] rxBuffer,
        IWebSocketReceiveResult rxResult)
    {
        if (rxResult.MessageType is not 1) return Task.CompletedTask; // Only Binary
        using var buffer = new MemoryStream(rxBuffer);
        using var binary = new BinaryReader(buffer);
        var version = binary.ReadUInt32();
        if (version > Version) throw new NotSupportedException($"Packet Version: {version}");
        // ReSharper disable once UnusedVariable
        var timestamp = binary.ReadInt64();
        var operation = (WsOperation)binary.ReadInt32();
        switch (operation)
        {
            case WsOperation.OP_CLOSE:
                return CloseAsync(context);
            case WsOperation.HUMAN_LIST:
            {
                using var bson = new BsonDataReader(buffer);
                // ReSharper disable once UnusedVariable
                var body = JObject.Load(bson);
                // $"{timestamp} {body}".Info("HUMAN_LIST");
                return Task.CompletedTask;
            }
            default:
                throw new NotSupportedException($"Packet Operation: {operation}");
        }
    }

    protected override Task OnClientConnectedAsync(IWebSocketContext context)
    {
        return Task.CompletedTask;
    }

    protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
    {
        return Task.CompletedTask;
    }

    private enum WsOperation
    {
        // ReSharper disable InconsistentNaming
        OP_CLOSE = 0,
        HUMAN_LIST = 0x0001_0002,
        // ReSharper restore InconsistentNaming
    }
}