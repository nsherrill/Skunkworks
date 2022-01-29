using CryptoPenguin.Accessors;
using CryptoPenguin.Engines;
using CryptoPenguin.Shared;
using CryptoPenguin.Shared.BaseObjects;
using CryptoPenguin.Shared.DTOs;
using CryptoPenguin.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoPenguin.Managers
{
    public class EmperorPenguinManager : BaseService, IPenguinManager
    {
        public async Task<ServiceResult<bool>> Start(string[] args = null)
        {
            var result = new ServiceResult<bool>();

            try
            {
                //await CleanupPreviousOrders();

                if (!(args?.Any() ?? false))
                {
                    var symbolAcc = new SymbolAccessor();
                    args = symbolAcc.GetAllActiveSymbols().Select(s => s.Symbol).Distinct().ToArray();
                }

                try
                {
                    foreach (var symbol in args)
                    {
                        //Process_DivergingRSIBottom(symbol);
                        Process_VWAPStochRSIPivotPoint(symbol);
                    }
                }
                catch (Exception ex)
                {
                    Log(ex, "Failed on loop");
                }

                return result.SetSuccess(true);
            }
            catch (Exception e)
            {
                return result.SetFailure(e.Message);
            }
        }

        internal async Task CleanupPreviousOrders()
        {
            try
            {
                var previousBuysAcc = new PreviousBuysAccessor();
                var allOpenOrders = previousBuysAcc.GetAllOpenOrders();

                var kucoinAcc = new KuCoinAccessor();
                foreach (var openOrder in allOpenOrders)
                {
                    var buyOrder = await kucoinAcc.GetOrder(openOrder.BuyOrderId);
                    var sellOrder = await kucoinAcc.GetOrder(openOrder.SellOrderId);

                    if (sellOrder != null
                        && (buyOrder == null || !buyOrder.IsActive || buyOrder.StopTriggered))
                    {
                        await kucoinAcc.CancelOrder(openOrder.SellOrderId);
                    }

                    if (buyOrder != null
                        && (sellOrder == null || !sellOrder.IsActive))
                    {
                        await kucoinAcc.CancelOrder(openOrder.BuyOrderId);
                    }
                }

            }
            catch (Exception e)
            {
                Log(e, $"Exception caught CleanupPreviousOrders()");
            }
        }

        internal async Task Process_VWAPStochRSIPivotPoint(string symbol)
        {
            try
            {
                var taapiAcc = new TAApiAccessor();
                var kucoinAcc = new KuCoinAccessor();
                var fcspiAcc = new FCSApiAccessor();
                var symbolAcc = new SymbolAccessor();

                var vwapData = taapiAcc.GetVWAPData(symbol, Configs.VWAPStochRSIPivotPoint.VWAPDataInterval, Configs.VWAPStochRSIPivotPoint.VWAPBacktracks);
                if (vwapData?.Data == null)
                    vwapData = taapiAcc.GetVWAPData(symbol, Configs.VWAPStochRSIPivotPoint.VWAPDataInterval, Configs.VWAPStochRSIPivotPoint.VWAPBacktracks, "bitstamp");
                if (vwapData?.Data == null) { symbolAcc.SetIsActive(symbol, false); return; }

                var stochRSIData = taapiAcc.GetStochRSIData(symbol, Configs.VWAPStochRSIPivotPoint.StochRSIDataInterval, Configs.VWAPStochRSIPivotPoint.StochRSIBacktracks);
                if (stochRSIData?.Data == null)
                    stochRSIData = taapiAcc.GetStochRSIData(symbol, Configs.VWAPStochRSIPivotPoint.StochRSIDataInterval, Configs.VWAPStochRSIPivotPoint.StochRSIBacktracks, "bitstamp");
                if (stochRSIData?.Data == null) { symbolAcc.SetIsActive(symbol, false); return; }

                var candlesData = taapiAcc.GetCandlesData(symbol, Configs.VWAPStochRSIPivotPoint.CandlesDataInterval, Configs.VWAPStochRSIPivotPoint.CandlesBacktracks);
                if (candlesData?.Data == null)
                    candlesData = taapiAcc.GetCandlesData(symbol, Configs.VWAPStochRSIPivotPoint.CandlesDataInterval, Configs.VWAPStochRSIPivotPoint.CandlesBacktracks, "bitstamp");
                if (candlesData?.Data == null) { symbolAcc.SetIsActive(symbol, false); return; }

                var pivotPointsData = fcspiAcc.GetPivotPointData(symbol, Configs.VWAPStochRSIPivotPoint.PivotPointsDataInterval);
                if (pivotPointsData?.Data?.response?.pivot_point?.fibonacci == null) { symbolAcc.SetIsActive(symbol, false); return; }

                var currentPrice = await kucoinAcc.GetCurrentPrice(symbol);
                if (currentPrice <= 0m) { symbolAcc.SetIsActive(symbol, false); return; }

                var empValidationEng = new EmperorValidationEngine_VWAPStochRSIPivotPoint();
                var validationResult = empValidationEng.ProcessData(candlesData, vwapData, stochRSIData, pivotPointsData, currentPrice);

                if (validationResult.ShouldDoIt)
                {
                    Log($" - !!!!!  Doin it for {symbol} (VWAPStochRSIPivotPoint) at ${currentPrice}");

                    var currentBalance = await kucoinAcc.GetAccountBalance();
                    if (!currentBalance.HasValue) return;

                    var symbolDetails = await kucoinAcc.GetSymbolDetails(symbol);

                    var scalpProcessingEng = new ScalpProcessingEngine();
                    var buyInstructions = scalpProcessingEng.CalculateScalp(currentBalance ?? 0m, Configs.VWAPStochRSIPivotPoint.MaxScalpBankrollPercent,
                        Configs.VWAPStochRSIPivotPoint.TargetScalpPercent, Configs.VWAPStochRSIPivotPoint.TargetScalpIncrement, currentPrice, symbolDetails.PriceIncrementDecimalPlaces);

                    var buyDetail = new BuyDetail(symbol, buyInstructions, "VWAPStochRSIPivotPoint");

                    buyDetail.BuyOrderId = await kucoinAcc.SubmitLimitBuy_FAKE(symbol, buyInstructions.LimitBuyPrice, buyInstructions.StopLossPrice, buyInstructions.Quantity);
                    if (!string.IsNullOrEmpty(buyDetail.BuyOrderId))
                        buyDetail.SellOrderId = await kucoinAcc.SubmitLimitSell_FAKE(symbol, buyInstructions.LimitSellPrice, buyInstructions.Quantity);

                    this.LogBuy(buyDetail);
                }
                else
                {
                    Log($" - Not doin it for {symbol} (VWAPStochRSIPivotPoint) at ${currentPrice} at {validationResult.ConfidenceLevel}% confidence:  ( StochRIS[{validationResult.SatisfiedStochRSI}], VWAP[{validationResult.SatisfiedVWAP}], PivotPoints[{validationResult.SatisfiedPivotPoints}], NextCandle[{validationResult.SatisfiedNextCandle}] )");
                }
            }
            catch (Exception e)
            {
                Log(e, $"Exception caught Process_VWAPStochRSIPivotPoint({symbol})");
            }
        }

        internal async Task Process_DivergingRSIBottom(string symbol)
        {
            try
            {
                var taapiAcc = new TAApiAccessor();
                var kucoinAcc = new KuCoinAccessor();
                var symbolAcc = new SymbolAccessor();

                var momentumRSIData = taapiAcc.GetMomentumRSIData(symbol, Configs.DivergingRSIBottom.MomentumRSIDataInterval, Configs.DivergingRSIBottom.MomentumRSIBacktracks);
                if (momentumRSIData?.Data == null)
                    momentumRSIData = taapiAcc.GetMomentumRSIData(symbol, Configs.DivergingRSIBottom.MomentumRSIDataInterval, Configs.DivergingRSIBottom.MomentumRSIBacktracks, "bitstamp");
                if (momentumRSIData?.Data == null) { symbolAcc.SetIsActive(symbol, false); return; }

                var candlesData = taapiAcc.GetCandlesData(symbol, Configs.DivergingRSIBottom.CandlesDataInterval, Configs.DivergingRSIBottom.CandlesBacktracks);
                if (candlesData?.Data == null)
                    candlesData = taapiAcc.GetCandlesData(symbol, Configs.DivergingRSIBottom.CandlesDataInterval, Configs.DivergingRSIBottom.CandlesBacktracks, "bitstamp");
                if (candlesData?.Data == null) { symbolAcc.SetIsActive(symbol, false); return; }

                var currentPrice = await kucoinAcc.GetCurrentPrice(symbol);
                if (currentPrice <= 0m) { symbolAcc.SetIsActive(symbol, false); return; }

                var empValidationEng = new EmperorValidationEngine_DivergingRSIBottom();
                var validationResult = empValidationEng.ProcessData(candlesData, momentumRSIData, currentPrice);

                if (validationResult.ShouldDoIt)
                {
                    Log($" - !!!!!  Doin it for {symbol} (DivergingRSIBottom) at ${currentPrice}");

                    var currentBalance = await kucoinAcc.GetAccountBalance();
                    if (!currentBalance.HasValue) return;

                    var symbolDetails = await kucoinAcc.GetSymbolDetails(symbol);

                    var scalpProcessingEng = new ScalpProcessingEngine();
                    var buyInstructions = scalpProcessingEng.CalculateScalp(currentBalance ?? 0m, Configs.DivergingRSIBottom.MaxScalpBankrollPercent,
                        Configs.DivergingRSIBottom.TargetScalpPercent, Configs.DivergingRSIBottom.TargetScalpIncrement, currentPrice, symbolDetails.PriceIncrementDecimalPlaces);

                    var buyDetail = new BuyDetail(symbol, buyInstructions, "DivergingRSIBottom");

                    buyDetail.BuyOrderId = await kucoinAcc.SubmitLimitBuy_FAKE(symbol, buyInstructions.LimitBuyPrice, buyInstructions.StopLossPrice, buyInstructions.Quantity);
                    if (!string.IsNullOrEmpty(buyDetail.BuyOrderId))
                        buyDetail.SellOrderId = await kucoinAcc.SubmitLimitSell_FAKE(symbol, buyInstructions.LimitSellPrice, buyInstructions.Quantity);

                    this.LogBuy(buyDetail);
                }
                else
                {
                    Log($" - Not doin it for {symbol} (DivergingRSIBottom) at ${currentPrice} at {validationResult.ConfidenceLevel}% confidence:  ( BullEngulf[{validationResult.SatisfiedBullishEngulfingCandle}], HL_RSI[{validationResult.SatisfiedHigherLowRSI}], LL_Price[{validationResult.SatisfiedLowerLowPrice}], VolUpFirst[{validationResult.SatisfiedIncreasedVolumeAtFirstBottom}], VolUpLast[{validationResult.SatisfiedIncreasedVolumeAtLastBottom}], SupplyAbsorb[{validationResult.SatisfiedSupplyAbsorbingCandle}] )");
                }
            }
            catch (Exception e)
            {
                Log(e, $"Exception caught DivergingRSIBottom({symbol})");
            }
        }
    }
}
