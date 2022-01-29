using CryptoPenguin.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared.BaseObjects
{
    public abstract class BaseService
    {
        public void Log(Exception e, string message)
        {
            Console.WriteLine($"{DateTime.Now} - {message} - {e.ToString()}");
        }

        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now} - {message}");
        }

        public void LogBuy(BuyDetail buyObj)
        {
            var sqlStr = @"insert into ep.BuyLog (Symbol, BuyPrice, StopLossPrice, SellPrice, Quantity, TotalInvestment, BuyOrderId, SellOrderId, StrategyName)
                values (@symbol, @buyPrice, @stopLossPrice, @sellPrice, @quantity, @totalInvestment, @buyOrderId, @sellOrderId, @strategyName)";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CryptoBots"].ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlStr;
                    cmd.Parameters.AddWithValue("@symbol", buyObj.Symbol);
                    cmd.Parameters.AddWithValue("@buyPrice", buyObj.BuyPrice);
                    cmd.Parameters.AddWithValue("@stopLossPrice", buyObj.StopLossPrice);
                    cmd.Parameters.AddWithValue("@sellPrice", buyObj.SellPrice);
                    cmd.Parameters.AddWithValue("@quantity", buyObj.Quantity);
                    cmd.Parameters.AddWithValue("@totalInvestment", buyObj.TotalInvestment);
                    cmd.Parameters.AddWithValue("@buyOrderId", buyObj.BuyOrderId);
                    cmd.Parameters.AddWithValue("@sellOrderId", buyObj.SellOrderId);
                    cmd.Parameters.AddWithValue("@strategyName", buyObj.StrategyName);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
