using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using EmbedIO.WebSockets;

namespace ZNT.Evolution.Host;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        using var server = new WebServer("http://localhost:8080/")
            .WithWebApi("/api", m => m.WithController<ApiController>())
            .WithModule(new WebSocketsChatServer("/chat"))
            .WithStaticFolder("/", "./www", true);
        await server.RunAsync();
    }
}

public class ApiController : WebApiController
{
    [Route(HttpVerbs.Get, "/hello")]
    public object SayHello() => new { hello = "world" };
}

public class WebSocketsChatServer : WebSocketModule
{
    public WebSocketsChatServer(string urlPath) : base(urlPath, true)
    {
        // placeholder
    }

    /// <inheritdoc />
    protected override Task OnMessageReceivedAsync(
        IWebSocketContext context,
        byte[] rxBuffer,
        IWebSocketReceiveResult rxResult)
        => SendToOthersAsync(context, Encoding.GetString(rxBuffer));

    /// <inheritdoc />
    protected override Task OnClientConnectedAsync(IWebSocketContext context)
        => Task.WhenAll(
            SendAsync(context, "Welcome to the chat room!"),
            SendToOthersAsync(context, "Someone joined the chat room."));

    /// <inheritdoc />
    protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        => SendToOthersAsync(context, "Someone left the chat room.");

    private Task SendToOthersAsync(IWebSocketContext context, string payload)
        => BroadcastAsync(payload, c => c != context);
}