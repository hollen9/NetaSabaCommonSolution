using Microsoft.Extensions.Options;
using NetaSabaPortal.Models.Entities;
using NetaSabaPortal.Options;
using NetaSabaPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using System.Data;
using DapperAid;
using NetaSabaPortal.Extensions.DapperAidJp;
//using Dapperer;

namespace NetaSabaPortal.Repositories
{
    public class WatcherRepository
    {
        private readonly IConnectionProvider _connProvider;
        private readonly QueryBuilder _queryBuilder;
        
        public WatcherRepository(IConnectionProvider connProvider, QueryBuilder queryBuilderInstance)
        {
            _connProvider = connProvider;
            _queryBuilder = queryBuilderInstance;

            // 注意! AddSingleton であることを前提としているので、ここでテーブル作成を行う
            CheckAndCreateTables();
        }

        private void CheckAndCreateTables()
        {
            using (var conn = _connProvider.Connect())
            {
                // conn.UseDapperAid(_queryBuilder);
                conn.CreateTableIfNotExists<ServerStat>(_queryBuilder).GetAwaiter().GetResult();
            }
            //using (var conn = _connProvider.Connect())
            //{
            //    var sql = "CREATE TABLE IF NOT EXISTS ServerStats ( Id INTEGER PRIMARY KEY AUTOINCREMENT, DemandingWatcherId TEXT NOT NULL, SessionId TEXT NOT NULL, Timestamp TEXT NOT NULL, Map TEXT NULL, MaxPlayers INTEGER NOT NULL, Players INTEGER NOT NULL )";
            //    conn.Execute(sql);
            //}
        }

        public async Task<ServerStat> GetLatestServerStatAsync(Guid watcherId, Guid sessionId)
        {
            using (var conn = _connProvider.Connect())
            {
                // Select last item order by id where watcher = watcherId
                // var sql = "SELECT * FROM ServerStat WHERE DemandingWatcherId = @DemandingWatcherId AND SessionId = @SessionId ORDER BY Id DESC LIMIT 1";
                // var result = await conn.QuerySingleAsync<ServerStat>(sql, new { DemandingWatcherId = watcherId, SessionId = sessionId });
                conn.UseDapperAid(_queryBuilder);
                var result = await conn.SelectFirstOrDefaultAsync<ServerStat>( x=> x.DemandingWatcherId == watcherId && x.SessionId == sessionId);
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
                    // Check if exists
                    sql = "SELECT COUNT(*) FROM ServerStats WHERE Id = @Id";
                    bool isExists = (await conn.ExecuteScalarAsync<int>(sql, new { Id = serverStat.Id })) == 1;
                    if (isExists)
                    {
                        // Update
                        sql = "UPDATE ServerStats SET SessionId = @SessionId, DemandingWatcherId = @DemandingWatcherId, Timestamp = @Timestamp, Map = @Map, MaxPlayers = @MaxPlayers, Players = @Players WHERE Id = @Id";
                        var resultUpdate = await conn.ExecuteAsync(sql, serverStat);
                        return resultUpdate > 0;
                    }

                    // Insert
                    sql = "INSERT INTO ServerStats (SessionId, DemandingWatcherId, Timestamp, Map, MaxPlayers, Players) VALUES (@SessionId, @DemandingWatcherId, @Timestamp, @Map, @MaxPlayers, @Players)";
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
