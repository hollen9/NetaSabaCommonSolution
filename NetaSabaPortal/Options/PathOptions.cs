using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Options
{
    public class PathOptions
    {
        public const string DefaultFileName = "config_path.jsonc";
        public string Steam { get; set; }
        public string Cs2 { get; set; }
        public string Cs2acf { get; set; }
    }
}
