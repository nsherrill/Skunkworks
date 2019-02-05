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
    }
}
