//// using Dapperer;
//using DapperAid.DataAnnotations;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Security.RightsManagement;

//namespace NetaSabaPortal.Models.Entities
//{
//    [Table("SessionEvents")]
//    [SelectSql(DefaultOtherClauses = "ORDER BY Id")]
//    public class SessionEvent
//    {
//        [Key]
//        [InsertValue(false, RetrieveInsertedId = true)]
//        [DapperAid.Ddl.DDL("INTEGER")]
//        public long Id { get; set; }
//        public Guid DemandingWatcherId { get; set; }
//        public Guid SessionId { get; set; }
//        public DateTime Timestamp { get; set; }
//        public bool NotifyHostAvailable { get; set; }
//        public bool NotifyMapChanged { get; set; }
//        public bool NotifyPlayerSlotAvailable { get; set; }
//        public bool AjHostAvailable { get; set; }
//        public bool AjMapChanged { get; set; }
//        public bool AjPlayerSlotAvailable { get; set; }
//    }
//}
