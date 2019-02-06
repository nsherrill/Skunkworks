using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TwitterSearch.Models;
using TwitterSearchBackend;
using TwitterSearchBackend.Managers;

namespace TwitterSearch.Controllers
{
    public class HomeController : Controller
    {
        public ITwitterManager twitterManager = null;

        [HttpGet]
        public ActionResult Index()
        {
            return View(new SearchResultModel());
        }

        [HttpPost]
        public ActionResult Index(string textToSearch)
        {
            if (string.IsNullOrEmpty(textToSearch))
                return View(new SearchResultModel()
                {
                    Error = "Please specify a valid search parameter"
                });

            List<TweetModel> result = new List<TweetModel>();
            try
            {
                if (twitterManager == null)
                    twitterManager = new TwitterManager();

                var tempResult = twitterManager.Search(textToSearch);
                foreach (var item in tempResult)
                {
                    result.Add(new TweetModel(item));
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Exception caught searching for {0}", textToSearch), e);
                return View(new SearchResultModel()
                {
                    Error = "Exception caught, please try again",
                    SearchText = textToSearch,
                });
            }

            return View(new SearchResultModel()
            {
                SearchText = textToSearch,
                Items = result.ToArray()
            });
        }
    }
}
