using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.WebApi;

namespace ZNT.Evolution.Host;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        using var server = new WebServer("http://localhost:8080/")
            .WithWebApi("/api", m => m.WithController<ApiController>())
            .WithModule(new GameServer("/game"))
            .WithStaticFolder("/", "./www", true);
        await server.RunAsync();
    }
}