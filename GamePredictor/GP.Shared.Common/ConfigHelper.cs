using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public static class ConfigHelper
    {
        public static double MinimumBuyIn { get { return GetConfigDouble("MinimumBuyIn", 0d); } }
        public static double MaximumBuyIn { get { return GetConfigDouble("MaximumBuyIn", 0d); } }

        public static string DBConnectionString { get { return GetConnectionString("DBConnection"); } }

        public static string GetConnectionString(string key)
        {
            string result = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            return result;
        }

        #region privates
        private static object GetConfigObject(string key, object defaultValue)
        {
            object result = defaultValue;

            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                result = ConfigurationManager.AppSettings[key];

            if (result == null)
                result = defaultValue;

            return result;
        }

        private static double GetConfigDouble(string key, double defaultValue)
        {
            var tempRes = GetConfigObject(key, defaultValue);
            double typedTempRes;
            if (tempRes is double)
            {
                return (double)tempRes;
            }
            else if (double.TryParse(tempRes.ToString(), out typedTempRes))
            {
                return typedTempRes;
            }
            return defaultValue;
        }
        #endregion

        public static string FanDuelPassword { get { return "Mackelv!3n"; } }

        public static string FanDuelUserName { get { return "ungusc73@yahoo.com"; } }
    }
}
