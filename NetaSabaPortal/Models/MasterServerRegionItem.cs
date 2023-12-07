using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Models
{
    public class MasterServerRegionItem : ObservableObject
    {
        public QueryMaster.MasterServer.Region Region { get; set; }
        public string DisplayName { get; set; }
        public int Code => (int)Region;
    }
}
