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
using System.Net;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueryMaster.Steam
{
    [Serializable]
    public class GetServersAtAddressResponse : SteamResponse
    {
        [JsonPropertyName("response")]
        public GetServersAtAddressResponseResponse ParsedResponse { get; internal set; }
    }

    [Serializable]
    public class GetServersAtAddressResponseResponse : DataObject
    {
        [JsonPropertyName("success")]
        public bool IsSuccess { get; internal set; }

        [JsonPropertyName("servers")]
        public QueryMasterCollection<GetServersAtAddressResponseServer> Servers { get; internal set; }
    }

    [Serializable]
    public class GetServersAtAddressResponseServer : DataObject
    {
        [JsonPropertyName("addr"), JsonConverter(typeof(StringIpEndPointConverter))]
        public IPEndPoint Endpoints { get; internal set; }

        [JsonPropertyName("gmsindex")]
        public int GMSIndex { get; internal set; }

        [JsonPropertyName("appid")]
        public uint AppId { get; internal set; }

        [JsonPropertyName("gamedir")]
        public string GameDirectory { get; internal set; }

        [JsonPropertyName("region")]
        public int Region { get; internal set; }

        [JsonPropertyName("secure")]
        public bool IsSecure { get; internal set; }

        [JsonPropertyName("lan")]
        public bool IsLan { get; internal set; }

        [JsonPropertyName("gameport")]
        public int GamePort { get; internal set; }

        [JsonPropertyName("specport")]
        public int SpecPort { get; internal set; }
    }
}

