using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterSearchBackend;
using TwitterSearchBackend.Engines;

namespace TwitterTests.EngineTests
{
    [TestClass]
    public class ValidationEngineTests
    {
        [TestMethod]
        public void ValidationEngineTests_TestMe()
        {
            var eng = new ValidationEngine();

            var results = eng.TestMe();

            Assert.IsFalse(string.IsNullOrEmpty(results));
            Assert.AreEqual("TwitterSearchBackend.Engines.ValidationEngine", results);
        }

        [TestMethod]
        public void ValidationEngineTests_Success()
        {
            var eng = new ValidationEngine();

            var results = eng.ValidateSearchText("norm");

            Assert.IsTrue(results != null);
            Assert.IsTrue(results.IsValid);
            Assert.IsTrue(string.IsNullOrEmpty(results.Error));
        }

        #region bads
        [TestMethod]
        public void ValidationEngineTests_HandleEmpty()
        {
            var eng = new ValidationEngine();

            var results = eng.ValidateSearchText("");

            Assert.IsTrue(results != null);
            Assert.IsFalse(results.IsValid);
            Assert.AreEqual(ErrorCodes.InvalidSearchParameter, results.Error);
        }

        [TestMethod]
        public void ValidationEngineTests_HandleNull()
        {
            var eng = new ValidationEngine();

            var results = eng.ValidateSearchText(null);

            Assert.IsTrue(results != null);
            Assert.IsFalse(results.IsValid);
            Assert.AreEqual(ErrorCodes.InvalidSearchParameter, results.Error);
        }
        #endregion
    }
}
