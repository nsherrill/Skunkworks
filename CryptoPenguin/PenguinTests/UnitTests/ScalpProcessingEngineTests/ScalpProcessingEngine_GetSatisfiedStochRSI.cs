using CryptoPenguin.Accessors;
using CryptoPenguin.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PenguinTests.UnitTests.ScalpProcessingEngineTests
{
    [TestClass]
    public class ScalpProcessingEngine_GetSatisfiedStochRSI
    {
        [TestMethod]
        [DataRow(1.0, 10.0, 10.0, 1.0, 10.0, 1.0, 10.0, 1.0, 10.0, 1.0, true)] // start sooner
        [DataRow(1.0, 10.0, 1.0, 10.0, 10.0, 1.0, 10.0, 1.0, 10.0, 1.0, true)] // start later
        [DataRow(1.0, 10.0, 1.0, 10.0, 1.0, 10.0, 10.0, 1.0, 10.0, 1.0, true)] // start event later
        [DataRow(1.0, 10.0, 10.0, 1.0, 10.0, 1.0, 10.0, 1.0, 1.0, 10.0, true)] // confirm last one doesnt matter

        [DataRow(1.0, 10.0, 10.0, 1.0, 10.0, 1.0, 1.0, 10.0, 10.0, 1.0, false)] // flip back
        [DataRow(1.0, 10.0, 10.0, 1.0, 1.0, 10.0, 10.0, 1.0, 10.0, 1.0, false)] // flip back later
        [DataRow(10.0, 1.0, 10.0, 1.0, 10.0, 1.0, 10.0, 1.0, 10.0, 1.0, false)] // dont flip at all
        public void GetSatisfiedStochRSI_BaseTest(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double x5, double y5, bool expectedSuccess)
        {
            var guid = Guid.NewGuid().ToString();
            var eng = new EmperorValidationEngine_VWAPStochRSIPivotPoint();

            var data = new TAApiAccessor.StochRSIDTO()
            {
                Data = new TAApiAccessor.StochRSIDTO.StochRSIBacktrack[]
                {
                    new TAApiAccessor.StochRSIDTO.StochRSIBacktrack(){ valueFastD = x1, valueFastK = y1 },
                    new TAApiAccessor.StochRSIDTO.StochRSIBacktrack(){ valueFastD = x2, valueFastK = y2 },
                    new TAApiAccessor.StochRSIDTO.StochRSIBacktrack(){ valueFastD = x3, valueFastK = y3 },
                    new TAApiAccessor.StochRSIDTO.StochRSIBacktrack(){ valueFastD = x4, valueFastK = y4 },
                    new TAApiAccessor.StochRSIDTO.StochRSIBacktrack(){ valueFastD = x5, valueFastK = y5 },
                }
            };

            var result = eng.GetSatisfiedStochRSI(null, data);
            Assert.AreEqual(expectedSuccess, result);
        }
    }
}
