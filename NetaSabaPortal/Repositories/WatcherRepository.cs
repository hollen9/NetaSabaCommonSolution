using Microsoft.Extensions.Options;
using NetaSabaPortal.Models.Sql;
using NetaSabaPortal.Options;
using NetaSabaPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using System.Data;

namespace NetaSabaPortal.Repositories
{
    public class WatcherRepository
    {
        private readonly IConnectionProvider _connProvider;
        
        public WatcherRepository(IConnectionProvider connProvider) : base()
        {
            _connProvider = connProvider;

            CheckAndCreateTables();
        }

        private void CheckAndCreateTables()
        {
            using (var conn = _connProvider.Connect())
            {
                //public long? Id { get; set; }
                //public Guid DemandingWatcherId { get; set; }
                //public DateTime Timestamp { get; set; }
                //public string Map { get; set; }
                //public byte MaxPlayers { get; set; }
                //public byte Players { get; set; }
                var sql = "CREATE TABLE IF NOT EXISTS ServerStats ( Id INTEGER PRIMARY KEY AUTOINCREMENT, DemandingWatcherId TEXT NOT NULL, Timestamp TEXT NOT NULL, Map TEXT NULL, MaxPlayers INTEGER NOT NULL, Players INTEGER NOT NULL )";
                conn.Execute(sql);
            }
        }

        public async Task<ServerStat> GetLatestServerStatAsync(Guid watcherId)
        {
            using (var conn = _connProvider.Connect())
            {
                // Select last item order by id where watcher = watcherId
                var sql = "SELECT * FROM ServerStat WHERE DemandingWatcherId = @DemandingWatcherId ORDER BY Id DESC LIMIT 1";
                var result = await conn.QuerySingleAsync<ServerStat>(sql, new { DemandingWatcherId = watcherId });
                return result;
            }
        }

        public async Task<bool> UpsertServerStatAsync(ServerStat serverStat)
        {
            try
            {
                using (var conn = _connProvider.Connect())
                {
                    string sql;
                    if (serverStat.Id != null)
                    {



                        // Check if exists
                        sql = "SELECT COUNT(*) FROM ServerStat WHERE Id = @Id";
                        bool isExists = (await conn.ExecuteScalarAsync<int>(sql, new { Id = serverStat.Id })) == 1;
                        if (isExists)
                        {
                            // Update
                            sql = "UPDATE ServerStats SET DemandingWatcherId = @DemandingWatcherId, Timestamp = @Timestamp, Map = @Map, MaxPlayers = @MaxPlayers, Players = @Players WHERE Id = @Id";
                            var resultUpdate = await conn.ExecuteAsync(sql, serverStat);
                            return resultUpdate > 0;
                        }
                    }

                    // Insert
                    sql = "INSERT INTO ServerStats (DemandingWatcherId, Timestamp, Map, MaxPlayers, Players) VALUES (@DemandingWatcherId, @Timestamp, @Map, @MaxPlayers, @Players)";
                    var resultInsert = await conn.ExecuteAsync(sql, serverStat);
                    return resultInsert > 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
