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
        ITwitterManager twitterManager = null;

        [HttpGet]
        public ActionResult Index()
        {
            return View(new SearchResultModel());
        }

        [HttpPost]
        public ActionResult Index(string textToSearch)
        {
            //string textToSearch = id;
            if (string.IsNullOrEmpty(textToSearch))
                return View();

            if (twitterManager == null)
                twitterManager = new TwitterManager();

            List<TweetModel> result = new List<TweetModel>();
            try
            {
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
