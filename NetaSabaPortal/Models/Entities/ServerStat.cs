// using Dapperer;
using DapperAid.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetaSabaPortal.Models.Entities
{
    [Table("ServerStats")]
    [SelectSql(DefaultOtherClauses = "ORDER BY Id")]
    public class ServerStat// : IIdentifier<long>
    {
        //[Column("Id", IsPrimary = true, AutoIncrement = true)]
        [Key]
        [InsertValue(false, RetrieveInsertedId = true)]
        [DapperAid.Ddl.DDL("INTEGER")]
        public long Id { get; set; }
        [Column("DemandingWatcherId")]
        public Guid DemandingWatcherId { get; set; }
        [Column("SessionId")]
        public Guid SessionId { get; set; }
        [Column("Timestamp"), UpdateValue(false)]
        public DateTime Timestamp { get; set; }
        [Column("Map")]
        public string Map { get; set; }
        [Column("MaxPlayers")]
        public byte MaxPlayers { get; set; }
        [Column("Players")]
        public byte Players { get; set; }

    }
}
