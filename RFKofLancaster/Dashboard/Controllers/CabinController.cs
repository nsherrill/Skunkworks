using RFKBackend.Managers;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    [Authorize]
    public class CabinController : RFKController
    {
        CabinManager cabMgr = new CabinManager();

        public ActionResult Index()
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            ViewBag.UserMessage = null;
            var result = cabMgr.FindAllCabins();

            return View(result);
        }

        public ActionResult Create(Cabin createdCabin = null)
        {
            base.SetMyUser();
            if (!base.MyUser.CanWrite)
                return RedirectToAction("Index", "Home", null);

            ViewBag.UserMessage = null;
            if (createdCabin != null
                && !string.IsNullOrEmpty(createdCabin.Name))
            {
                bool success = cabMgr.SaveCabin(createdCabin);
                if (success)
                    ViewBag.UserMessage = string.Format("{0} created successfully", createdCabin.Name);
                else
                    return View(createdCabin);
            }

            var result = cabMgr.CreateNewCabin();

            return View(result);
        }

        public ActionResult Details(int id)
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            ViewBag.UserMessage = null;
            var result = cabMgr.FindCabin(id);
            if (result != null)
                return View(result);

            return Create();
        }
    }
}