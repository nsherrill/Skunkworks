using CryptoPenguin.Accessors;
using CryptoPenguin.Shared.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenguinTests.IntegrationTests.KuCoinAccessorTests
{
    [TestClass]
    public class KuCoinAccessor_GetAccountBalance
    {
        [TestMethod]
        public async Task GetAccountBalance_BaseTest()
        {
            var kucoinAcc = new KuCoinAccessor();
            var balance = await kucoinAcc.GetAccountBalance();
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
