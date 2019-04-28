using Dashboard.Models;
using RFKBackend;
using RFKBackend.Managers;
using RFKBackend.Shared.Contracts;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    [Authorize]
    public class YearConfigController : RFKController
    {
        IVolunteerManager volMgr = new VolunteerManager();
        CamperManager camperMgr = new CamperManager();

        public ActionResult Index(int thisYear = 2019)
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            var result = new YearConfigModel(thisYear);

            result.RoleCounts = volMgr.GetRoleCounts(thisYear);

            return View(result);
        }

        [HttpPost]
        public bool AdjustRoleCount(int year, int roleId, int delta)
        {
            base.SetMyUser();
            if (!base.MyUser.CanWrite)
                return false;

            try
            {
                volMgr.AdjustRoleCount(year, roleId, delta);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}