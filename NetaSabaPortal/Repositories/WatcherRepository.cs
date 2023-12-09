using Microsoft.Extensions.Options;
using NetaSabaPortal.Options;
using NetaSabaPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Repositories
{
    public class WatcherRepository : IRepository
    {
        private readonly IConnectionProvider _connProvider;
        private readonly IOptions<AdvancedOptions> _advOptions;

        public WatcherRepository(IConnectionProvider connProvider, IOptions<AdvancedOptions> advOptions)
        {
            _connProvider = connProvider;
            _advOptions = advOptions;
        }


    }
}
