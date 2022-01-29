using CryptoPenguin.Shared;
using CryptoPenguin.Shared.BaseObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Accessors
{
    public class TAApiAccessor : BaseCoinService
    {
        bool useTestData = false;
        public TAApiAccessor(bool returnTestData = false)
        {
            this.useTestData = returnTestData;
        }

        public VWAPDTO GetVWAPData(string symbol, string dataInterval, int backtracks, string exchange = "binance")
        {
            if (this.useTestData) return new VWAPDTO() { Data = (VWAPDTO.VWAPBacktrack[])JsonConvert.DeserializeObject(@"[{""value"":1.257832921585818,""backtrack"":0},{""value"":1.2580661045874457,""backtrack"":1},{""value"":1.2584792760852286,""backtrack"":2},{""value"":1.1479,""backtrack"":3},{""value"":1.2599,""backtrack"":4}]", typeof(VWAPDTO.VWAPBacktrack[])) };

            VWAPDTO result = null;
            try
            {
                var secret = Configs.TAAPI.APIKey;
                //https://api.taapi.io/vwap?secret=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InVuZ3VzYzczK3RhYXBpQGdtYWlsLmNvbSIsImlhdCI6MTYxOTQwMjE0NiwiZXhwIjo3OTI2NjAyMTQ2fQ.LbT0bVBtkfoIvbGWsDoS0m4cJdnwHAAkyQWj9jIUA3E&exchange=bitstamp&symbol=XRP/USDT&interval=1h&backtracks=5

                symbol = CleanupSymbol(symbol);

                var url = $"https://api.taapi.io/vwap?secret={secret}&exchange={exchange}&symbol={symbol}&interval={dataInterval}&backtracks={backtracks}&anchorPeriod=session";
                var response = this.ProcessHTTPGet(url);

                if (!string.IsNullOrEmpty(response))
                {
                    var items = (VWAPDTO.VWAPBacktrack[])JsonConvert.DeserializeObject(response, typeof(VWAPDTO.VWAPBacktrack[]));
                    result = new VWAPDTO() { Data = items.OrderByDescending(i=>i.backtrack).ToArray() };
                }
            }
            catch (Exception e)
            {
                //Log(e, $"TAApiAccessor.GetVWAPData({symbol}, {dataInterval}, {backtracks})");
            }
            return result;
        }
        
        public StochRSIDTO GetStochRSIData(string symbol, string dataInterval, int backtracks, string exchange = "binance")
        {
            if (this.useTestData) return new StochRSIDTO() { Data = (StochRSIDTO.StochRSIBacktrack[])JsonConvert.DeserializeObject(@"[{""valueFastK"":55.81502131846507,""valueFastD"":41.11511971138086,""backtrack"":0},{""valueFastK"":21.81698917677927,""valueFastD"":27.8650688045458,""backtrack"":1},{""valueFastK"":15.713348638897997,""valueFastD"":20.105218707551227,""backtrack"":2},{""valueFastK"":19.96486859795988,""valueFastD"":19.87236887936324,""backtrack"":3},{""valueFastK"":18.537438885795563,""valueFastD"":29.103958723505574,""backtrack"":4}]", typeof(StochRSIDTO.StochRSIBacktrack[])) };

            StochRSIDTO result = null;
            try
            {
                var secret = Configs.TAAPI.APIKey;
                //https://api.taapi.io/stochrsi?secret=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InVuZ3VzYzczK3RhYXBpQGdtYWlsLmNvbSIsImlhdCI6MTYxOTQwMjE0NiwiZXhwIjo3OTI2NjAyMTQ2fQ.LbT0bVBtkfoIvbGWsDoS0m4cJdnwHAAkyQWj9jIUA3E&exchange=bitstamp&symbol=XRP/USDT&interval=1h&backtracks=5

                symbol = CleanupSymbol(symbol);
                var url = $"https://api.taapi.io/stochrsi?secret={secret}&exchange={exchange}&symbol={symbol}&interval={dataInterval}&backtracks={backtracks}";
                var response = this.ProcessHTTPGet(url);

                if (!string.IsNullOrEmpty(response))
                {
                    var items = (StochRSIDTO.StochRSIBacktrack[])JsonConvert.DeserializeObject(response, typeof(StochRSIDTO.StochRSIBacktrack[]));
                    result = new StochRSIDTO() { Data = items.OrderByDescending(i => i.backtrack).ToArray() };
                }
            }
            catch (Exception e)
            {
                Log(e, $"TAApiAccessor.GetStochRSIData({symbol}, {dataInterval}, {backtracks})");
            }
            return result;
        }

        public MomentumRSIDTO GetMomentumRSIData(string symbol, string dataInterval, int backtracks, string exchange = "binance")
        {
            if (this.useTestData) return new MomentumRSIDTO() { Data = (MomentumRSIDTO.MomentumRSIBacktrack[])JsonConvert.DeserializeObject(@"[{""valueFastK"":55.81502131846507,""valueFastD"":41.11511971138086,""backtrack"":0},{""valueFastK"":21.81698917677927,""valueFastD"":27.8650688045458,""backtrack"":1},{""valueFastK"":15.713348638897997,""valueFastD"":20.105218707551227,""backtrack"":2},{""valueFastK"":19.96486859795988,""valueFastD"":19.87236887936324,""backtrack"":3},{""valueFastK"":18.537438885795563,""valueFastD"":29.103958723505574,""backtrack"":4}]", typeof(MomentumRSIDTO.MomentumRSIBacktrack[])) };

            MomentumRSIDTO result = null;
            try
            {
                var secret = Configs.TAAPI.APIKey;
                //https://api.taapi.io/stochrsi?secret=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InVuZ3VzYzczK3RhYXBpQGdtYWlsLmNvbSIsImlhdCI6MTYxOTQwMjE0NiwiZXhwIjo3OTI2NjAyMTQ2fQ.LbT0bVBtkfoIvbGWsDoS0m4cJdnwHAAkyQWj9jIUA3E&exchange=bitstamp&symbol=XRP/USDT&interval=1h&backtracks=5

                symbol = CleanupSymbol(symbol);
                var url = $"https://api.taapi.io/rsi?secret={secret}&exchange={exchange}&symbol={symbol}&interval={dataInterval}&backtracks={backtracks}";
                var response = this.ProcessHTTPGet(url);

                if (!string.IsNullOrEmpty(response))
                {
                    var items = (MomentumRSIDTO.MomentumRSIBacktrack[])JsonConvert.DeserializeObject(response, typeof(MomentumRSIDTO.MomentumRSIBacktrack[]));
                    result = new MomentumRSIDTO() { Data = items.OrderByDescending(i => i.backtrack).ToArray() };
                }
            }
            catch (Exception e)
            {
                Log(e, $"TAApiAccessor.GetRSIMomentumData({symbol}, {dataInterval}, {backtracks})");
            }
            return result;
        }

        public CandlesDTO GetCandlesData(string symbol, string dataInterval, int backtracks, string exchange = "binance")
        {
            if (this.useTestData) return new CandlesDTO() { Data = (CandlesDTO.CandlesBacktrack[])JsonConvert.DeserializeObject(@"[{""timestampHuman"":""2021-04-25 14:00:00 (Sunday) UTC"",""timestamp"":1619359200,""open"":1.0783,""high"":1.105,""low"":1.0711,""close"":1.0994,""volume"":36998313.8800001},{""timestampHuman"":""2021-04-25 15:00:00 (Sunday) UTC"",""timestamp"":1619362800,""open"":1.0989,""high"":1.15,""low"":1.0915,""close"":1.1474,""volume"":86435265.32000089},{""timestampHuman"":""2021-04-25 16:00:00 (Sunday) UTC"",""timestamp"":1619366400,""open"":1.1469,""high"":1.156,""low"":1.1257,""close"":1.1419,""volume"":58279439.31000008},{""timestampHuman"":""2021-04-25 17:00:00 (Sunday) UTC"",""timestamp"":1619370000,""open"":1.1419,""high"":1.144,""low"":1.1021,""close"":1.1116,""volume"":44754714.51999997},{""timestampHuman"":""2021-04-25 18:00:00 (Sunday) UTC"",""timestamp"":1619373600,""open"":1.1112,""high"":1.1135,""low"":1.0823,""close"":1.0939,""volume"":42971825.03999984},{""timestampHuman"":""2021-04-25 19:00:00 (Sunday) UTC"",""timestamp"":1619377200,""open"":1.0942,""high"":1.0989,""low"":1.0601,""close"":1.0653,""volume"":33103884.110000234},{""timestampHuman"":""2021-04-25 20:00:00 (Sunday) UTC"",""timestamp"":1619380800,""open"":1.0651,""high"":1.0849,""low"":1.0222,""close"":1.0349,""volume"":79244811.47000055},{""timestampHuman"":""2021-04-25 21:00:00 (Sunday) UTC"",""timestamp"":1619384400,""open"":1.0348,""high"":1.0389,""low"":0.9418,""close"":0.9876,""volume"":173501292.70999938},{""timestampHuman"":""2021-04-25 22:00:00 (Sunday) UTC"",""timestamp"":1619388000,""open"":0.9876,""high"":1.0348,""low"":0.9803,""close"":1.0298,""volume"":97853055.8300014},{""timestampHuman"":""2021-04-25 23:00:00 (Sunday) UTC"",""timestamp"":1619391600,""open"":1.0299,""high"":1.0461,""low"":1.0166,""close"":1.0351,""volume"":51541404.65000009},{""timestampHuman"":""2021-04-26 00:00:00 (Monday) UTC"",""timestamp"":1619395200,""open"":1.0353,""high"":1.1168,""low"":1.0133,""close"":1.1045,""volume"":88373843.4900009},{""timestampHuman"":""2021-04-26 01:00:00 (Monday) UTC"",""timestamp"":1619398800,""open"":1.1042,""high"":1.1473,""low"":1.0827,""close"":1.1339,""volume"":66264911.06999925},{""timestampHuman"":""2021-04-26 02:00:00 (Monday) UTC"",""timestamp"":1619402400,""open"":1.1338,""high"":1.148,""low"":1.1061,""close"":1.1314,""volume"":60347095.33000077},{""timestampHuman"":""2021-04-26 03:00:00 (Monday) UTC"",""timestamp"":1619406000,""open"":1.1314,""high"":1.15,""low"":1.1288,""close"":1.1298,""volume"":18543928.81}]", typeof(CandlesDTO.CandlesBacktrack[])) };

            CandlesDTO result = null;
            try
            {
                var secret = Configs.TAAPI.APIKey;
                //https://api.taapi.io/candles?secret=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InVuZ3VzYzczK3RhYXBpQGdtYWlsLmNvbSIsImlhdCI6MTYxOTQwMjE0NiwiZXhwIjo3OTI2NjAyMTQ2fQ.LbT0bVBtkfoIvbGWsDoS0m4cJdnwHAAkyQWj9jIUA3E&exchange=bitstamp&symbol=XRP/USDT&interval=1h&backtracks=5

                symbol = CleanupSymbol(symbol);
                var url = $"https://api.taapi.io/candles?secret={secret}&exchange={exchange}&symbol={symbol}&interval={dataInterval}";
                var response = this.ProcessHTTPGet(url);

                if (!string.IsNullOrEmpty(response))
                {
                    var items = (CandlesDTO.CandlesBacktrack[])JsonConvert.DeserializeObject(response, typeof(CandlesDTO.CandlesBacktrack[]));
                    result = new CandlesDTO() { Data = items };
                }
            }
            catch (Exception e)
            {
                Log(e, $"TAApiAccessor.GetStochRSIData({symbol}, {dataInterval}, {backtracks})");
            }
            return result;
        }

        public override string CleanupSymbol(string symbol)
        {
            symbol = symbol.Replace("-", "/").ToUpper();
            if (!symbol.Contains('/')) return null;
            return symbol;
        }

        #region helperclasses
        public class VWAPDTO
        {
            public VWAPBacktrack[] Data { get; set; }
            public class VWAPBacktrack
            {
                public double value { get; set; }
                public int backtrack { get; set; }

                public override string ToString()
                {
                    return value.ToString();
                }
            }
        }

        public class StochRSIDTO
        {
            public StochRSIBacktrack[] Data { get; set; }
            public class StochRSIBacktrack
            {
                public double valueFastK { get; set; }
                public double valueFastD { get; set; }
                public int backtrack { get; set; }

                public bool IsBearish() { return valueFastD <= valueFastK; }
                public bool IsBullish() { return valueFastD >= valueFastK; }
                public override string ToString()
                {
                    var bstr = IsBullish() ? "BULLISH" : "BEARISH";
                    return $"{bstr}: {valueFastD}-{valueFastK}";
                }
            }
        }

        public class MomentumRSIDTO
        {
            public MomentumRSIBacktrack[] Data { get; set; }
            public class MomentumRSIBacktrack
            {
                public double value { get; set; }
                public int backtrack { get; set; }

                public override string ToString()
                {
                    return $"{backtrack}: {value}";
                }
            }
        }

        public class CandlesDTO
        {
            public CandlesBacktrack[] Data { get; set; }
            public class CandlesBacktrack
            {
                public string timestampHuman { get; set; }
                public int timestamp { get; set; }
                public double open { get; set; }
                public double high { get; set; }
                public double low { get; set; }
                public double close { get; set; }
                public double volume { get; set; }

                public DateTime? TypedTimeStamp
                {
                    get
                    {
                        if (DateTime.TryParse(timestampHuman.Replace("(Sunday)", "").Replace("(Monday)", "").Replace("(Tuesday)", "").Replace("(Wednesday)", "").Replace("(Thursday)", "").Replace("(Friday)", "").Replace("(Saturday)", "")
                            , out DateTime result))
                            return result;
                        return null;
                    }
                }
                public override string ToString()
                {
                    var gC = close > open ? "GREEN" : "RED";
                    return $"{gC}: {open}->{close} ({low}->{high})";
                }
            }
        }
        #endregion
    }
}
