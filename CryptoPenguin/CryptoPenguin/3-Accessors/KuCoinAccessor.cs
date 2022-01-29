using CryptoPenguin.Shared;
using CryptoPenguin.Shared.BaseObjects;
using CryptoPenguin.Shared.DTOs;
using Kucoin.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Accessors
{
    public class KuCoinAccessor : BaseCoinService
    {
        KucoinClientOptions clientOptions
        {
            get
            {
                return new KucoinClientOptions()
                {
                    ApiCredentials = new KucoinApiCredentials(Configs.KuCoinAPI.APIKey, Configs.KuCoinAPI.APISecret, Configs.KuCoinAPI.Passphrase)
                };
            }
        }

        public async Task<decimal?> GetAccountBalance()
        {
            try
            {
                using (var client = new Kucoin.Net.KucoinClient(clientOptions))
                {

                    var acct = client.GetAccount("609005a993a6df0006e422d4"); // usdt-specific trading account for the sub-account
                    return acct?.Data?.Available;//.Sum(a => a.Balance);
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.GetAccountBalance()");
            }
            return null;
        }

        public async Task<decimal> GetCurrentPrice(string symbol)
        {
            try
            {
                symbol = CleanupSymbol(symbol);
                using (var client = new Kucoin.Net.KucoinClient(clientOptions))
                {
                    var tickerInfo = client.GetTicker(symbol);
                    return tickerInfo.Data?.LastTradePrice ?? 0m;
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.GetCurrentPrice({symbol})");
            }
            return 0m;
        }

        public async Task<SymbolDetail> GetSymbolDetails(string symbol)
        {
            try
            {
                symbol = CleanupSymbol(symbol);
                using (var client = new Kucoin.Net.KucoinClient(clientOptions))
                {
                    var allSymbols = client.GetSymbols();
                    //var allSymbol = string.Join(",", allSymbols.Data.Where(s=>s.QuoteCurrency == "USDT").Select(s => $" ('{s.Name}')"));

                    var result = allSymbols?.Data?.FirstOrDefault(s => s.Symbol == symbol);

                    return ParseSymbolResult(result);
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.GetSymbolDetails({symbol})");
            }
            return null;
        }

        public async Task<OrderDetail> GetOrder(string orderId)
        {
            try
            {
                using (var client = new Kucoin.Net.KucoinClient(clientOptions))
                {
                    var result = client.GetOrder(orderId)?.Data;

                    return ParseOrderResult(result);
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.GetOrder({orderId})");
            }
            return null;
        }

        public async Task CancelOrder(string orderId)
        {
            try
            {

                using (var client = new Kucoin.Net.KucoinClient(clientOptions))
                {
                    var result = client.GetOrder(orderId)?.Data;
                    if (!string.IsNullOrEmpty(result?.Id))
                    {
                        this.Log($"CLEANUP - KuCoinAccessor.CancelOrder({orderId}) :   {result.Symbol} {result.Side} @ ${result.Price}");
                        client.CancelOrder(result.Id);
                    }
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.CancelOrder({orderId})");
            }
        }

        public async Task<string> SubmitLimitBuy_FAKE(string symbol, decimal limitBuyPrice, decimal stopLossPrice, decimal quantity)
        {
            try
            {
                Log($"!!!!!  LIMIT BUY - on {symbol} for {quantity} at ${limitBuyPrice} (SL at {stopLossPrice}");
                return Guid.NewGuid().ToString();
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.SubmitLimitBuy({symbol}, {limitBuyPrice}, {stopLossPrice}, {quantity}");
            }
            return null;
        }

        public async Task<string> SubmitLimitSell_FAKE(string symbol, decimal limitSellPrice, decimal quantity)
        {
            try
            {
                Log($"!!!!!  LIMIT SELL - on {symbol} for {quantity} at ${limitSellPrice}");
                return Guid.NewGuid().ToString();
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.SubmitLimitSell_FAKE({symbol}, {limitSellPrice}, {quantity}");
            }
            return null;
        }

        public async Task<string> SubmitLimitBuy(string symbol, decimal limitBuyPrice, decimal stopLossPrice, decimal quantity)
        {
            try
            {
                symbol = CleanupSymbol(symbol);
                if (string.IsNullOrEmpty(symbol) || limitBuyPrice <= 0 || stopLossPrice <= 0 || quantity <= 0) return null;

                using (var client = new Kucoin.Net.KucoinClient(clientOptions))
                {
                    var orderResult = client.PlaceOrder(symbol, KucoinOrderSide.Buy, KucoinNewOrderType.Limit, limitBuyPrice, quantity,
                        stop: KucoinStopCondition.Loss, stopPrice: stopLossPrice);

                    return orderResult?.Data?.OrderId;
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.SubmitLimitBuy({symbol}, {limitBuyPrice}, {stopLossPrice}, {quantity}");
            }
            return null;
        }

        public async Task<string> SubmitLimitSell(string symbol, decimal limitSellPrice, decimal quantity)
        {
            try
            {
                symbol = CleanupSymbol(symbol);
                if (string.IsNullOrEmpty(symbol) || limitSellPrice <= 0 || quantity <= 0) return null;

                using (var client = new Kucoin.Net.KucoinClient(clientOptions))
                {
                    var allSymbols = client.GetSymbols();
                    var thisSymbol = allSymbols?.Data.FirstOrDefault(s => s.Symbol == symbol);
                    if (thisSymbol != null)
                    {
                        limitSellPrice = limitSellPrice - (limitSellPrice % thisSymbol.PriceIncrement);
                        quantity = quantity - (quantity % thisSymbol.BaseIncrement);

                        //var acct = client.GetAccounts();
                        var orderResult = client.PlaceOrder(symbol, KucoinOrderSide.Sell, KucoinNewOrderType.Limit, limitSellPrice, quantity);
                        return orderResult?.Data?.OrderId;
                    }
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.SubmitLimitSell({symbol}, {limitSellPrice}, {quantity}");
            }
            return null;
        }

        public async Task<bool> CancelAllBuys(string symbol)
        {
            try
            {
                symbol = CleanupSymbol(symbol);
                if (string.IsNullOrEmpty(symbol)) return false;

                using (var client = new Kucoin.Net.KucoinClient(clientOptions))
                {
                    var allOrders = client.GetOrders(symbol, KucoinOrderSide.Buy, status: KucoinOrderStatus.Active);

                    if ((allOrders?.Data?.Items ?? Enumerable.Empty<KucoinOrder>()).Any())
                    {
                        foreach (var order in allOrders.Data.Items)
                        {
                            client.CancelOrder(order.Id);
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.CancelAllBuys({symbol}");
            }
            return false;
        }

        public async Task<bool> CancelAllSells(string symbol)
        {
            try
            {
                symbol = CleanupSymbol(symbol);
                if (string.IsNullOrEmpty(symbol)) return false;

                using (var client = new Kucoin.Net.KucoinClient(clientOptions))
                {
                    var allOrders = client.GetOrders(symbol, KucoinOrderSide.Sell, status: KucoinOrderStatus.Active);

                    if ((allOrders?.Data?.Items ?? Enumerable.Empty<KucoinOrder>()).Any())
                    {
                        foreach (var order in allOrders.Data.Items)
                        {
                            client.CancelOrder(order.Id);
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"KuCoinAccessor.CancelAllBuys({symbol}");
            }
            return false;
        }

        public override string CleanupSymbol(string symbol)
        {
            symbol = symbol.Replace("/", "-").ToUpper();
            if (!symbol.Contains('-')) return null;
            return symbol;
        }

        #region internals
        internal SymbolDetail ParseSymbolResult(KucoinSymbol symbol)
        {
            if (symbol == null) return null;

            return new SymbolDetail()
            {
                Name = symbol.Name,
                PriceIncrement = symbol.PriceIncrement,
            };
        }

        internal OrderDetail ParseOrderResult(KucoinOrder order)
        {
            if (order == null) return null;

            return new OrderDetail()
            {
                Id = order.Id,
                IsBuy = order.Side == KucoinOrderSide.Buy,
                IsSell = order.Side == KucoinOrderSide.Sell,
                StopTriggered = order.StopTriggered,
                IsActive = order.IsActive ?? false,
            };
        }
        #endregion
    }
}
