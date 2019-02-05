using System;
using System.Collections.Generic;
using System.Text;
using TweetSharp;

namespace TwitterSearchBackend.Accessors
{
    public interface ITwitterApiAccessor
    {
        TweetModel[] SearchForTweets(string searchText);
    }

    public class TwitterAPIAccessor : ITwitterApiAccessor
    {
        private string apiKey = "";
        private string apiSecret = "";
        private string token = "";
        private string tokenSecret = "";

        public TweetModel[] SearchForTweets(string searchText)
        {
            var twitterService = new TwitterService(apiKey, apiSecret);
            twitterService.AuthenticateWith(token, tokenSecret);

            var searchOptions = new SearchOptions()
            {
                Q = searchText,
            };
            var result = twitterService.Search(searchOptions);

            List<TweetModel> results = new List<TweetModel>();
            if (result != null)
                foreach (var item in result.Statuses)
                {
                    results.Add(new TweetModel(item.Text, item.User.Name));
                }

            return results.ToArray();
        }
    }
}
