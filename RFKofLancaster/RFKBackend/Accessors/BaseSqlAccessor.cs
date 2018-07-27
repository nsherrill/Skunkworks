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

        #region generic readers
        internal int IntReader(SqlDataReader rdr)
        {
            return rdr.GetInt32(0);
        }
        #endregion
    }
}
