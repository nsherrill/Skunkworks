using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterSearch.Controllers;
using TwitterSearchBackend.Managers;

namespace TwitterTests.ControllerTests
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void HomeControllerTests_Index()
        {
            var controller = new HomeController();
            var result = controller.Index();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HomeControllerTests_Index_Standard()
        {
            var controller = new HomeController();
            controller.FactoryOverride<ITwitterManager>(new Mocks.ManagerMock_TwitterManager());

            var result = controller.Index("text");

            Assert.IsNotNull(result);
            Assert.IsTrue(result is System.Web.Mvc.ViewResult);
            Assert.IsTrue(((System.Web.Mvc.ViewResult)result).Model != null);
            Assert.IsTrue(((System.Web.Mvc.ViewResult)result).Model is TwitterSearch.Models.SearchResultModel);

            var typedResult = (TwitterSearch.Models.SearchResultModel)((System.Web.Mvc.ViewResult)result).Model;
            Assert.IsTrue(string.IsNullOrEmpty(typedResult.Error));
        }

        [TestMethod]
        public void HomeControllerTests_Index_empty()
        {
            var controller = new HomeController();
            controller.FactoryOverride<ITwitterManager>(new Mocks.ManagerMock_TwitterManager());

            var result = controller.Index("");

            Assert.IsNotNull(result);
            Assert.IsTrue(result is System.Web.Mvc.ViewResult);
            Assert.IsTrue(((System.Web.Mvc.ViewResult)result).Model != null);
            Assert.IsTrue(((System.Web.Mvc.ViewResult)result).Model is TwitterSearch.Models.SearchResultModel);

            var typedResult = (TwitterSearch.Models.SearchResultModel)((System.Web.Mvc.ViewResult)result).Model;
            Assert.IsTrue(typedResult.Error != null && typedResult.Error.Length > 0);
        }

        [TestMethod]
        public void HomeControllerTests_Index_null()
        {
            var controller = new HomeController();
            controller.FactoryOverride<ITwitterManager>(new Mocks.ManagerMock_TwitterManager());

            var result = controller.Index(null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is System.Web.Mvc.ViewResult);
            Assert.IsTrue(((System.Web.Mvc.ViewResult)result).Model != null);
            Assert.IsTrue(((System.Web.Mvc.ViewResult)result).Model is TwitterSearch.Models.SearchResultModel);

            var typedResult = (TwitterSearch.Models.SearchResultModel)((System.Web.Mvc.ViewResult)result).Model;
            Assert.IsTrue(typedResult.Error != null && typedResult.Error.Length > 0);
        }
    }
}
