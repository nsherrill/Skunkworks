using RFKBackend.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    [Authorize]
    public class CamperController : Controller
    {
        CamperManager volMgr = new CamperManager();

        public ActionResult Index()
        {
            var result = volMgr.FindAllCampers();

            return View(result);
        }

        public ActionResult Create()
        {
            var result = volMgr.CreateNewCamper();
            return View(result);
        }

        public ActionResult Details(int id)
        {
            var result = volMgr.FindCamper(id);
            if (result != null)
                return View(result);

            return Create();
        }
    }
}