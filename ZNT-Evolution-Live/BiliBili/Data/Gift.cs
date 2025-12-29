using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct Gift
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

    [JsonProperty("glory_level")]
    public long GloryLevel;

    [JsonProperty("paid")]
    public bool IsPaid;

    [JsonProperty("gift_id")]
    public long GiftId;

    [JsonProperty("gift_name")]
    public string GiftName;

    [JsonProperty("gift_num")]
    public long Amount;

    [JsonProperty("price")]
    public long Price;

    [JsonProperty("r_price")]
    public long RealPrice;
}