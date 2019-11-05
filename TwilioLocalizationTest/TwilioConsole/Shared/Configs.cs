using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilioConsole.Shared
{
    public static class Configs
    {
        public static string SmsAccountSid => ConfigurationManager.AppSettings["Sms.AccountSid"];
        public static string SmsAuthToken => ConfigurationManager.AppSettings["Sms.AuthToken"];
        public static string SmsSourcePhoneNumber => ConfigurationManager.AppSettings["Sms.SourcePhoneNumber"];
    }
}
