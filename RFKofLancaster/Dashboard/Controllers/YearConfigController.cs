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
    public class YearConfigController : Controller
    {
        IVolunteerManager volMgr = new VolunteerManager();
        CamperManager camperMgr = new CamperManager();

        public ActionResult Index(int thisYear = 2019)
        {
            var result = new YearConfigModel(thisYear);

            result.RoleCounts = volMgr.GetRoleCounts(thisYear);

            return View(result);
        }

        [HttpPost]
        public bool AdjustRoleCount(int year, int roleId, int delta)
        {
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