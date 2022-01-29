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
    public class FCSApiAccessor : BaseCoinService
    {
        bool useTestData = false;
        public FCSApiAccessor(bool returnTestData = false)
        {
            this.useTestData = returnTestData;
        }

        public PivotPointsDTO GetPivotPointData(string symbol, string dataInterval)
        {
            if (this.useTestData) return new PivotPointsDTO() { Data = (PivotPointsDTO.PivotPointsRoot)JsonConvert.DeserializeObject(@"{""status"":true,""code"":200,""msg"":""Successfully"",""response"":{""pivot_point"":{""classic"":{""pp"":""1.2584"",""R1"":""1.2708"",""R2"":""1.277"",""R3"":""1.2894"",""S1"":""1.2522"",""S2"":""1.2398"",""S3"":""1.2336""},""fibonacci"":{""pp"":""1.2584"",""R1"":""1.2655"",""R2"":""1.2699"",""R3"":""1.277"",""S1"":""1.2513"",""S2"":""1.2469"",""S3"":""1.2398""},""camarilla"":{""pp"":""1.2584"",""R1"":""1.2663"",""R2"":""1.268"",""R3"":""1.2697"",""R4"":""1.2748"",""S1"":""1.2629"",""S2"":""1.2612"",""S3"":""1.2595"",""S4"":""1.2544""},""woodie"":{""pp"":""1.2599"",""R1"":""1.2739"",""R2"":""1.2785"",""S1"":""1.2553"",""S2"":""1.2413""},""demark"":{""high"":""1.2739"",""low"":""1.2553"",""R1"":""1.2739"",""S1"":""1.2553""}},""overall"":{""summary"":""Strong Buy"",""change_at"":""2021-04-26 17:21:21"",""msg"":""summary based on all SMA,EMA,Pivot Points and indicators""}},""info"":{""id"":""80"",""decimal"":""5"",""symbol"":""XRP\/USD"",""period"":""15m"",""disclaimer"":""Note: These Prices, Market trends and signals are not develop for trading purpose. Therefore we doesn`t bear any responsibility for any trading losses you might incur as a result of using this data."",""update"":""just now"",""update_time"":""2021-04-26 17:21:21 UTC"",""server_time"":""2021-04-26 17:21:21 UTC"",""credit_count"":1}}", typeof(PivotPointsDTO.PivotPointsRoot)) };

            PivotPointsDTO result = null;
            try
            {
                var secret = Configs.FCSAPI.APIKey;
                //httpshttps://fcsapi.com/api-v3/crypto/pivot_points?symbol={symbol}&period={dataInterval}&access_key={secret}

                symbol = CleanupSymbol(symbol);

                var url = $"https://fcsapi.com/api-v3/crypto/pivot_points?symbol={symbol}&period={dataInterval}&access_key={secret}";
                var response = this.ProcessHTTPGet(url);

                if (!string.IsNullOrEmpty(response))
                {
                    var items = (PivotPointsDTO.PivotPointsRoot)JsonConvert.DeserializeObject(response, typeof(PivotPointsDTO.PivotPointsRoot));
                    result = new PivotPointsDTO() { Data = items };
                }
            }
            catch (Exception e)
            {
                Log(e, $"FCSApiAccessor.GetPivotPointData({symbol}, {dataInterval})");
            }
            return result;
        }

        public override string CleanupSymbol(string symbol)
        {
            symbol = symbol.Replace("-", "/");
            symbol = symbol.Replace("USDT", "USD");
            if (!symbol.Contains('/')) return null;
            if (!symbol.Contains("USD")) return null;
            return symbol;
        }

        #region helper classes
        public class PivotPointsDTO
        {
            public PivotPointsRoot Data { get; set; }
            public class Classic
            {
                public string pp { get; set; }
                public string R1 { get; set; }
                public string R2 { get; set; }
                public string R3 { get; set; }
                public string S1 { get; set; }
                public string S2 { get; set; }
                public string S3 { get; set; }
            }

            public class Fibonacci
            {
                public string pp { get; set; }
                public string R1 { get; set; }
                public string R2 { get; set; }
                public string R3 { get; set; }
                public string S1 { get; set; }
                public string S2 { get; set; }
                public string S3 { get; set; }
            }

            public class Camarilla
            {
                public string pp { get; set; }
                public string R1 { get; set; }
                public string R2 { get; set; }
                public string R3 { get; set; }
                public string R4 { get; set; }
                public string S1 { get; set; }
                public string S2 { get; set; }
                public string S3 { get; set; }
                public string S4 { get; set; }
            }

            public class Woodie
            {
                public string pp { get; set; }
                public string R1 { get; set; }
                public string R2 { get; set; }
                public string S1 { get; set; }
                public string S2 { get; set; }
            }

            public class Demark
            {
                public string high { get; set; }
                public string low { get; set; }
                public string R1 { get; set; }
                public string S1 { get; set; }
            }

            public class PivotPoint
            {
                public Classic classic { get; set; }
                public Fibonacci fibonacci { get; set; }
                public Camarilla camarilla { get; set; }
                public Woodie woodie { get; set; }
                public Demark demark { get; set; }
            }

            public class Overall
            {
                public string summary { get; set; }
                public string change_at { get; set; }
                public string msg { get; set; }
            }

            public class Response
            {
                public PivotPoint pivot_point { get; set; }
                public Overall overall { get; set; }
            }

            public class Info
            {
                public string id { get; set; }
                public string @decimal { get; set; }
                public string symbol { get; set; }
                public string period { get; set; }
                public string disclaimer { get; set; }
                public string update { get; set; }
                public string update_time { get; set; }
                public string server_time { get; set; }
                public int credit_count { get; set; }
                public string _t { get; set; }
            }

            public class PivotPointsRoot
            {
                public bool status { get; set; }
                public int code { get; set; }
                public string msg { get; set; }
                public Response response { get; set; }
                public Info info { get; set; }

                public override string ToString()
                {
                    List<double> items = new List<double>();
                    if (double.TryParse(this.response.pivot_point.fibonacci.R1, out var val)) items.Add(val);
                    if (double.TryParse(this.response.pivot_point.fibonacci.R2, out val)) items.Add(val);
                    if (double.TryParse(this.response.pivot_point.fibonacci.R3, out val)) items.Add(val);
                    if (double.TryParse(this.response.pivot_point.fibonacci.S1, out val)) items.Add(val);
                    if (double.TryParse(this.response.pivot_point.fibonacci.S2, out val)) items.Add(val);
                    if (double.TryParse(this.response.pivot_point.fibonacci.S3, out val)) items.Add(val);

                    return string.Join(", ", items);
                }
            }
        }
        #endregion
    }
}
