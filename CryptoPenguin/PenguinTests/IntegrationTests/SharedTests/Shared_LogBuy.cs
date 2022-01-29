using CryptoPenguin.Accessors;
using CryptoPenguin.Managers;
using CryptoPenguin.Shared.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenguinTests.IntegrationTests.SharedTests
{
    [TestClass]
    public class Shared_LogBuy
    {
        [TestMethod]
        public async Task LogBuy_BaseTest()
        {
            //[DataRow(10000, .1, 0.0127, 5.0, 1.5, 5, 393.70079, 262.46719, 1.5, 1.48095, 1.51905)]
            var mgr = new EmperorPenguinManager();
            mgr.LogBuy(new BuyDetail()
            {
                Symbol = "TEST SYMBOL",
                BuyPrice = 1.50m,
                StopLossPrice = 1.48095m,
                SellPrice = 1.51905m,
                Quantity = 262.46719m,
                TotalInvestment = 393.70079m,
                SellOrderId = Guid.NewGuid().ToString(),
                BuyOrderId = Guid.NewGuid().ToString(),
            });
        }

        //[TestMethod]
        public async Task SubmitLimitBuy_BaseTest()
        {
            var kucoinAcc = new KuCoinAccessor();
            //var balance = await kucoinAcc.SubmitLimitBuy("DASH/USDT", 332.0m, 3.34346527m);
        }
        //6090028a5b31e60006060801


    }
}
