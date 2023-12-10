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

            string raw_str = _advOptions.Value.ConnectionString;
            string temp_str;
            string app_data_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.ConfigParentFolder, App.AppIdentifier);
            string program_root = _advOptions.Value.IsStoreInAppData ? app_data_path : AppDomain.CurrentDomain.BaseDirectory;

            temp_str = raw_str.Replace("{PROGRAM_ROOT}", program_root);

            if (!temp_str.StartsWith("Data Source="))
            {
                temp_str = $"Data Source={temp_str}";
            }

            _connectionString = temp_str;

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
