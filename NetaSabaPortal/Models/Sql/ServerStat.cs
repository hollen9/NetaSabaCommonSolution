using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Models.Sql
{
    public class ServerStat
    {
        public long? Id { get; set; }
        public Guid DemandingWatcherId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Map { get; set; }
        public byte MaxPlayers { get; set; }
        public byte Players { get; set; }
    }
}
