using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSearchBackend;
using TwitterSearchBackend.Managers;

namespace TwitterTests.Mocks
{
    public class ManagerMock_TwitterManager : ITwitterManager
    {
        public TweetContract[] Search(string searchText)
        {
            if (searchText.Contains("null"))
                return null;
            if (searchText.Contains("empty"))
                return new TweetContract[] { };

            List<TweetContract> result = new List<TweetContract>();
            for (int i = 0; i < 10; i++)
            {
                result.Add(new TweetContract()
                {
                    Text = Guid.NewGuid().ToString() + " " + searchText + " " + Guid.NewGuid().ToString(),
                    UserName = Guid.NewGuid().ToString(),
                });
            }

            return result.ToArray();
        }
    }
}
