
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
using System.Text.RegularExpressions;

namespace QueryMaster
{
    public class StringIpEndPointConverter : JsonConverter<IPEndPoint>
    {
        public override IPEndPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
            if (Regex.Match(value, @"^(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}):(\d{1,5})$").Success)
            {
                return Util.ToIPEndPoint(value);
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, IPEndPoint value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }

    public class StringIpEndPointCollectionConverter : JsonConverter<QueryMasterCollection<IPEndPoint>>
    {
        public override QueryMasterCollection<IPEndPoint> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<IPEndPoint> endPoints = new List<IPEndPoint>();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                string value = reader.GetString();
                if (Regex.Match(value, @"^(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}):(\d{1,5})$").Success)
                {
                    endPoints.Add(Util.ToIPEndPoint(value));
                }
                reader.Read();
            }
            return new QueryMasterCollection<IPEndPoint>(endPoints);
        }

        public override void Write(Utf8JsonWriter writer, QueryMasterCollection<IPEndPoint> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var endPoint in value)
            {
                writer.WriteStringValue(endPoint.ToString());
            }
            writer.WriteEndArray();
        }
    }
}

