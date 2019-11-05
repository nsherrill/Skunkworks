using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwilioConsole.Shared.ExternalResponses;

namespace TwilioConsole.Accessors
{
    public class POEditorAccessor
    {
        public async Task<Dictionary<string, string>> GetTerms()
        {
            var url = @"https://api.poeditor.com/v2/terms/list";
            var httpMethod = HttpMethod.Post;
            var contentType = @"application/x-www-form-urlencoded";

            var httpRequest = System.Net.WebRequest.CreateHttp(url);
            httpRequest.Method = httpMethod.ToString().ToUpper();
            httpRequest.ContentType = contentType;
            string body = @"api_token=3c1ac9c7195ee55fbfbbc62a081198ce&id=295805&language=en-US";

            var data = System.Text.Encoding.ASCII.GetBytes(body);
            httpRequest.ContentLength = data.Length;
            using (var strm = httpRequest.GetRequestStream())
            {
                strm.Write(data, 0, data.Length);
            }

            var res = await httpRequest.GetResponseAsync();
            var resStream = res.GetResponseStream();

            if (resStream != null)
            {
                using (var streamReader = new StreamReader(resStream))
                {
                    var info = streamReader.ReadToEnd();
                    var parsedResult = JsonConvert.DeserializeObject<RootObject>(info);

                    Dictionary<string, string> result = new Dictionary<string, string>();
                    foreach (var item in parsedResult.result.terms)
                        result.Add(item.term, item.translation.content);

                    return result;
                }
            }

            return null;
        }
    }
}
