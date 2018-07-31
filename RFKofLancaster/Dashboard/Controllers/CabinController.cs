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
    public class CabinController : Controller
    {
        CabinManager cabMgr = new CabinManager();

        public ActionResult Index()
        {
            ViewBag.UserMessage = null;
            var result = cabMgr.FindAllCabins();

            return View(result);
        }

        public ActionResult Create(Cabin createdCabin = null)
        {
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
            ViewBag.UserMessage = null;
            var result = cabMgr.FindCabin(id);
            if (result != null)
                return View(result);

            return Create();
        }
    }
}