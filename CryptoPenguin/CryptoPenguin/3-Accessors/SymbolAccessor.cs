using CryptoPenguin.Shared.BaseObjects;
using CryptoPenguin.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace CryptoPenguin.Accessors
{
    public class SymbolAccessor : BaseService
    {
        public SymbolAccessor()
        {
        }

        public IEnumerable<SymbolModel> GetAllActiveSymbols()
        {
            var result = new List<SymbolModel>();
            try
            {
                var sqlStr = $"select [Symbol],[IsActive],[Id] from ep.symbols where isactive = 1 order by symbol";
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CryptoBots"].ConnectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlStr;

                        var rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            result.Add(new SymbolModel()
                            {
                                Symbol = (string)rdr["Symbol"],
                                IsActive = (bool)rdr["IsActive"],
                                Id = (long)rdr["Id"],
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log(e, $"SymbolAccessor.GetAllActiveSymbols()");
            }
            return result.ToArray();
        }

        public void SetIsActive(string symbol, bool isActive)
        {
            try
            {
                Log($"** Disabling {symbol} **");

                var isActiveStr = isActive ? "1" : "0";
                var sqlStr = $"update ep.symbols set isactive = {isActiveStr} where symbol = '{symbol}'";

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CryptoBots"].ConnectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlStr;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Log(e, $"SymbolAccessor.SetIsactive({symbol}, {isActive})");
            }
        }
    }
}
