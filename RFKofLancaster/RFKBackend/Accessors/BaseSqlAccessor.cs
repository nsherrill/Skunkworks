using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFKBackend.Shared;

namespace RFKBackend.Accessors
{
    public abstract class BaseSqlAccessor
    {
        const string UPDATE_FORMATTER = "update {0} set {1} = '{2}' where {3} = '{4}' {5}";
        internal abstract string TABLE_NAME { get; }
        internal abstract string PRIMARY_KEY_NAME { get; }
        private string GetBracketlessPrimaryKey()
        {
            return this.PRIMARY_KEY_NAME.Replace("[", "").Replace("]", "");
        }

        public T[] ExecuteReader<T>(string sqlString, Func<SqlDataReader, T> handler, SqlParameter[] paramList = null)
        {
            List<T> result = new List<T>();

            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;

                    if (paramList != null)
                        cmd.Parameters.AddRange(paramList);

                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        result.Add(handler(rdr));
                    }
                }
            }

            return result.ToArray();
        }

        public object ExecuteScalar(string sqlString, SqlParameter[] paramList = null)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;

                    if (paramList != null)
                        cmd.Parameters.AddRange(paramList);

                    var result = cmd.ExecuteScalar();
                    return result;
                }
            }
        }

        #region getbasic
        internal string GetBasicFindSql(string idParam, string overrideTableName = null, string overrideKeyName = null)
        {
            var tableToUse = string.IsNullOrEmpty(overrideTableName) ? this.TABLE_NAME : overrideTableName;
            var keyToUse = string.IsNullOrEmpty(overrideKeyName) ? this.PRIMARY_KEY_NAME : overrideKeyName;

            string sqlString = $"SELECT * FROM {tableToUse} WHERE {keyToUse} = {idParam}";
            return sqlString;
        }

        public string GetBasicFindAllSql(string orderByClause = null)
        {
            string sqlString = $@"select * from {TABLE_NAME}";
            if (!string.IsNullOrEmpty(orderByClause))
            {
                orderByClause = orderByClause.ToLower().Replace("order by", "");
                sqlString += $" order by {orderByClause}";
            }
            return sqlString;
        }

        public string GetBasicMergeText(SqlParameter[] paramList, string additionalWhereClause = null, string overrideTableName = null)
        {
            string mergeText = $@"
                merge into {(string.IsNullOrEmpty(overrideTableName) ? TABLE_NAME : overrideTableName)} as target
                    using (select 
                            {string.Join(", ", paramList.Select(p => $"{p.ParameterName} as {p.GetParameterNameWithoutAt()}"))}
                        ) as source
                    on (target.{PRIMARY_KEY_NAME} = source.{PRIMARY_KEY_NAME} 
                        {additionalWhereClause}
                        and source.{PRIMARY_KEY_NAME} > 0)
                    when matched then 
                        update set 
                            {string.Join(", ", paramList.Where(p => !p.GetParameterNameWithoutAt().Equals(GetBracketlessPrimaryKey(), StringComparison.InvariantCultureIgnoreCase)).Select(p => $"{p.GetParameterNameWithoutAt()} = source.{p.GetParameterNameWithoutAt()}"))}
                    when not matched then 
                        insert ({string.Join(", ", paramList.Select(p => p.GetParameterNameWithoutAt()).Where(p => !p.Equals(GetBracketlessPrimaryKey(), StringComparison.InvariantCultureIgnoreCase)))} )
                        values ({string.Join(", ", paramList.Select(p => p.GetParameterNameWithoutAt()).Where(p => !p.Equals(GetBracketlessPrimaryKey(), StringComparison.InvariantCultureIgnoreCase)).Select(p => $" source.{p}"))} );";

            return mergeText;
        }
        #endregion

        public string[] GetHeaders(SqlDataReader rdr)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                result.Add(rdr.GetName(i));
            }

            return result.ToArray();
        }

        public string[] GetValues(SqlDataReader rdr)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                if (rdr.GetFieldType(i) == typeof(bool) && !rdr.IsDBNull(i))
                {
                    result.Add((rdr.GetBoolean(i) ? 1 : 0).ToString());
                }
                else
                {
                    var val = rdr.GetValue(i);
                    string myVal = (val ?? "").ToString();
                    result.Add(myVal);
                }
            }

            return result.ToArray();
        }

        internal void ToggleBool(string boolColumnName, int id, bool shouldBeOn, string additionalWhereClause = null, string overrideTableName = null)
        {
            string sql = string.Format(UPDATE_FORMATTER
                , string.IsNullOrEmpty(overrideTableName) ? TABLE_NAME : overrideTableName
                , boolColumnName
                , shouldBeOn.ToString()
                , PRIMARY_KEY_NAME
                , id
                , additionalWhereClause);
            this.ExecuteScalar(sql);
        }

        #region generic readers
        internal int IntReader(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }
        #endregion
    }
}
