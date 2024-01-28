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
        public string EntityName { get; set; }
        public string WorkshopId { get; set; }
        public string ServerId { get; set; }
        public string[] Copies { get; set; }
        public string[] Types { get; set; }
        public string[] Nested { get; set; }
        public Dictionary<string,string> EntityNames { get; set; }
        public Dictionary<string,string> EntityDescriptions { get; set; }
        public string Image { get; set; }
        public string ImageBackground { get; set; }
        public bool? IsDefault { get; set; }

        public bool? IsDeleteObsolete { get; set; }
        public string[] DeleteExplicitly { get; set; }
    }
}
