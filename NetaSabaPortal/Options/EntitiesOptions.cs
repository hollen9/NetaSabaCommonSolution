using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetaSabaPortal.Models;

namespace NetaSabaPortal.Options
{
    public class EntitiesOptions
    {
        public const string DefaultFileName = "config_entities.json";
        public List<EntityDefinition> Definitions { get; set; }
        public long EntVersion {get;set;}
        public string MinVersionRequired { get; set; }
    }
}
