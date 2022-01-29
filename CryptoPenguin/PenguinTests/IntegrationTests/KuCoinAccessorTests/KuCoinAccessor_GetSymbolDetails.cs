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
    public class KuCoinAccessor_GetSymbolDetails
    {
        [TestMethod]
        public async Task GetSymbolDetails_BaseTest()
        {
            var acc = new KuCoinAccessor();
            var details = await acc.GetSymbolDetails("XRP/USDT");
            Assert.AreEqual("XRP-USDT", details.Name);
            Assert.AreEqual(5, details.PriceIncrementDecimalPlaces);
        }

        [TestMethod]
        public void SymbolDetail_ConfirmDecimalPlacesMapping()
        {
            var obj = new SymbolDetail()
            {
                Name = "XRP",
                Price = 1.05m,
                PriceIncrement = .0001m,
            };
            Assert.AreEqual(4, obj.PriceIncrementDecimalPlaces);
        }
    }
}
