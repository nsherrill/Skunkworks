using CryptoPenguin.Accessors;
using CryptoPenguin.Shared.BaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Engines
{
    public class EmperorValidationEngine_VWAPStochRSIPivotPoint : BaseValidationService
    {
        public EmperorValidationResult ProcessData(TAApiAccessor.CandlesDTO candlesData, TAApiAccessor.VWAPDTO vwapData,
            TAApiAccessor.StochRSIDTO stochRSIData, FCSApiAccessor.PivotPointsDTO pivotPointsData, decimal currentPrice)
        {
            if (!(candlesData?.Data?.Any() ?? false)
                || !(vwapData?.Data?.Any() ?? false)
                || !(stochRSIData?.Data?.Any() ?? false)
                || currentPrice <= 0m)
            {
                return new EmperorValidationResult();
            }

            bool satisfiedVWAP = GetSatisfiedVWAP(candlesData, vwapData);
            bool satisfiedStochRSI = GetSatisfiedStochRSI(candlesData, stochRSIData);
            bool statisfiedPivotPoints = GetSatisfiedPivotPoints(candlesData, pivotPointsData);
            bool statisfiedNextCandle = GetSatisfiedNextCandle(candlesData, currentPrice);

            return new EmperorValidationResult()
            {
                SatisfiedStochRSI = satisfiedStochRSI,
                SatisfiedVWAP = satisfiedVWAP,
                SatisfiedPivotPoints = statisfiedPivotPoints,
                SatisfiedNextCandle = statisfiedNextCandle,

                ShouldDoIt = satisfiedStochRSI && satisfiedVWAP && statisfiedPivotPoints && statisfiedNextCandle,
                ConfidenceLevel =
                    (satisfiedStochRSI ? 25 : 0)
                    + (satisfiedVWAP ? 25 : 0)
                    + (statisfiedPivotPoints ? 25 : 0)
                    + (statisfiedNextCandle ? 25 : 0),
            };
        }

        public bool GetSatisfiedPivotPoints(TAApiAccessor.CandlesDTO candlesData, FCSApiAccessor.PivotPointsDTO pivotPointsData)
        {
            if (pivotPointsData?.Data?.response?.pivot_point?.fibonacci != null
                && (candlesData?.Data.Any() ?? false))
            {
                List<double> items = new List<double>();
                if (double.TryParse(pivotPointsData.Data.response.pivot_point.fibonacci.R1, out var val)) items.Add(val);
                if (double.TryParse(pivotPointsData.Data.response.pivot_point.fibonacci.R2, out val)) items.Add(val);
                if (double.TryParse(pivotPointsData.Data.response.pivot_point.fibonacci.R3, out val)) items.Add(val);
                if (double.TryParse(pivotPointsData.Data.response.pivot_point.fibonacci.S1, out val)) items.Add(val);
                if (double.TryParse(pivotPointsData.Data.response.pivot_point.fibonacci.S2, out val)) items.Add(val);
                if (double.TryParse(pivotPointsData.Data.response.pivot_point.fibonacci.S3, out val)) items.Add(val);

                var latestCandle = candlesData.Data[candlesData.Data.Length - 1];
                var secondLatestCandle = candlesData.Data[candlesData.Data.Length - 2];

                var testCandleLow = Math.Min(secondLatestCandle.low, latestCandle.low);
                var testCandleHigh = latestCandle.close;
                if (items.Any(i => i >= testCandleLow && i <= testCandleHigh))
                    return true;
            }

            return false;
        }

        public bool GetSatisfiedStochRSI(TAApiAccessor.CandlesDTO candlesData, TAApiAccessor.StochRSIDTO stochRSIData)
        {
            var cndleCount = 3;
            var startIndex = stochRSIData.Data.Length - cndleCount;

            bool startedBearish = stochRSIData.Data[startIndex].IsBearish();
            bool hasFlippedToBullish = false;

            if (!startedBearish) return false;

            for (int i = 0; i < cndleCount; i++)
            {
                if (hasFlippedToBullish && stochRSIData.Data[startIndex + i].IsBearish())
                    return false;

                hasFlippedToBullish = stochRSIData.Data[i].IsBullish();
            }

            return hasFlippedToBullish;
        }

        public bool GetSatisfiedVWAP(TAApiAccessor.CandlesDTO candlesData, TAApiAccessor.VWAPDTO vwapData)
        {
            bool satisfiedVWAP = false;

            if (candlesData.Data.Length > 4
                && vwapData.Data.Length > 4)
            {
                if (candlesData.Data[candlesData.Data.Length - 2].close < vwapData.Data[vwapData.Data.Length - 2].value
                    && candlesData.Data[candlesData.Data.Length - 3].close < vwapData.Data[vwapData.Data.Length - 3].value
                    && candlesData.Data[candlesData.Data.Length - 4].close < vwapData.Data[vwapData.Data.Length - 4].value)
                {
                    satisfiedVWAP = candlesData.Data.Last().close > vwapData.Data.Last().value;
                }
            }


            return satisfiedVWAP;
        }

        public bool GetSatisfiedNextCandle(TAApiAccessor.CandlesDTO candlesData, decimal currentPrice)
        {
            if (candlesData.Data.Length > 0)
            {
                return currentPrice > (decimal)(candlesData.Data[candlesData.Data.Length - 1].close);
            }

            return false;
        }

        public class EmperorValidationResult
        {
            public int ConfidenceLevel { get; set; }
            public bool ShouldDoIt { get; set; }
            public string Notes { get; set; }

            public bool SatisfiedStochRSI { get; set; }
            public bool SatisfiedVWAP { get; set; }
            public bool SatisfiedPivotPoints { get; set; }
            public bool SatisfiedNextCandle { get; set; }
        }
    }
}
