
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
    public class GetSupportedAPIListResponse : SteamResponse
    {
        [JsonPropertyName("apilist")]
        public GetSupportedAPIListResponseApilist ParsedResponse { get; internal set; }
    }

    [Serializable]
    public class GetSupportedAPIListResponseApilist : DataObject
    {
        [JsonPropertyName("interfaces")]
        public QueryMasterCollection<GetSupportedAPIListResponseInterface> Interfaces { get; internal set; }
    }

    [Serializable]
    public class GetSupportedAPIListResponseInterface : DataObject
    {
        [JsonPropertyName("name")]
        public string Name { get; internal set; }

        [JsonPropertyName("methods")]
        public QueryMasterCollection<GetSupportedAPIListResponseMethod> Methods { get; internal set; }
    }

    [Serializable]
    public class GetSupportedAPIListResponseMethod : DataObject
    {
        [JsonPropertyName("name")]
        public string Name { get; internal set; }

        [JsonPropertyName("version")]
        public int Version { get; internal set; }

        [JsonPropertyName("httpmethod")]
        public string HttpMethod { get; internal set; }

        [JsonPropertyName("parameters")]
        public List<GetSupportedAPIListResponseParameter> Parameters { get; internal set; }
    }

    [Serializable]
    public class GetSupportedAPIListResponseParameter : DataObject
    {
        [JsonPropertyName("name")]
        public string Name { get; internal set; }

        [JsonPropertyName("type")]
        public string Type { get; internal set; }

        [JsonPropertyName("optional")]
        public bool IsOptional { get; internal set; }

        [JsonPropertyName("description")]
        public string Description { get; internal set; }
    }
}
