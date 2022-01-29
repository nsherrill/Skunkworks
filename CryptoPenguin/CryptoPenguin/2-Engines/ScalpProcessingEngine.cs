using CryptoPenguin.Accessors;
using CryptoPenguin.Shared;
using CryptoPenguin.Shared.BaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Engines
{
    public class ScalpProcessingEngine : BaseService
    {
        public ScalpProcessingResult CalculateScalp(decimal currentBankroll, double maxBankrollInvestmentPercentPerScalp, double targetScalpPercent,
            decimal targetScalpDollarIncrement, decimal symbolPrice, int decimalPlaces)
        {
            // i've got $7,000 in the bank, and want a max-scalp at 0.10 percent ($700). with a scalp percent of (0.0127) i'd like each scalp to target $5. 
            // current XRP is worth $1.50. buy @ $1.50, sell at $1.48095 and sell at $1.51905
            // buy 262.467 to make $5
            // investment: 262.467 * $1.50 = 393.70
            // stoploss:  262.467 * $1.48095 = 388.70
            // limit sell:  262.467 * $1.51905 = 398.70

            var targetQuantity = targetScalpDollarIncrement / (symbolPrice * (decimal)targetScalpPercent);
            var targetInvestment = Math.Min(targetQuantity * symbolPrice, currentBankroll * (decimal)maxBankrollInvestmentPercentPerScalp);
            targetInvestment = decimal.Round(targetInvestment, decimalPlaces, MidpointRounding.AwayFromZero);

            var result = new ScalpProcessingResult()
            {
                TotalInvestment = targetInvestment,
                Quantity = decimal.Round(targetInvestment / symbolPrice, decimalPlaces, MidpointRounding.AwayFromZero),
                LimitBuyPrice = symbolPrice,
                LimitSellPrice = decimal.Round(symbolPrice + (symbolPrice * (decimal)targetScalpPercent), decimalPlaces, MidpointRounding.AwayFromZero),
                StopLossPrice = decimal.Round(symbolPrice - (symbolPrice * (decimal)targetScalpPercent), decimalPlaces, MidpointRounding.AwayFromZero),
            };

            return result;
        }

        public class ScalpProcessingResult
        {
            public decimal TotalInvestment { get; set; }
            public decimal Quantity { get; set; }
            public decimal LimitBuyPrice { get; set; }
            public decimal StopLossPrice { get; set; }
            public decimal LimitSellPrice { get; set; }
        }
    }
}
