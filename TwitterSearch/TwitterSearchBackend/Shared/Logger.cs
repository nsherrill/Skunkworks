using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend
{
    public static class Logger
    {
        private static bool SetterComplete = false;

        public static void Error(string textToLog, Exception exc)
        {
            return;

            Init();
            string logText = textToLog + Environment.NewLine + exc.ToString();
            Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logText, "Standard");
        }

        internal static void Log(string textToLog)
        {
            return;

            Init();
            Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(textToLog, "Standard");
        }

        private static void Init()
        {
            if (!SetterComplete)
            {
                var myLogWriter = new Microsoft.Practices.EnterpriseLibrary.Logging.LogWriterFactory();
                Microsoft.Practices.EnterpriseLibrary.Logging.Logger.SetLogWriter(myLogWriter.Create());

                SetterComplete = true;
            }
        }
    }
}
