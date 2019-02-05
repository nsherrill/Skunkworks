using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSearchBackend;
using TwitterSearchBackend.Accessors;

namespace TwitterTests.Mocks
{
    public class AccessorMock_TwitterAPIAccessor : ITwitterApiAccessor
    {
        public TweetModel[] SearchForTweets(string searchText)
        {
            if (searchText.Contains("null"))
                return null;
            if (searchText.Contains("empty"))
                return new TweetModel[] { };

            List<TweetModel> result = new List<TweetModel>();
            for (int i = 0; i < 10; i++)
            {
                result.Add(new TweetModel()
                {
                    Text = Guid.NewGuid().ToString() + " " + searchText + " " + Guid.NewGuid().ToString(),
                    UserName = Guid.NewGuid().ToString(),
                });
            }

            return result.ToArray();
        }
    }
}
