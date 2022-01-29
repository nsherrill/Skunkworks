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
    public class KuCoinAccessor_SubmitLimitSell
    {
        //[TestMethod]
        public async Task SubmitLimitSell_BaseTest()
        {
            var kucoinAcc = new KuCoinAccessor();
            //var balance = await kucoinAcc.SubmitLimitSell("DASH/USDT", 332.0m, 3.34346527m);
        }
    }
}
