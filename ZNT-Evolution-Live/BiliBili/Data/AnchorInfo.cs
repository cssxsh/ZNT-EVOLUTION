using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct AnchorInfo
{
    [JsonProperty("room_id")]
    public long RoomId;

    [JsonProperty("uid")]
    public long UserId;

    [JsonProperty("open_id")]
    public string OpenId;

    [JsonProperty("union_id")]
    public string UnionId;

    [JsonProperty("uname")]
    public string UserName;

    [JsonProperty("uface")]
    public string UserFace;
}