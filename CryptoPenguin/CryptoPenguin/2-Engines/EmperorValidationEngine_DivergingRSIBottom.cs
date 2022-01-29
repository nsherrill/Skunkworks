using CryptoPenguin.Accessors;
using CryptoPenguin.Shared.BaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Engines
{
    public class EmperorValidationEngine_DivergingRSIBottom : BaseValidationService
    {
        public EmperorValidationResult ProcessData(TAApiAccessor.CandlesDTO candlesData, TAApiAccessor.MomentumRSIDTO momentumRSIData, decimal currentPrice)
        {
            if (!(candlesData?.Data?.Any() ?? false)
                || !(momentumRSIData?.Data?.Any() ?? false)
                || currentPrice <= 0m)
            {
                return new EmperorValidationResult();
            }

            FindRecentLowPoints(candlesData, out int firstLowBacktrackIndex, out int secondLowBacktrackIndex);

            bool satisfiedBullishEngulfingCandle = GetBullishEngulfingCandle(candlesData, currentPrice);
            bool satisfiedIncreasedVolumeAtFirstBottom = GetSatisfiedIncreasedVolume(candlesData, firstLowBacktrackIndex);
            bool satisfiedIncreasedVolumeAtLastBottom = GetSatisfiedIncreasedVolume(candlesData, secondLowBacktrackIndex);
            bool satisfiedSupplyAbsorbingCandle = GetSatisfiedSupplyAbsorbingCandle(momentumRSIData);

            bool satisfiedLowerLowPrice = GetSatisfiedLowerLowPrice(momentumRSIData, firstLowBacktrackIndex, secondLowBacktrackIndex);
            bool satisfiedHigherLowRSI = GetSatisfiedHigherLowRSI(momentumRSIData, firstLowBacktrackIndex, secondLowBacktrackIndex);

            return new EmperorValidationResult()
            {
                SatisfiedBullishEngulfingCandle = satisfiedBullishEngulfingCandle,
                SatisfiedHigherLowRSI = satisfiedHigherLowRSI,
                SatisfiedIncreasedVolumeAtFirstBottom = satisfiedIncreasedVolumeAtFirstBottom,
                SatisfiedIncreasedVolumeAtLastBottom = satisfiedIncreasedVolumeAtLastBottom,
                SatisfiedLowerLowPrice = satisfiedLowerLowPrice,
                SatisfiedSupplyAbsorbingCandle = satisfiedSupplyAbsorbingCandle,

                ShouldDoIt = satisfiedBullishEngulfingCandle
                    && satisfiedHigherLowRSI
                    && satisfiedIncreasedVolumeAtFirstBottom
                    && satisfiedIncreasedVolumeAtLastBottom
                    && satisfiedLowerLowPrice
                    && satisfiedSupplyAbsorbingCandle,
                ConfidenceLevel =
                    (int)Math.Round((double)(
                        (satisfiedBullishEngulfingCandle ? 1 : 0)
                        + (satisfiedHigherLowRSI ? 1 : 0)
                        + (satisfiedIncreasedVolumeAtFirstBottom ? 1 : 0)
                        + (satisfiedIncreasedVolumeAtLastBottom ? 1 : 0)
                        + (satisfiedLowerLowPrice ? 1 : 0)
                        + (satisfiedSupplyAbsorbingCandle ? 1 : 0))
                    / 6.0)
            };
        }

        private bool GetSatisfiedSupplyAbsorbingCandle(TAApiAccessor.MomentumRSIDTO momentumRSIData)
        {
            throw new NotImplementedException();
        }

        private bool GetSatisfiedHigherLowRSI(TAApiAccessor.MomentumRSIDTO momentumRSIData, int firstLowBacktrackIndex, int secondLowBacktrackIndex)
        {
            throw new NotImplementedException();
        }

        private bool GetSatisfiedLowerLowPrice(TAApiAccessor.MomentumRSIDTO momentumRSIData, int firstLowBacktrackIndex, int secondLowBacktrackIndex)
        {
            throw new NotImplementedException();
        }

        private bool GetSatisfiedIncreasedVolume(TAApiAccessor.CandlesDTO candlesData, int backtrackIndex)
        {
            throw new NotImplementedException();
        }

        private bool GetBullishEngulfingCandle(TAApiAccessor.CandlesDTO candlesData, decimal currentPrice)
        {
            if (candlesData.Data.Length > 0)
            {
                return currentPrice > (decimal)(candlesData.Data[candlesData.Data.Length - 1].close)
                    && currentPrice > (decimal)(candlesData.Data[candlesData.Data.Length - 2].close);
            }

            return false;
        }

        private void FindRecentLowPoints(TAApiAccessor.CandlesDTO candlesData, out int firstLowBacktrackIndex, out int secondLowBacktrackIndex)
        {
            throw new NotImplementedException();
        }

        public class EmperorValidationResult
        {
            public int ConfidenceLevel { get; set; }
            public bool ShouldDoIt { get; set; }
            public string Notes { get; set; }

            public bool SatisfiedHigherLowRSI { get; set; }
            public bool SatisfiedLowerLowPrice { get; set; }
            public bool SatisfiedIncreasedVolumeAtFirstBottom { get; set; }
            public bool SatisfiedIncreasedVolumeAtLastBottom { get; set; }
            public bool SatisfiedSupplyAbsorbingCandle { get; set; }
            public bool SatisfiedBullishEngulfingCandle { get; set; }
        }
    }
}
