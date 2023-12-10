// using Dapperer;

namespace NetaSabaPortal.Models.Entities
{
    //[Table("ServerStats")]
    public class ServerStat// : IIdentifier<long>
    {
        //[Column("Id", IsPrimary = true, AutoIncrement = true)]
        public long Id { get; set; }
        //[Column("DemandingWatcherId")]
        public Guid DemandingWatcherId { get; set; }
        //[Column("SessionId")]
        public Guid SessionId { get; set; }
        //[Column("Timestamp")]
        public DateTime Timestamp { get; set; }
        //[Column("Map")]
        public string Map { get; set; }
        //[Column("MaxPlayers")]
        public byte MaxPlayers { get; set; }
        //[Column("Players")]
        public byte Players { get; set; }

        public long GetIdentity() => Id;

        public void SetIdentity(long identity)
        {
            Id = identity;
        }
    }
}
