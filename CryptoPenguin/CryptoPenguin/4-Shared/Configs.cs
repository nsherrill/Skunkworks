using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared
{
    public static class Configs
    {
        public static class VWAPStochRSIPivotPoint
        {
            public static string VWAPDataInterval => GetAppSetting_String("VWAPStochRSIPivotPoint.VWAPDataInterval", "15m");
            public static int VWAPBacktracks => GetAppSetting_Int("VWAPStochRSIPivotPoint.VWAPBacktracks", 5);

            public static string StochRSIDataInterval => GetAppSetting_String("VWAPStochRSIPivotPoint.StochRSIDataInterval", "15m");
            public static int StochRSIBacktracks => GetAppSetting_Int("VWAPStochRSIPivotPoint.StochRSIBacktracks", 5);

            public static string CandlesDataInterval => GetAppSetting_String("VWAPStochRSIPivotPoint.CandlesDataInterval", "15m");
            public static int CandlesBacktracks => GetAppSetting_Int("VWAPStochRSIPivotPoint.CandlesDataBacktracks", 5);

            public static string PivotPointsDataInterval => GetAppSetting_String("VWAPStochRSIPivotPoint.PivotPointsDataInterval", "15m");

            public static double TargetScalpPercent => GetAppSetting_Double("VWAPStochRSIPivotPoint.TargetScalpPercent", .0127); // 1.27%
            public static double MaxScalpBankrollPercent => GetAppSetting_Double("VWAPStochRSIPivotPoint.MaxScalpBankrollPercent", .1); // 10%%
            public static decimal TargetScalpIncrement => (decimal)GetAppSetting_Double("VWAPStochRSIPivotPoint.TargetScalpIncrement", 2); // $2
        }

        public static class DivergingRSIBottom
        {
            public static string MomentumRSIDataInterval => GetAppSetting_String("DivergingRSIBottom.MomentumRSIDataInterval", "4h");
            public static int MomentumRSIBacktracks => GetAppSetting_Int("DivergingRSIBottom.MomentumRSIBacktracks", 20);

            public static string CandlesDataInterval => GetAppSetting_String("DivergingRSIBottom.CandlesDataInterval", "4h");
            public static int CandlesBacktracks => GetAppSetting_Int("DivergingRSIBottom.CandlesDataBacktracks", 20);

            public static double TargetScalpPercent => GetAppSetting_Double("DivergingRSIBottom.TargetScalpPercent", .025); // 2.5%
            public static double MaxScalpBankrollPercent => GetAppSetting_Double("DivergingRSIBottom.MaxScalpBankrollPercent", .1); // 10%%
            public static decimal TargetScalpIncrement => (decimal)GetAppSetting_Double("DivergingRSIBottom.TargetScalpIncrement", 10); // $10
        }

        public static class TAAPI
        {
            public static string APIKey => GetAppSetting_String("APIKey", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InVuZ3VzYzczK3RhYXBpQGdtYWlsLmNvbSIsImlhdCI6MTYxOTQwMjE0NiwiZXhwIjo3OTI2NjAyMTQ2fQ.LbT0bVBtkfoIvbGWsDoS0m4cJdnwHAAkyQWj9jIUA3E");
        }

        public static class FCSAPI
        {
            public static string APIKey => GetAppSetting_String("APIKey", "QdF7JmIzYpllYdXkKgyvNw");

            
        }

        public static class KuCoinAPI
        {
            //// main API acct
            //public static string APIKey => GetAppSetting_String("APIKey", "6087265b0a80a20006c5b7fe"); // available at kucoin.com
            //public static string APISecret => GetAppSetting_String("APISecret", "4ab93405-8fe7-4836-a4d1-2eb832db33e1"); // provided at create-time
            //public static string Passphrase => GetAppSetting_String("Passphrase", "caf3d774af89"); // self-created

            ////API subaccount
            //public static string APIKey => GetAppSetting_String("APIKey", "6089db478304410006f29e58"); // available at kucoin.com
            //public static string APISecret => GetAppSetting_String("APISecret", "c11f885a-d942-419b-8f4b-c7977a14163e"); // provided at create-time
            //public static string Passphrase => GetAppSetting_String("Passphrase", "caf3d774af89"); // self-created

            //// real acct
            //public static string APIKey => GetAppSetting_String("APIKey", "6089da1370b7ca00060a54cb"); // available at kucoin.com
            //public static string APISecret => GetAppSetting_String("APISecret", "c0ccc34c-845f-4367-a06c-b9d699405b77"); // provided at create-time
            //public static string Passphrase => GetAppSetting_String("Passphrase", "caf3d774af89"); // self-created
            
            // real acct - API subaccount
            public static string APIKey => GetAppSetting_String("APIKey", "608aae005ef8260006cbc6ce"); // available at kucoin.com
            public static string APISecret => GetAppSetting_String("APISecret", "14c2492b-5d4a-4819-8420-1f94eabd1f2d"); // provided at create-time
            public static string Passphrase => GetAppSetting_String("Passphrase", "caf3d774af89"); // self-created

        }

        internal static string[] GetAppSetting_StringArray(string key, string[] dft = null)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return dft;

                var vals = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(vals))
                {
                    var result = vals.Split(',');
                    return result;
                }
            }
            catch
            {
            }
            return dft;
        }
        internal static string GetAppSetting_String(string key, string dft = null)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return dft;

                var result = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(result)) return dft;

                return result;
            }
            catch
            {
            }
            return dft;
        }
        internal static int GetAppSetting_Int(string key, int dft = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return dft;

                var val = ConfigurationManager.AppSettings[key];
                if (int.TryParse(val, out var result))
                    return result;
            }
            catch
            {
            }
            return dft;
        }
        internal static double GetAppSetting_Double(string key, double dft = 0.0)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return dft;

                var val = ConfigurationManager.AppSettings[key];
                if (double.TryParse(val, out var result))
                    return result;
            }
            catch
            {
            }
            return dft;
        }
    }
}
