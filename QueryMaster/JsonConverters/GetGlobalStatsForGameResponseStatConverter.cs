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
using QueryMaster.Steam;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QueryMaster
{
    public class GetGlobalStatsForGameResponseStatConverter : JsonConverter<QueryMasterCollection<GetGlobalStatsForGameResponseStat>>
    {
        public override QueryMasterCollection<GetGlobalStatsForGameResponseStat> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            List<GetGlobalStatsForGameResponseStat> stats = new List<GetGlobalStatsForGameResponseStat>();
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                {
                    stats.Add(new GetGlobalStatsForGameResponseStat
                    {
                        Name = property.Name,
                        Total = property.Value.GetUInt64()
                    });
                }
            }
            return new QueryMasterCollection<GetGlobalStatsForGameResponseStat>(stats);
        }

        public override void Write(
            Utf8JsonWriter writer,
            QueryMasterCollection<GetGlobalStatsForGameResponseStat> value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}