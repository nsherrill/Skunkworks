using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TwitterSearchBackend.Accessors
{
    public interface ITwitterApiAccessor
    {
        TweetContract[] SearchForTweets(string searchText);
    }

    public class TwitterAPIAccessor : ITwitterApiAccessor
    {
        private string URL_FORMATTER { get { return "https://api.twitter.com/1.1/search/tweets.json?q={0}&result_type=popular"; } }
        private string SIGNATURE_FORMATTER
        {
            get
            {
                return @"oauth_consumer_key={1}&oauth_nonce={2}&oauth_signature_method={3}&oauth_timestamp={4}&oauth_token={0}&oauth_version=1.0";
            }
        }
        private string HEADER_FORMATTER
        {
            get
            {
                return @"oauth_consumer_key=""{1}"", oauth_nonce=""{2}"", oauth_signature=""{3}"", oauth_signature_method=""{4}"", oauth_timestamp=""{5}"", oauth_token=""{0}"", oauth_version=""1.0""";
            }
        }

        private string oauth_consumer_key = "SLhi3WzsmiNf4uLJ30g4MvEKj";
        private string oauth_consumer_secret = "yDnSD4oGn3taam8fNTHctvz4GVXn97SiRej0E20CWE5aIxvgiH";

        private string oauth_user_token = "39583056-tEZvPlELK2u6cNHz1ve92xLNIHid9kvd29ARWpD9W";
        private string oauth_user_token_secret = "3fksc6UWituz45TcGmmo0U8aymzXN1TmavxPirctk9J7W";

        public TweetContract[] SearchForTweets(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return null;

            List<TweetContract> results = new List<TweetContract>();

            try
            {
                var creds = Tweetinvi.Auth.CreateCredentials(
                    oauth_consumer_key, oauth_consumer_secret, oauth_user_token, oauth_user_token_secret);

                Tweetinvi.Auth.SetCredentials(creds);

                var tweetResults = Tweetinvi.Search.SearchTweets(searchText);

                if (tweetResults != null)
                {
                    foreach (var item in tweetResults)
                    {
                        results.Add(new TweetContract(item.FullText, item.CreatedBy.ScreenName, item.CreatedAt));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Exception caught hitting Twitter API for [{0}]", searchText), e);
            }

            return results.ToArray();
        }

        private string PercentEncode(string source)
        {
            var result = Uri.EscapeDataString(source);
            return result;
        }

        private double SecondsFromEpoch(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
