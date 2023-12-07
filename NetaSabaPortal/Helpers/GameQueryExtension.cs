using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Helpers
{
    public static class GameQueryExtension
    {
        public static async Task<IPEndPoint> GetIPEndpointAsync(string host, int defaultPort = 27015)
        {
            try
            {
                string uri = host;
                string[] uriParts = uri.Split(':');
                string sHostname = uriParts[0];
                string sPort = uriParts.Length > 1 ? uriParts[1] : null;
                int iPort = defaultPort;
                if (!string.IsNullOrWhiteSpace(sPort))
                {
                    int.TryParse(sPort, out iPort);
                }
                bool isDns = Uri.CheckHostName(sHostname) == UriHostNameType.Dns;
                IPAddress addr;
                if (isDns)
                {
                    IPAddress[] addrArr = await Dns.GetHostAddressesAsync(sHostname);
                    addr = addrArr[0];
                }
                else
                {
                    addr = IPAddress.Parse(sHostname);
                }
                var result = new IPEndPoint(addr, iPort);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<SteamQueryNet.Interfaces.IServerQuery> CreateServerQueryInstanceAsync(string host, int sendTimeout = 5000, int recieveTimeout = 30000)
        {
            var localep = new IPEndPoint(IPAddress.Any, 0);
            var remoteep = await GameQueryExtension.GetIPEndpointAsync(host);
            SteamQueryNet.Services.UdpWrapper udp = new SteamQueryNet.Services.UdpWrapper(localep, sendTimeout, recieveTimeout);
            SteamQueryNet.ServerQuery sq = new SteamQueryNet.ServerQuery(udp, remoteep);
            var qq = sq.Connect(remoteep);
            return qq;
        }
    }
}
