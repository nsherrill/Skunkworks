using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterSearchBackend.Accessors;

namespace TwitterTests.AccessorTests
{
    [TestClass]
    public class TwitterAPIAccessorTests
    {
        [TestMethod]
        public void TwitterAPIAccessorTests_TestMe()
        {
            var twitterAcc = new TwitterAPIAccessor();

            var result = twitterAcc.TestMe();

            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreEqual("TwitterSearchBackend.Accessors.TwitterAPIAccessor", result);
        }

        [TestMethod]
        public void TwitterAPIAccessorTests_SearchForTweets()
        {
            var twitterAcc = new TwitterAPIAccessor();

            var results = twitterAcc.SearchForTweets("norm");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Items != null);
            Assert.IsTrue(string.IsNullOrEmpty(results.Error));
        }

        #region bads
        [TestMethod]
        public void TwitterAPIAccessorTests_SearchForTweets_empty()
        {
            var twitterAcc = new TwitterAPIAccessor();

            var results = twitterAcc.SearchForTweets("");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Items == null);
            Assert.IsFalse(string.IsNullOrEmpty(results.Error));
        }

        [TestMethod]
        public void TwitterAPIAccessorTests_SearchForTweets_null()
        {
            var twitterAcc = new TwitterAPIAccessor();

            var results = twitterAcc.SearchForTweets(null);

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Items == null);
            Assert.IsFalse(string.IsNullOrEmpty(results.Error));
        }
        #endregion
    }
}
