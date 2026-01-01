using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct Enter
{
    [JsonProperty("msg_id")]
    public string Id;

    [JsonProperty("room_id")]
    public long RoomId;

    [JsonProperty("open_id")]
    public string OpenId;

    [JsonProperty("union_id")]
    public string UnionId;

    [JsonProperty("timestamp")]
    public long Timestamp;

    [JsonProperty("uid")]
    public long UserId;

    [JsonProperty("uname")]
    public string UserName;

    [JsonProperty("uface")]
    public string UserFace;
}