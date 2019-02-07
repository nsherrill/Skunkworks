using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSearchBackend;
using TwitterSearchBackend.Managers;

namespace TwitterTests.Mocks
{
    public class ManagerMock_TwitterManager : TwitterSearchServiceBase, ITwitterManager
    {
        public TweetArrayResult Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return new TweetArrayResult() { Error = ErrorCodes.InvalidSearchParameter };
            if (searchText.Contains("null"))
                return null;
            if (searchText.Contains("empty"))
                return new TweetArrayResult() { Items = new TweetContract[] { } };

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
