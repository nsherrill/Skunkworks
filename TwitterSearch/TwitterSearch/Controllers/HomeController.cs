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
        private ITwitterManager twitterMgr = null;

        public void FactoryOverride<T>(T service)
        {
            if (typeof(T) == typeof(ITwitterManager))
                this.twitterMgr = (ITwitterManager)service;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new SearchResultModel());
        }

        [HttpPost]
        public ActionResult Index(string textToSearch)
        {
            //Intended as a manager-passthrough, validation handled by manager
            try
            {
                Init();

                var tempResult = twitterMgr.Search(textToSearch);

                return View(new SearchResultModel()
                {
                    SearchText = textToSearch,
                    Items = tempResult.Items,
                    Error = tempResult.Error,
                });
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Exception caught hitting the manager for [{0}]", textToSearch), e);
                return View(new SearchResultModel()
                {
                    Error = ErrorCodes.ExceptionCaught(ErrorTextType.Public, e),
                    SearchText = textToSearch,
                });
            }
        }

        #region privates
        private void Init()
        {
            if (twitterMgr == null)
                twitterMgr = new TwitterManager();
        }
        #endregion
    }
}
