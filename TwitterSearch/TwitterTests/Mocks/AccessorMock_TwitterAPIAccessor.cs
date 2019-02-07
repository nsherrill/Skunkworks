using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSearchBackend;
using TwitterSearchBackend.Accessors;

namespace TwitterTests.Mocks
{
    public class AccessorMock_TwitterAPIAccessor : TwitterSearchServiceBase, ITwitterApiAccessor
    {
        public TweetArrayResult SearchForTweets(string searchText)
        {
            if (searchText.Contains("null"))
                return new TweetArrayResult() { Items = null };
            if (searchText.Contains("empty"))
                return new TweetArrayResult() { Items = new TweetContract[] { } };
            if (searchText.Contains("error"))
                return new TweetArrayResult() { Error = "ERROR!!" };

            List<TweetContract> result = new List<TweetContract>();
            for (int i = 0; i < 10; i++)
            {
                result.Add(new TweetContract()
                {
                    Text = Guid.NewGuid().ToString() + " " + searchText + " " + Guid.NewGuid().ToString(),
                    UserName = Guid.NewGuid().ToString(),
                });
            }

            return new TweetArrayResult() { Items = result.ToArray() };
        }
    }
}
