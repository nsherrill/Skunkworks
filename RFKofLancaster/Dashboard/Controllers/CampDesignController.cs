using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    [Authorize]
    public class CampDesignController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}