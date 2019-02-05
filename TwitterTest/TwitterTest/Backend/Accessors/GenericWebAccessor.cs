using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterTest.Backend.Accessors
{
    public class GenericWebAccessor
    {
        public T[] PostToUrl<T>(string url, Func<string, T> parserMethod)
        {
            List<T> result = new List<T>();

            string jsonText = string.Empty;
            for (int i = 0; i < 50; i++)
            {
                result.Add(parserMethod(jsonText));
            }

            return result.ToArray();
        }
    }
}