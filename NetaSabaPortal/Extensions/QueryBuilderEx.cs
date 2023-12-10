using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetaSabaPortal.Extensions
{
    public class QueryBuilderEx : DapperAid.QueryBuilder
    {
        /// <summary>
        /// 引数で指定された値をSQLリテラル値表記へと変換します。
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>SQLリテラル値表記</returns>
        public new string ToSqlLiteral(object? value)
        {
            if (IsNull(value)) { return "null"; }
            foreach (var c in _literalConverters)
            {
                if (c.Key.IsAssignableFrom(value.GetType())) { return c.Value(value); }
            }
            if (value is string s) { return ToSqlLiteral(s); }
            if (value is Guid g) { return ToSqlLiteral(g.ToString()); }
            if (value is DateTime dt) { return ToSqlLiteral(dt); }
            if (value is bool b) { return (b ? TrueLiteral : FalseLiteral); }
            if (value is Enum e) { return e.ToString("d"); }
            if (value is byte[] blob) { return ToSqlLiteral(blob); }
            if (value is System.Collections.IEnumerable ienumerable && value.GetType().GetInterfaces().Any(t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(ICollection<>)))
            {
                var sb = new StringBuilder();
                sb.Append("ARRAY[");
                var delimiter = "";
                foreach (var v in ienumerable)
                {
                    sb.Append(delimiter).Append(ToSqlLiteral(v));
                    delimiter = ",";
                }
                sb.Append("]");
                return sb.ToString();
            }
            return value.ToString() ?? throw new ArgumentException($"The value of type {value.GetType().FullName} cannot be represented as a sql literal.");
        }
    }
}
