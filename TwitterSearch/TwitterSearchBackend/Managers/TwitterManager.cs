using System;
using System.Collections.Generic;
using System.Text;
using TwitterSearchBackend.Accessors;

namespace TwitterSearchBackend.Managers
{
    public interface ITwitterManager
    {
        TweetContract[] Search(string textToSearch);
    }

    public class TwitterManager : ITwitterManager
    {
        public ITwitterApiAccessor twitterAccessor { get; set; }

        public TweetContract[] Search(string textToSearch)
        {
            if (string.IsNullOrEmpty(textToSearch))
            {
                Logger.Log("null text to Search");
                return null;
            }

            if (twitterAccessor == null)
                twitterAccessor = new TwitterAPIAccessor();
            TweetContract[] result = null;

            result = twitterAccessor.SearchForTweets(textToSearch);

            return result;
        }
    }
}
