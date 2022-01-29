using CryptoPenguin.Shared;
using CryptoPenguin.Shared.BaseObjects;
using CryptoPenguin.Shared.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Accessors
{
    public class PreviousBuysAccessor : BaseService
    {
        public PreviousBuysAccessor()
        {
        }

        public IEnumerable<BuyDetail> GetAllOpenOrders()
        {
            var result = new List<BuyDetail>();
            try
            {
                var sqlStr = $"select [Symbol],[BuyPrice],[StopLossPrice],[SellPrice],[Quantity],[TotalInvestment],[CreateDate],[BuyOrderId],[SellOrderId] from ep.buylog where success is null and updatedate is null";
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CryptoBots"].ConnectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlStr;

                        var rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            result.Add(new BuyDetail()
                            {
                                Symbol = (string)rdr["Symbol"],
                                BuyPrice = (decimal)rdr["BuyPrice"],
                                StopLossPrice = (decimal)rdr["StopLossPrice"],
                                SellPrice = (decimal)rdr["SellPrice"],
                                Quantity = (decimal)rdr["Quantity"],
                                TotalInvestment = (decimal)rdr["TotalInvestment"],
                                BuyOrderId = (string)rdr["BuyOrderId"],
                                SellOrderId = (string)rdr["SellOrderId"],
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log(e, $"PreviousBuysAccessor.GetAllOpenOrders()");
            }
            return result.ToArray();
        }
    }
}
