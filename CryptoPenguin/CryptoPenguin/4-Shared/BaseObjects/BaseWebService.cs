using CryptoPenguin.Shared.BaseObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared.BaseObjects
{
    public class BaseWebService : BaseService
    {
        public string ProcessHTTPGet(string url)
        {

            try
            {
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            var result = reader.ReadToEnd();
                            return result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Log(e, $"BaseWebService.ProcessHTTPGet({url})");
            }

            return null;
        }
    }
}
