using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend
{
    public enum ErrorTextType
    {
        Public,
        Internal
    }

    public static class ErrorCodes
    {
        public static string InvalidSearchParameter { get { return "Error 201: Invalid search parameter"; } }

        private static string public_ExceptionFormatter { get { return "Error 200: Exception caught [{0}]"; } }
        private static string internal_ExceptionFormatter { get { return "Error 200: Exception caught: {0}"; } }
        public static string ExceptionCaught(ErrorTextType errorType, Exception exc)
        {
            if (errorType == ErrorTextType.Public)
                return string.Format(public_ExceptionFormatter, exc.Message);
            else
                return string.Format(internal_ExceptionFormatter, exc.ToString());
        }
    }
}
