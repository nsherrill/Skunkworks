using GP.Shared.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Accessors.DatabaseAccessor
{
    public interface IDatabaseAcc
    {
        bool ExecuteNonQuery(string sql, ValuePair[] paramArr);

        List<List<object>> ExecuteQuery(string sql, ValuePair[] paramArr, GP.Accessors.DatabaseAccessor.DatabaseAcc.ReaderDelegate reader);
    }

    public class DatabaseAcc : IDatabaseAcc
    {
        public delegate object ReaderDelegate(SqlDataReader rdr, int index);

        public bool ExecuteNonQuery(string sql, ValuePair[] paramArr)
        {
            using (SqlConnection conn = new SqlConnection(ConfigHelper.DBConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    if (paramArr != null
                        && paramArr.Length > 0)
                        foreach (var param in paramArr)
                            cmd.Parameters.AddWithValue(param.Key, param.Value);

                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public List<List<object>> ExecuteQuery(string sql, ValuePair[] paramArr, DatabaseAcc.ReaderDelegate reader)
        {
            using (SqlConnection conn = new SqlConnection(ConfigHelper.DBConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 300;

                    if (paramArr != null
                        && paramArr.Length > 0)
                        foreach (var param in paramArr)
                            cmd.Parameters.AddWithValue(param.Key, param.Value);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        List<List<object>> result = new List<List<object>>();
                        int index = 0;
                        List<object> curr = new List<object>();
                        while (rdr.Read())
                        {
                            curr.Add(reader(rdr, index));
                        }
                        result.Add(curr);
                        while (rdr.NextResult())
                        {
                            List<object> curr2 = new List<object>();
                            index++;
                            while (rdr.Read())
                            {
                                curr2.Add(reader(rdr, index));
                            }
                            result.Add(curr2);
                        }

                        return result;
                    }
                }
            }
        }
    }
}
