using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterSearchBackend.Accessors;

namespace TwitterTests.AccessorTests
{
    [TestClass]
    public class TwitterAPIAccessorTests
    {
        [TestMethod]
        public void TwitterAPIAccessorTests_SearchForTweets()
        {
            var twitterAcc = new TwitterAPIAccessor();

            var results = twitterAcc.SearchForTweets("norm");

            Assert.IsTrue(results != null);
        }

        [TestMethod]
        public void TwitterAPIAccessorTests_SearchForTweets_empty()
        {
            var twitterAcc = new TwitterAPIAccessor();

            var results = twitterAcc.SearchForTweets("");

            Assert.IsTrue(results == null);
        }

        [TestMethod]
        public void TwitterAPIAccessorTests_SearchForTweets_null()
        {
            var twitterAcc = new TwitterAPIAccessor();

            var results = twitterAcc.SearchForTweets(null);

            Assert.IsTrue(results == null);
        }
    }
}
