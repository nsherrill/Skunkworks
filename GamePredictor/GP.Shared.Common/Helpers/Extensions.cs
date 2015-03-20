using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public static class Extensions
    {
        public static T GetValue<T>(this List<ValuePair> source, string key, T defaultValue)
        {
            T result = defaultValue;
            foreach (var tempVal in source)
                if (tempVal.Key == key)
                {
                    var keyType = typeof(T).ToString();
                    switch (keyType)
                    {
                        case "System.Double":
                            double res;
                            if (tempVal.Value is double)
                                return (T)tempVal.Value;
                            else if (double.TryParse(tempVal.Value.ToString(), out res))
                                return (T)(object)res;
                            break;
                    }
                    break;
                }

            return defaultValue;
        }

        #region SqlDataReader

        public static long GetLong(this SqlDataReader source, string colName, long dft = -1)
        {
            int index = source.GetIndex(colName);
            if (index >= 0)
            {
                if (source.IsDBNull(index))
                    return dft;

                var type = source.GetFieldType(index);
                if (type == typeof(long))
                    return source.GetInt64(index);
                long result;
                if (long.TryParse(source[index].ToString(), out result))
                    return result;
            }
            return dft;
        }

        public static int GetInt(this SqlDataReader source, string colName, int dft = -1)
        {
            int index = source.GetIndex(colName);
            if (index >= 0)
            {
                if (source.IsDBNull(index))
                    return dft;

                var type = source.GetFieldType(index);
                if (type == typeof(int))
                    return source.GetInt32(index);
                int result;
                if (int.TryParse(source[index].ToString(), out result))
                    return result;
            }
            return dft;
        }

        public static bool GetBool(this SqlDataReader source, string colName, bool dft = true)
        {
            int index = source.GetIndex(colName);
            if (index >= 0)
            {
                if (source.IsDBNull(index))
                    return dft;

                var type = source.GetFieldType(index);
                if (type == typeof(bool))
                    return source.GetBoolean(index);
                if (type == typeof(int))
                    return source.GetInt32(index) > 0;

                bool result;
                if (bool.TryParse(source[index].ToString(), out result))
                    return result;

                int intResult;
                if (int.TryParse(source[index].ToString(), out intResult))
                    return intResult > 0;
            }
            return dft;
        }

        public static string GetString(this SqlDataReader source, string colName, string dft = null)
        {
            int index = source.GetIndex(colName);
            if (index >= 0)
            {
                if (source.IsDBNull(index))
                    return dft;

                return source[index].ToString();
            }
            return dft;
        }

        public static DateTime GetDateTime(this SqlDataReader source, string colName, DateTime dft)
        {
            int index = source.GetIndex(colName);
            if (index >= 0)
            {
                if (source.IsDBNull(index))
                    return dft;

                var type = source.GetFieldType(index);
                if (type == typeof(DateTime))
                    return source.GetDateTime(index);

                DateTime result;
                if (DateTime.TryParse(source[index].ToString(), out result))
                    return result;
            }
            return dft;
        }

        public static double GetDouble(this SqlDataReader source, string colName, double dft = -1.0)
        {
            int index = source.GetIndex(colName);
            if (index >= 0)
            {
                if (source.IsDBNull(index))
                    return dft;

                var type = source.GetFieldType(index);
                if (type == typeof(decimal))
                    return (double)source.GetDecimal(index);
                if (type == typeof(int))
                    return source.GetInt32(index);

                double result;
                if (double.TryParse(source[index].ToString(), out result))
                    return result;

                int intResult;
                if (int.TryParse(source[index].ToString(), out intResult))
                    return intResult;
            }
            return dft;
        }

        public static T GetEnum<T>(this SqlDataReader source, string colName, T dft) where T : struct
        {
            int index = source.GetIndex(colName);
            if (index >= 0)
            {
                if (source.IsDBNull(index))
                    return dft;

                T result;

                var tempResult = source[index].ToString();
                if (Enum.TryParse<T>(tempResult, true, out result))
                    return result;

            }
            return dft;
        }

        public static int GetIndex(this SqlDataReader source, string colName)
        {
            for (int i = 0; i < source.FieldCount; i++)
                if (source.GetName(i).Equals(colName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            return -1;
        }
        #endregion

        #region
        public static object GetData(this PlayerEventStats source, string propName)
        {
            if (source.Data != null
                && source.Data.Length > 0)
            {
                foreach (var prop in source.Data)
                {
                    if (prop != null
                        && !string.IsNullOrEmpty(prop.Key)
                        && prop.Key.Equals(propName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        double temp;
                        if (!double.TryParse(prop.Value.ToString(), out temp))
                        {

                        }
                        return prop.Value;
                    }
                }
            }

            return 0;
            //throw new Exception("Prop doesn't exist: " + propName);
            //return null;
        }
        #endregion
    }
}
