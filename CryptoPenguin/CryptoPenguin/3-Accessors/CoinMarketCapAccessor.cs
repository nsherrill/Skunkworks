using CryptoPenguin.Shared.BaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Accessors
{
    public class CoinMarketCapAccessor : BaseService
    {
        public PivotPointsDTO GetPivotPointsData(string symbol, string dataInterval)
        {
            PivotPointsDTO result = null;
            try
            {
                //var secret = Configs.TAAPI.APIKey;
                ////https://api.taapi.io/stochrsi?secret=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InVuZ3VzYzczK3RhYXBpQGdtYWlsLmNvbSIsImlhdCI6MTYxOTQwMjE0NiwiZXhwIjo3OTI2NjAyMTQ2fQ.LbT0bVBtkfoIvbGWsDoS0m4cJdnwHAAkyQWj9jIUA3E&exchange=binance&symbol=XRP/USDT&interval=1h&backtracks=5

                //var url = $"https://api.taapi.io/stochrsi?secret={secret}&exchange=binance&symbol={symbol}&interval={dataInterval}&backtracks={backtracks}";
                //var response = this.ProcessHTTPGet(url);

                //if (!string.IsNullOrEmpty(response))
                //{
                //    result = (StochRSIDTO)JsonConvert.DeserializeObject(response, typeof(StochRSIDTO));
                //}
            }
            catch (Exception e)
            {
                Log(e, $"CoinMarketCapAccessor.GetPivotPointsData({symbol}, {dataInterval})");
            }
            return result;
        }

        public class PivotPointsDTO
        {
            public PivotPointsRow[] Data { get; set; }
            public class PivotPointsRow
            {
            }
        }

    }
}
