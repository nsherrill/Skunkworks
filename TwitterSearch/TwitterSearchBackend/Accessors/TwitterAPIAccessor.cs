using CoreTweet;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend.Accessors
{
    public interface ITwitterApiAccessor
    {
        TweetModel[] SearchForTweets(string searchText);
    }

    public class TwitterAPIAccessor : ITwitterApiAccessor
    {
        private string apiKey = "SLhi3WzsmiNf4uLJ30g4MvEKj";
        private string apiSecret = "yDnSD4oGn3taam8fNTHctvz4GVXn97SiRej0E20CWE5aIxvgiH";
        private string token = "39583056-tEZvPlELK2u6cNHz1ve92xLNIHid9kvd29ARWpD9W";
        private string tokenSecret = "3fksc6UWituz45TcGmmo0U8aymzXN1TmavxPirctk9J7W";

        public TweetModel[] SearchForTweets(string searchText)
        {
            var service = OAuth.Authorize(apiKey, apiSecret);
            var tempuri = service.AuthorizeUri;
            var temp = OAuth.GetTokens(service, "0969215");
            var result = temp.Search.Tweets(searchText);

            //var twitterService = new TwitterService(apiKey, apiSecret);
            //twitterService.AuthenticateWith(token, tokenSecret);

            //var searchOptions = new SearchOptions()
            //{
            //    Q = searchText,
            //    Count = 100,
            //};
            //var result = twitterService.Search(searchOptions);

            //List<TweetModel> results = new List<TweetModel>();
            //if (result != null)
            //    foreach (var item in result..Statuses)
            //    {
            //        results.Add(new TweetModel(item.Text, item.User.Name));
            //    }

            //return results.ToArray();
            return null;
        }
    }
}
