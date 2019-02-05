using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TwitterSearchBackend;
using TwitterSearchBackend.Managers;

namespace TwitterSearch.Controllers
{
    public class HomeController : Controller
    {
        ITwitterManager twitterManager = null;

        public ActionResult Index()
        {
            return View();
        }

        public TweetModel[] Search(string textToSearch)
        {
            if (twitterManager == null)
                twitterManager = new TwitterManager();

            (twitterManager as TwitterManager).twitterAccessor = new TwitterTests.Mocks.AccessorMock_TwitterAPIAccessor();

            TweetModel[] result = null;
            try
            {
                result = twitterManager.Search(textToSearch);
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Exception caught searching for {0}", textToSearch), e);
            }
            return result;
        }
    }
}
