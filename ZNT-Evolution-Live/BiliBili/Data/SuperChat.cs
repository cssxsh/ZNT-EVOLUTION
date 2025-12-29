using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct SuperChat
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

    [JsonProperty("fans_medal_level")]
    public long FansMedalLevel;

    [JsonProperty("fans_medal_name")]
    public string FansMedalName;

    [JsonProperty("fans_medal_wearing_status")]
    public bool FansMedalWearingStatus;

    [JsonProperty("guard_level")]
    public long GuardLevel;

    [JsonProperty("rmb")]
    public long Cost;

    [JsonProperty("message")]
    public string Message;

    [JsonProperty("start_time")]
    public long StartTime;

    [JsonProperty("end_time")]
    public long EndTime;
}