
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
    public class GetNewsForAppResponse : SteamResponse
    {
        [JsonPropertyName("appnews")]
        public GetNewsForAppResponseAppNews ParsedResponse { get; internal set; }
    }

    [Serializable]
    public class GetNewsForAppResponseNewsItem : DataObject
    {
        [JsonPropertyName("gid")]
        public string GId { get; internal set; }

        [JsonPropertyName("title")]
        public string Title { get; internal set; }

        [JsonPropertyName("url")]
        public string Url { get; internal set; }

        [JsonPropertyName("is_external_url")]
        public bool IsExternalUrl { get; internal set; }

        [JsonPropertyName("author")]
        public string Author { get; internal set; }

        [JsonPropertyName("contents")]
        public string Contents { get; internal set; }

        [JsonPropertyName("feedlabel")]
        public string FeedLabel { get; internal set; }

        [JsonPropertyName("date"), JsonConverter(typeof(IntegerUnixTimeStampConverter))]
        public DateTime Date { get; internal set; }

        [JsonPropertyName("feedname")]
        public string FeedName { get; internal set; }
    }

    [Serializable]
    public class GetNewsForAppResponseAppNews : DataObject
    {
        [JsonPropertyName("appid")]
        public int AppId { get; internal set; }

        [JsonPropertyName("newsitems")]
        public QueryMasterCollection<GetNewsForAppResponseNewsItem> NewsItems { get; internal set; }
    }
}
