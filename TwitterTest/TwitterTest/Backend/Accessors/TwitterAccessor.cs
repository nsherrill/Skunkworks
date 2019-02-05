using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterTest.Backend.Accessors
{
    public interface ITwitterAccessor
    {
        TweetModel[] SearchForTweets(string searchText);
    }

    public class TwitterAccessor : GenericWebAccessor, ITwitterAccessor
    {
        private string URL_FORMATTER { get { return ""; } }

        public TweetModel[] SearchForTweets(string searchText)
        {
            var encodedSearchText = System.Net.WebUtility.HtmlEncode(searchText);

            string url = string.Format(URL_FORMATTER, encodedSearchText);
            TweetModel[] result = base.PostToUrl<TweetModel>(url, ParseModel);

            return result;
        }

        private TweetModel ParseModel(string json)
        {
            var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
            var result = new TweetModel()
            {
                UserName = (string)obj["UserName"],
            };
            return result;
        }
    }
}