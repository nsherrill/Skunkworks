using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Accessors
{
    public class BaseSqlAccessor
    {
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
                var val = rdr.GetValue(i);
                string myVal = (val ?? "").ToString();
                result.Add(myVal);
            }

            return result.ToArray();
        }

        #region generic readers
        internal int IntReader(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }
        #endregion
    }
}
