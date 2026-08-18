using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace ZNT.Evolution.Host;

public class ApiController : WebApiController
{
    [Route(HttpVerbs.Get, "/hello")]
    public object SayHello() => new { hello = "world" };
}