﻿
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
    public class GetRecentlyPlayedGamesResponse : SteamResponse
    {
        [JsonPropertyName("response")]
        public GetRecentlyPlayedGamesResponseResponse ParsedResponse { get; internal set; }
    }

    [Serializable]
    public class GetRecentlyPlayedGamesResponseResponse : DataObject
    {
        [JsonPropertyName("total_count")]
        public uint Count { get; internal set; }

        [JsonPropertyName("games")]
        public List<GetRecentlyPlayedGamesResponseGame> Games { get; internal set; }
    }

    [Serializable]
    public class GetRecentlyPlayedGamesResponseGame : DataObject
    {
        [JsonPropertyName("appid")]
        public uint AppId { get; internal set; }

        [JsonPropertyName("name")]
        public string Name { get; internal set; }

        [JsonPropertyName("playtime_2weeks")]
        [JsonConverter(typeof(IntegerTimeSpanConverter))]
        public TimeSpan Playtime2Weeks { get; internal set; }

        [JsonPropertyName("playtime_forever")]
        [JsonConverter(typeof(IntegerTimeSpanConverter))]
        public TimeSpan PlaytimeForever { get; internal set; }

        [JsonPropertyName("img_icon_url")]
        public string IconUrl { get; internal set; }

        [JsonPropertyName("img_logo_url")]
        public string LogoUrl { get; internal set; }
    }
}
