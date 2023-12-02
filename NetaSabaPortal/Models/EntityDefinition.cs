using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Models
{
    /// <summary>
    /// "entities": [
    ///  {
    ///    "entityName": "KuromeKuro - DLC",
    ///    "workshopId": "3101610901",
    ///    "serverId": "kuromekuro",
    ///    "copy": [
    ///      "sounds/kurome"
    ///    ]
    ///  }
    /// ]
    /// </summary>
    public class EntityDefinition
    {
        public required string EntityName { get; set; }
        public required string WorkshopId { get; set; }
        public required string ServerId { get; set; }
        public required string[] Copies { get; set; }
        public required string[] Types { get; set; }
        public string? EntityDescription { get; set; }
        public bool? IsDefault { get; set; }
    }
}
