using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend
{
    public static class ConfigHelper
    {
        public static string OAuth_ConsumerKey
        {
            get { return GetConfigString("OAuth_ConsumerKey", "SLhi3WzsmiNf4uLJ30g4MvEKj"); }
        }
        public static string OAuth_ConsumerSecret
        {
            get { return GetConfigString("OAuth_ConsumerSecret", "yDnSD4oGn3taam8fNTHctvz4GVXn97SiRej0E20CWE5aIxvgiH"); }
        }
        public static string OAuth_UserToken
        {
            get { return GetConfigString("OAuth_UserToken", "39583056-tEZvPlELK2u6cNHz1ve92xLNIHid9kvd29ARWpD9W"); }
        }
        public static string OAuth_UserTokenSecret
        {
            get { return GetConfigString("OAuth_UserTokenSecret", "3fksc6UWituz45TcGmmo0U8aymzXN1TmavxPirctk9J7W"); }
        }

        #region privates
        private static string GetConfigString(string key, string defaultValue = null)
        {
            var result = defaultValue;// ConfigurationSettings[key];
            return result;
        }
        #endregion
    }
}
