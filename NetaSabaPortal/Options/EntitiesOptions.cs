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
        public const string DefaultFileName = "config_entities.jsonc";
        public List<EntityDefinition> Definitions { get; set; }
    }
}
