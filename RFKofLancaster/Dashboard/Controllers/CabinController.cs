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
            var result = cabMgr.FindAllCabins();

            return View(result);
        }

        public ActionResult Create()
        {
            var result = cabMgr.CreateNewCabin();
            return View(result);
        }

        public ActionResult Details(int id)
        {
            var result = cabMgr.FindCabin(id);
            if (result != null)
                return View(result);

            return Create();
        }
    }
}