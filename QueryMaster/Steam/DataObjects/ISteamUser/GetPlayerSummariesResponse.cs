
#region License
/*
Copyright (c) 2015 Betson Roy

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueryMaster.Steam
{
    [Serializable]
    public class GetPlayerSummariesResponse : SteamResponse
    {
        [JsonPropertyName("response")]
        public GetPlayerSummariesResponseResponse ParsedResponse { get; internal set; }
    }

    [Serializable]
    public class GetPlayerSummariesResponsePlayer : DataObject
    {
        [JsonPropertyName("steamid")]
        public ulong SteamId { get; internal set; }

        [JsonPropertyName("communityvisibilitystate")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CommunityVisibilityState CommunityVisibilityState { get; internal set; }

        [JsonPropertyName("profilestate")]
        public bool ProfileState { get; internal set; }

        [JsonPropertyName("personaname")]
        public string PersonaName { get; internal set; }

        [JsonPropertyName("lastlogoff")]
        [JsonConverter(typeof(IntegerUnixTimeStampConverter))]
        public DateTime LastLogOff { get; internal set; }

        [JsonPropertyName("profileurl")]
        public string ProfileUrl { get; internal set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; internal set; }

        [JsonPropertyName("avatarmedium")]
        public string AvatarMedium { get; internal set; }

        [JsonPropertyName("avatarfull")]
        public string AvatarFull { get; internal set; }

        [JsonPropertyName("personastate")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PersonaState PersonaState { get; internal set; }

        [JsonPropertyName("commentpermission")]
        public bool? CommentPermission { get; internal set; }

        [JsonPropertyName("realname")]
        public string RealName { get; internal set; }

        [JsonPropertyName("primaryclanid")]
        public ulong? PrimaryClanId { get; internal set; }

        [JsonPropertyName("timecreated")]
        [JsonConverter(typeof(IntegerUnixTimeStampConverter))]
        public DateTime? TimeCreated { get; internal set; }

        [JsonPropertyName("loccountrycode")]
        public string LocCountryCode { get; internal set; }

        [JsonPropertyName("locstatecode")]
        public string LocStateCode { get; internal set; }

        [JsonPropertyName("loccityid")]
        public string LocCityId { get; internal set; }

        [JsonPropertyName("gameid")]
        public uint GameId { get; internal set; }

        [JsonPropertyName("gameextrainfo")]
        public string GameExtraInfo { get; internal set; }

        [JsonPropertyName("gameserverip")]
        [JsonConverter(typeof(StringIpEndPointConverter))]
        public IPEndPoint GameServerIp { get; internal set; }
    }

    [Serializable]
    public class GetPlayerSummariesResponseResponse : DataObject
    {
        [JsonPropertyName("players")]
        public QueryMasterCollection<GetPlayerSummariesResponsePlayer> Players { get; internal set; }
    }
}
