using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Models
{
    public class GameServerRecord
    {
        public List<DateTime> Heartbeats { get; set; }
        public List<SteamQueryNet.Models.ServerInfo> infoItems { get; set; }

    }
}
