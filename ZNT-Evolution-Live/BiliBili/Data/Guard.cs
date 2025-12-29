using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct Guard
{
    [JsonProperty("msg_id")]
    public string Id;

    [JsonProperty("room_id")]
    public long RoomId;

    [JsonProperty("timestamp")]
    public long Timestamp;

    [JsonProperty("user_info")]
    public AnchorInfo UserInfo;

    [JsonProperty("fans_medal_level")]
    public long FansMedalLevel;

    [JsonProperty("fans_medal_name")]
    public string FansMedalName;

    [JsonProperty("fans_medal_wearing_status")]
    public bool FansMedalWearingStatus;

    [JsonProperty("guard_level")]
    public long GuardLevel;

    [JsonProperty("guard_num")]
    public long Amount;

    [JsonProperty("guard_unit")]
    public string GuardUnit;

    [JsonProperty("price")]
    public long Price;
}