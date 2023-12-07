using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueryMaster.Steam
{
    [Serializable]
    public class GetSteamLevelResponse : SteamResponse
    {
        [JsonPropertyName("response")]
        public GetSteamLevelResponseResponse ParsedResponse { get; internal set; }
    }

    [Serializable]
    public class GetSteamLevelResponseResponse : DataObject
    {
        [JsonPropertyName("player_level")]
        public int Level { get; internal set; }
    }
}
