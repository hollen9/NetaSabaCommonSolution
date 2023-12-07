
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
using QueryMaster.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueryMaster.Steam
{
    /// <summary>
    /// Contains response of GetBadges method.
    /// </summary>
    [Serializable]
    public class GetBadgesResponse : SteamResponse
    {
        /// <summary>
        /// Parsed response.
        /// </summary>
        [JsonPropertyName("response")]
        public GetBadgesResponseResponse ParsedResponse { get; internal set; }
    }

    [Serializable]
    public class GetBadgesResponseBadge : DataObject
    {
        /// <summary>
        /// Id.
        /// </summary>
        [JsonPropertyName("badgeid")]
        public uint Id { get; internal set; }
        /// <summary>
        /// Application Id of the game(Optional).
        /// </summary>
        [JsonPropertyName("appid")]
        public uint AppId { get; internal set; }
        /// <summary>
        /// Level.
        /// </summary>
        [JsonPropertyName("level")]
        public uint Level { get; internal set; }
        /// <summary>
        /// The date/time when the steam user acquired the badge.
        /// </summary>
        [JsonPropertyName("completion_time")]
        [JsonConverter(typeof(JsonUnixTimeConverter))]
        public DateTime CompletionTime { get; internal set; }
        /// <summary>
        /// The experience this badge is worth, contributing toward the steam account's player_xp. 
        /// </summary>
        [JsonPropertyName("xp")]
        public uint Xp { get; internal set; }
        /// <summary>
        /// The amount of people who has this badge. 
        /// </summary>
        [JsonPropertyName("scarcity")]
        public uint Scarcity { get; internal set; }
        /// <summary>
        /// Provided if the badge relates to an app (trading cards)(Optional).
        /// </summary>
        [JsonPropertyName("communityitemid")]
        public uint CommunityItemId { get; internal set; }
        /// <summary>
        /// Provided if the badge relates to an app (trading cards)(Optional).
        /// </summary>
        [JsonPropertyName("border_color")]
        public string BorderColor { get; internal set; }
    }

    [Serializable]
    public class GetBadgesResponseResponse : DataObject
    {
        /// <summary>
        /// Collection of <see cref="GetBadgesResponseBadge"/> instances.
        /// </summary>
        [JsonPropertyName("badges")]
        public QueryMasterCollection<GetBadgesResponseBadge> Badges { get; internal set; }
        /// <summary>
        /// Player Xp.
        /// </summary>
        [JsonPropertyName("player_xp")]
        public uint PlayerXp { get; internal set; }
        /// <summary>
        /// Player level.
        /// </summary>
        [JsonPropertyName("player_level")]
        public uint PlayerLevel { get; internal set; }
        /// <summary>
        /// Amount of hp needed to level up.
        /// </summary>
        [JsonPropertyName("player_xp_needed_to_level_up")]
        public uint PlayerXpNeededToLevelUp { get; internal set; }
        /// <summary>
        /// Amount of hp needed to complete current level.
        /// </summary>
        [JsonPropertyName("player_xp_needed_current_level")]
        public uint PlayerXpNeededCurrentLevel { get; internal set; }
    }
}
