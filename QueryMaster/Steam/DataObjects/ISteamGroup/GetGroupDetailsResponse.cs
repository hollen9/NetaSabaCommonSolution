
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
    public class GetGroupDetailsResponse : SteamResponse
    {
        [JsonPropertyName("memberList")]
        public GetGroupDetailsResponseMemberList ParsedResponse { get; set; }

        public override string GetRawResponse(Format format)
        {
            switch (format)
            {
                case Format.Json:
                    return string.Empty;
                case Format.Vdf:
                    return string.Empty;
                case Format.Xml:
                    return ReceivedResponse;
                default:
                    return string.Empty;
            }
        }
    }

    [Serializable]
    public class GetGroupDetailsResponseMemberList : DataObject
    {
        [JsonPropertyName("groupID64")]
        public string SteamId { get; set; }

        [JsonPropertyName("groupDetails")]
        public GetGroupDetailsResponseDetails Details { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("startingMember")]
        public ulong StartingMember { get; set; }

        [JsonPropertyName("steamID64")]
        public QueryMasterCollection<ulong> Members { get; set; }
    }

    [Serializable]
    public class GetGroupDetailsResponseDetails : DataObject
    {
        [JsonPropertyName("groupName")]
        public string Name { get; set; }

        [JsonPropertyName("groupURL")]
        public string URL { get; set; }

        [JsonPropertyName("headline")]
        public string Headline { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("avatarIcon")]
        public string AvatarIcon { get; set; }

        [JsonPropertyName("avatarMedium")]
        public string AvatarMedium { get; set; }

        [JsonPropertyName("avatarFull")]
        public string AvatarFull { get; set; }

        [JsonPropertyName("memberCount")]
        public string MemberCount { get; set; }

        [JsonPropertyName("membersInChat")]
        public string MembersInChat { get; set; }

        [JsonPropertyName("membersInGame")]
        public string MembersInGame { get; set; }

        [JsonPropertyName("membersOnline")]
        public string MembersOnline { get; set; }
    }
}

