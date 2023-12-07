
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
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueryMaster.Steam
{
    /// <summary>
    /// Represents the ISteamWebAPIUtil interface.
    /// </summary>
    public class ISteamWebApiUtil : InterfaceBase
    {
        internal ISteamWebApiUtil()
        {
            Interface = "ISteamWebAPIUtil";
        }

        /// <summary>
        /// Gets WebAPI server time and checks server status(GetServerInfo web api method(version 1)).
        /// </summary>
        /// <returns>Instance of <see cref="GetServerInfoResponse"/>.</returns>
        public async Task<GetServerInfoResponse> GetServerInfoAsync()
        {
            SteamUrl url = new SteamUrl { Interface = Interface, Method = "GetServerInfo", Version = 1 };
            string responseString = await new HttpClient().GetStringAsync(url.ToString());
            return ParseResponse<GetServerInfoResponse>(responseString, true);
        }

        /// <summary>
        /// Lists all available WebAPI interfaces(GetSupportedAPIList web api method(version 1)).
        /// </summary>
        /// <param name="appendKey">if true then response would include all available methods and interfaces allowed for that key.</param>
        /// <returns>Instance of <see cref="GetSupportedAPIListResponse"/>.</returns>
        public async Task<GetSupportedAPIListResponse> GetSupportedAPIListAsync(bool appendKey = false)
        {
            SteamUrl url = new SteamUrl { Interface = Interface, Method = "GetSupportedAPIList", Version = 1, AppendKey = appendKey };
            string responseString = await new HttpClient().GetStringAsync(url.ToString());
            return ParseResponse<GetSupportedAPIListResponse>(responseString);
        }
    }
}
