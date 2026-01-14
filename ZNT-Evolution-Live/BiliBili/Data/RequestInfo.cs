using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct RequestInfo
{
    [JsonProperty("request_id")]
    public string RequestId;

    [JsonProperty("code")]
    public int Code;

    [JsonProperty("message")]
    public string Message;
}