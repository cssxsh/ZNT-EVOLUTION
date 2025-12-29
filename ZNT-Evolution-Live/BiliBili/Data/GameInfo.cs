using Newtonsoft.Json;

namespace ZNT.Evolution.Live.BiliBili.Data;

public struct GameInfo
{
    [JsonProperty("game_id")]
    public string GameId;
}