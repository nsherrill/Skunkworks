using RFKBackend.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    [Authorize]
    public class CamperController : RFKController
    {
        CamperManager volMgr = new CamperManager();

        public ActionResult Index()
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            var result = volMgr.FindAllCampers();

            return View(result);
        }

        public ActionResult Create()
        {
            base.SetMyUser();
            if (!base.MyUser.CanWrite)
                return RedirectToAction("Index", "Home", null);

            var result = volMgr.CreateNewCamper();
            return View(result);
        }

        public ActionResult Details(int id)
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            var result = volMgr.FindCamper(id);
            if (result != null)
                return View(result);

            return Create();
        }
    }
}