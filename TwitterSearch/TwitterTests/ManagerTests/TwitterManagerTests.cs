using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterSearchBackend.Managers;

namespace TwitterTests.ManagerTests
{
    [TestClass]
    public class TwitterManagerTests
    {
        [TestMethod]
        public void TwitterManagerTests_Search_Standard()
        {
            var manager = new TwitterManager();
            manager.twitterAccessor = new Mocks.AccessorMock_TwitterAPIAccessor();

            var results = manager.Search("norm");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Length > 0);
        }

        #region bads
        [TestMethod]
        public void TwitterManagerTests_Search_HandleNullResults()
        {
            var manager = new TwitterManager();
            manager.twitterAccessor = new Mocks.AccessorMock_TwitterAPIAccessor();

            var results = manager.Search("null");

            Assert.IsTrue(results == null);
        }

        [TestMethod]
        public void TwitterManagerTests_Search_HandleEmptyResults()
        {
            var manager = new TwitterManager();
            manager.twitterAccessor = new Mocks.AccessorMock_TwitterAPIAccessor();

            var results = manager.Search("empty");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Length == 0);
        }

        [TestMethod]
        public void TwitterManagerTests_Search_HandleEmptySearch()
        {
            var manager = new TwitterManager();
            manager.twitterAccessor = new Mocks.AccessorMock_TwitterAPIAccessor();

            var results = manager.Search("");

            Assert.IsTrue(results == null);
        }

        [TestMethod]
        public void TwitterManagerTests_Search_HandleNullSearch()
        {
            var manager = new TwitterManager();
            manager.twitterAccessor = new Mocks.AccessorMock_TwitterAPIAccessor();

            var results = manager.Search(null);

            Assert.IsTrue(results == null);
        }
        #endregion
    }
}
