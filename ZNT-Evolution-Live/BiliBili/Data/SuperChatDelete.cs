using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct SuperChatDelete
{
    [JsonProperty("room_id")]
    public long RoomId;

    [JsonProperty("message_ids")]
    public long[] MessageIds;
}