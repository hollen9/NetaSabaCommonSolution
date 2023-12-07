
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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueryMaster.Steam
{
    [Serializable]
    public class GetPlayerAchievementsResponse : SteamResponse
    {
        [JsonPropertyName("playerstats")]
        public GetPlayerAchievementsResponsePlayerstats ParsedResponse { get; internal set; }
    }

    [Serializable]
    public class GetPlayerAchievementsResponseAchievement : DataObject
    {
        [JsonPropertyName("apiname")]
        public string Achievement { get; internal set; }

        [JsonPropertyName("achieved")]
        public bool HasAchieved { get; internal set; }

        [JsonPropertyName("name")]
        public string Name { get; internal set; }

        [JsonPropertyName("description")]
        public string Description { get; internal set; }
    }

    [Serializable]
    public class GetPlayerAchievementsResponsePlayerstats : DataObject
    {
        [JsonPropertyName("steamID")]
        public ulong SteamID { get; internal set; }

        [JsonPropertyName("gameName")]
        public string GameName { get; internal set; }

        [JsonPropertyName("achievements")]
        public List<GetPlayerAchievementsResponseAchievement> Achievements { get; internal set; }

        [JsonPropertyName("success")]
        public bool Success { get; internal set; }
    }
}

