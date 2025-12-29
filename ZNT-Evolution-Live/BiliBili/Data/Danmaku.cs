using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct Danmaku
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

    [JsonProperty("is_admin")]
    public bool IsAdmin;

    [JsonProperty("dm_type")]
    public string Type;

    [JsonProperty("msg")]
    public string Message;

    [JsonProperty("emoji_img_url")]
    public string EmojiImageUrl;

    [JsonProperty("reply_open_id")]
    public string ReplyOpenId;

    [JsonProperty("reply_union_id")]
    public string ReplyUnionId;

    [JsonProperty("reply_uname")]
    public string ReplyUserName;
}