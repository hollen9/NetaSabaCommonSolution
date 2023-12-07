
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
    public class GetSchemaForGameResponse : SteamResponse
    {
        [JsonPropertyName("game")]
        public GetSchemaForGameResponseGame ParsedResponse { get; internal set; }
    }

    [Serializable]
    public class GetSchemaForGameResponseGame : DataObject
    {
        [JsonPropertyName("gameName")]
        public string Name { get; internal set; }

        [JsonPropertyName("gameVersion")]
        public uint Version { get; internal set; }

        [JsonPropertyName("availableGameStats")]
        public GetSchemaForGameResponseAvailableGameStats AvailableGameStats { get; internal set; }
    }

    [Serializable]
    public class GetSchemaForGameResponseAvailableGameStats : DataObject
    {
        [JsonPropertyName("stats")]
        public List<GetSchemaForGameResponseStat> Stats { get; internal set; }

        [JsonPropertyName("achievements")]
        public List<GetSchemaForGameResponseAchievement> Achievements { get; internal set; }
    }

    [Serializable]
    public class GetSchemaForGameResponseStat : DataObject
    {
        [JsonPropertyName("name")]
        public string Name { get; internal set; }

        [JsonPropertyName("defaultvalue")]
        public ulong DefaultValue { get; internal set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; internal set; }
    }

    [Serializable]
    public class GetSchemaForGameResponseAchievement : DataObject
    {
        [JsonPropertyName("name")]
        public string Name { get; internal set; }

        [JsonPropertyName("defaultvalue")]
        public int DefaultValue { get; internal set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; internal set; }

        [JsonPropertyName("hidden")]
        public bool IsHidden { get; internal set; }

        [JsonPropertyName("description")]
        public string Description { get; internal set; }

        [JsonPropertyName("icon")]
        public string Icon { get; internal set; }

        [JsonPropertyName("icongray")]
        public string IconGray { get; internal set; }
    }
}
