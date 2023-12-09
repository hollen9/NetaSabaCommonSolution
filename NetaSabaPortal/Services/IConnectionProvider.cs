using System.Data;

namespace NetaSabaPortal.Services
{
    public interface IConnectionProvider
    {
        IDbConnection Connect();
    }
}