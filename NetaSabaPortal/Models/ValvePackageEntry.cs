using SteamDatabase.ValvePak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Models
{
    public class ValvePackageEntry : PackageEntry
    {
        public bool IsNested { get; set; }
        public string? NestedPath { get; set; }
    }
}
