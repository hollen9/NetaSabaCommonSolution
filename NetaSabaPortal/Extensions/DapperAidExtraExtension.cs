using Dapper;
using DapperAid;
using DapperAid.Ddl;
using Discord;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NetaSabaPortal.Extensions.DapperAidJp
{
    public static class DapperAidExtraExtension
    {
        public static string GenerateCheckTableSQL<T>(QueryBuilder? queryBuilder = null) where T : notnull
        {
            if (queryBuilder is not DapperAid.QueryBuilder.SQLite)
            {
                throw new NotSupportedException("Only Sqlite is supported atm");
            }

            DapperAid.Helpers.TableInfo tableInfo = (queryBuilder ?? QueryBuilder.DefaultInstance).GetTableInfo<T>();
            StringBuilder stringBuilder = new StringBuilder();
            // Check if table exists

            stringBuilder.Append($"SELECT EXISTS(SELECT name FROM sqlite_schema WHERE type = 'table' AND name = '{tableInfo.Name}');");
            return stringBuilder.ToString();
        }

        public static async Task CreateTableIfNotExists<T>(this IDbConnection conn, QueryBuilder qB) where T : notnull
        {
            string sql = GenerateCheckTableSQL<T>(qB);
            bool isExists = await conn.QuerySingleAsync<bool>(sql);
            if (isExists)
            {
                return;
            }
            sql = DapperAid.Ddl.DDLAttribute.GenerateCreateSQL<T>(qB);
            await conn.ExecuteAsync(sql);
        }
    }
}
