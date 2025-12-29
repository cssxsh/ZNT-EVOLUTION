using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct WebsocketInfo
{
    [JsonProperty("auth_body")]
    public string AuthBody;

    [JsonProperty("wss_link")]
    public string[] WssLink;
}