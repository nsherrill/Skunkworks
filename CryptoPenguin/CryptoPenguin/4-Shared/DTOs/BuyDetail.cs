using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared.DTOs
{
    public class BuyDetail
    {
        public string Symbol { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal StopLossPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalInvestment { get; set; }
        public string BuyOrderId { get; set; }
        public string SellOrderId { get; set; }
        public string StrategyName { get; set; }

        public BuyDetail()
        {

        }

        public BuyDetail(string symbol, Engines.ScalpProcessingEngine.ScalpProcessingResult buyInstruction, string stratName) : base()
        {
            this.Symbol = symbol;
            this.Quantity = buyInstruction.Quantity;
            this.SellPrice = buyInstruction.LimitSellPrice;
            this.BuyPrice = buyInstruction.LimitBuyPrice;
            this.StopLossPrice = buyInstruction.StopLossPrice;
            this.TotalInvestment = buyInstruction.TotalInvestment;
            this.StrategyName = stratName;
        }
    }
}
