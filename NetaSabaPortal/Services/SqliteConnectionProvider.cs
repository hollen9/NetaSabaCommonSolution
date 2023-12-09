using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetaSabaPortal.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Services
{
    public class SqliteConnectionProvider: IConnectionProvider
    {
        IOptions<AdvancedOptions> _advOptions;
        private readonly string _connectionString;

        public SqliteConnectionProvider(IOptions<AdvancedOptions> advOptions)
        {
            _advOptions = advOptions;
            _connectionString = _advOptions.Value.ConnectionString;

            // SQLite3, which is the current supported version,
            // does not need you to create the file first, just open a connection to whatever path you want. 

            // get sqlite file path from _connectionString
            //var builder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(_connectionString);
            //var filePath = builder.DataSource;
            //if (!File.Exists(filePath))
            //{
            //    using (var conn = (SqliteConnection) Connect())
            //    {
            //        conn.CreateFile
            //    }
            //}
        }

        public IDbConnection Connect()
            => new Microsoft.Data.Sqlite.SqliteConnection(_connectionString);
    }
}
