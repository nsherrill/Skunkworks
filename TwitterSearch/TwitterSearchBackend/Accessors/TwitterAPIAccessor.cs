using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TwitterSearchBackend.Accessors
{
    public interface ITwitterApiAccessor : ITwitterSearchServiceBase
    {
        TweetArrayResult SearchForTweets(string searchText);
    }

    public class TwitterAPIAccessor : TwitterSearchServiceBase, ITwitterApiAccessor
    {
        public TweetArrayResult SearchForTweets(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return new TweetArrayResult() { Error = ErrorCodes.InvalidSearchParameter };

            List<TweetContract> results = new List<TweetContract>();

            try
            {
                VerifyCredentials();

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
                string errorText = string.Format("Exception caught hitting Twitter API for [{0}]", searchText);
                Logger.Error(errorText, e);
                return new TweetArrayResult()
                {
                    Error = ErrorCodes.ExceptionCaught(ErrorTextType.Internal, e)
                };
            }

            return new TweetArrayResult()
            {
                Items = results.ToArray()
            };
        }

        #region privates
        private bool credentialsSet = false;

        private void VerifyCredentials()
        {
            if (!credentialsSet)
            {
                var creds = Tweetinvi.Auth.CreateCredentials(
                    ConfigHelper.OAuth_ConsumerKey,
                    ConfigHelper.OAuth_ConsumerSecret,
                    ConfigHelper.OAuth_UserToken,
                    ConfigHelper.OAuth_UserTokenSecret);
                Tweetinvi.Auth.SetCredentials(creds);
                credentialsSet = true;
            }
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
        #endregion
    }
}
