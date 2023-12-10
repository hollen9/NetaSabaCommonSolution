using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Options
{
    public class AdvancedOptions
    {
        public const string DefaultFileName = "config_advanced.jsonc";
        public List<string> Langs { get; set; }
        public bool IsPermanentDeleteNoRecycleBin { get; set; }
        public bool IsDisabledAutoDelete { get; set; }
        public string? ConnectionString { get; set; }
        public bool IsStoreInAppData { get; set; }
        public bool IsForceInitEntities { get; set; }
        public string[] EntitiesUpdateEndpoints { get; set; }
    }
}
