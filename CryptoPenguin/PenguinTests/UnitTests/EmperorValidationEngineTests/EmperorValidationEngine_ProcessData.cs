using CryptoPenguin.Accessors;
using CryptoPenguin.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PenguinTests.UnitTests.ScalpProcessingEngineTests
{
    [TestClass]
    public class EmperorValidationEngine_ProcessData
    {
        /*
            // i've got $7,000 in the bank, and want a max-scalp at 0.10 percent ($700). with a scalp percent of (0.0127) i'd like each scalp to target $5. 
            // current XRP is worth $1.50. buy @ $1.50, sell at $1.48095 and sell at $1.51905
            // buy 262.467 to make $5
            // investment: 262.467 * $1.50 = 393.70
            // stoploss:  262.467 * $1.48095 = 388.70
            // limit sell:  262.467 * $1.51905 = 398.70
        }*/

        [TestMethod]
        [DataRow(10000, .1, 0.0127, 5.0, 1.5, 5, 393.70079, 262.46719, 1.5, 1.48095, 1.51905)]
        [DataRow(100, .1, 0.0127, 5.0, 1.5, 5, 10.0, 6.66667, 1.5, 1.48095, 1.51905)]

        [DataRow(10000, .1, 0.0127, 5.0, 1.5, 4, 393.7008, 262.4672, 1.5, 1.481, 1.5191)]
        [DataRow(100, .1, 0.0127, 5.0, 1.5, 4, 10.0, 6.6667, 1.5, 1.481, 1.5191)]

        [DataRow(10000, .1, 0.0127, 5.0, 0.5, 5, 393.70079, 787.40158, 0.5, 0.49365, 0.50635)]
        [DataRow(10000, .1, 0.0127, 5.0, 1.5, 5, 393.70079, 262.46719, 1.5, 1.48095, 1.51905)]
        [DataRow(10000, .1, 0.0127, 5.0, 15.0, 5, 393.70079, 26.24672, 15.0, 14.8095, 15.1905)]
        [DataRow(10000, .1, 0.0127, 5.0, 1500.0, 5, 393.70079, 0.26247, 1500, 1480.95, 1519.05)]

        [DataRow(10000, .1, 0.0127, 2.0, 0.5, 5, 157.48031, 314.96062, 0.5, 0.49365, 0.50635)]
        [DataRow(10000, .1, 0.0127, 2.0, 1.5, 5, 157.48031, 104.98687, 1.5, 1.48095, 1.51905)]
        [DataRow(10000, .1, 0.0127, 2.0, 15.0, 5, 157.48031, 10.49869, 15.0, 14.8095, 15.1905)]
        [DataRow(10000, .1, 0.0127, 2.0, 1500.0, 5, 157.48031, 0.10499, 1500, 1480.95, 1519.05)]
        public void ProcessData_BaseTest(double currentBankroll, double maxBankrollInvestmentPercentPerScalp, double targetScalpPercent, double targetScalpDollarIncrement, 
            double symbolPrice, int decimalPlaces,
            double expectedTotalInvestment, double expectedQuantity, double expectedBuyPrice, double expectedStopLossPrice, double expectedSellPrice)
        {
            var eng = new ScalpProcessingEngine();

            var result = eng.CalculateScalp((decimal)currentBankroll, maxBankrollInvestmentPercentPerScalp, targetScalpPercent, (decimal)targetScalpDollarIncrement, (decimal)symbolPrice, decimalPlaces);

            Assert.IsNotNull(result);
            Assert.AreEqual((decimal)expectedTotalInvestment, result.TotalInvestment);
            Assert.AreEqual((decimal)expectedQuantity, result.Quantity);
            Assert.AreEqual((decimal)expectedBuyPrice, result.LimitBuyPrice);
            Assert.AreEqual((decimal)expectedStopLossPrice, result.StopLossPrice);
            Assert.AreEqual((decimal)expectedSellPrice, result.LimitSellPrice);
        }
    }
}
