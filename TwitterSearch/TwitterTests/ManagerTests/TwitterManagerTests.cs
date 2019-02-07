using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterSearchBackend.Accessors;
using TwitterSearchBackend.Engines;
using TwitterSearchBackend.Managers;

namespace TwitterTests.ManagerTests
{
    [TestClass]
    public class TwitterManagerTests
    {
        [TestMethod]
        public void TwitterManagerTests_TestMe_WithMocks()
        {
            var manager = new TwitterManager();
            manager.FactoryOverride<IValidationEngine>(new Mocks.EngineMock_ValidationEngine());
            manager.FactoryOverride<ITwitterApiAccessor>(new Mocks.AccessorMock_TwitterAPIAccessor());

            var results = manager.TestMe();

            Assert.IsFalse(string.IsNullOrEmpty(results));
            Assert.AreEqual("TwitterTests.Mocks.EngineMock_ValidationEngine, TwitterTests.Mocks.AccessorMock_TwitterAPIAccessor, TwitterSearchBackend.Managers.TwitterManager", results);
        }
        [TestMethod]
        public void TwitterManagerTests_TestMe_NoMocks()
        {
            var manager = new TwitterManager();

            var results = manager.TestMe();

            Assert.IsFalse(string.IsNullOrEmpty(results));
            Assert.AreEqual("TwitterSearchBackend.Engines.ValidationEngine, TwitterSearchBackend.Accessors.TwitterAPIAccessor, TwitterSearchBackend.Managers.TwitterManager", results);
        }

        [TestMethod]
        public void TwitterManagerTests_Search_Standard()
        {
            var manager = new TwitterManager();
            manager.FactoryOverride<IValidationEngine>(new Mocks.EngineMock_ValidationEngine());
            manager.FactoryOverride<ITwitterApiAccessor>(new Mocks.AccessorMock_TwitterAPIAccessor());

            var results = manager.Search("norm");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Items != null);
            Assert.IsTrue(results.Items.Length > 0);
            Assert.IsTrue(string.IsNullOrEmpty(results.Error));
        }

        #region bads
        [TestMethod]
        public void TwitterManagerTests_Search_HandleNullResults()
        {
            var manager = new TwitterManager();
            manager.FactoryOverride<IValidationEngine>(new Mocks.EngineMock_ValidationEngine());
            manager.FactoryOverride<ITwitterApiAccessor>(new Mocks.AccessorMock_TwitterAPIAccessor());

            var results = manager.Search("null");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Items == null);
            Assert.IsTrue(string.IsNullOrEmpty(results.Error));
        }

        [TestMethod]
        public void TwitterManagerTests_Search_HandleEmptyResults()
        {
            var manager = new TwitterManager();
            manager.FactoryOverride<IValidationEngine>(new Mocks.EngineMock_ValidationEngine());
            manager.FactoryOverride<ITwitterApiAccessor>(new Mocks.AccessorMock_TwitterAPIAccessor());

            var results = manager.Search("empty");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Items != null);
            Assert.IsTrue(results.Items.Length == 0);
            Assert.IsTrue(string.IsNullOrEmpty(results.Error));
        }

        [TestMethod]
        public void TwitterManagerTests_Search_HandleErrorResults()
        {
            var manager = new TwitterManager();
            manager.FactoryOverride<IValidationEngine>(new Mocks.EngineMock_ValidationEngine());
            manager.FactoryOverride<ITwitterApiAccessor>(new Mocks.AccessorMock_TwitterAPIAccessor());

            var results = manager.Search("error");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Items == null);
            Assert.IsFalse(string.IsNullOrEmpty(results.Error));
        }

        [TestMethod]
        public void TwitterManagerTests_Search_HandleEmptySearch()
        {
            var manager = new TwitterManager();
            manager.FactoryOverride<IValidationEngine>(new Mocks.EngineMock_ValidationEngine());
            manager.FactoryOverride<ITwitterApiAccessor>(new Mocks.AccessorMock_TwitterAPIAccessor());

            var results = manager.Search("");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Items == null);
            Assert.IsFalse(string.IsNullOrEmpty(results.Error));
        }

        [TestMethod]
        public void TwitterManagerTests_Search_HandleNullSearch()
        {
            var manager = new TwitterManager();
            manager.FactoryOverride<IValidationEngine>(new Mocks.EngineMock_ValidationEngine());
            manager.FactoryOverride<ITwitterApiAccessor>(new Mocks.AccessorMock_TwitterAPIAccessor());

            var results = manager.Search(null);

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.Items == null);
            Assert.IsFalse(string.IsNullOrEmpty(results.Error));
        }
        #endregion
    }
}
