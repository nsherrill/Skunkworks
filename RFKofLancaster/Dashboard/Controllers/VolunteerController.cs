using Dashboard.Models;
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
    public class VolunteerController : Controller
    {
        VolunteerManager volMgr = new VolunteerManager();

        public ActionResult Index()
        {
            var result = volMgr.FindAllVolunteers();

            List<VolunteerModel> models = new List<VolunteerModel>();
            result.ToList().ForEach(v => models.Add(new VolunteerModel(v)));

            return View(models.ToArray());
        }

        public ActionResult Create()
        {
            var result = volMgr.CreateNewVolunteer();
            return View(result);
        }

        public ActionResult Details(int id)
        {
            var result = volMgr.FindVolunteer(id);
            if (result != null)
                return View(result);

            return Create();
        }

        [HttpPost]
        public VolunteerSnapshot ToggleVerbal(int id, bool shouldBeOn, int year)
        {
            volMgr.ToggleVerbal(id, shouldBeOn, year);

            return volMgr.FindVolunteer(id);
        }

        [HttpPost]
        public VolunteerSnapshot ToggleApplication(int id, bool shouldBeOn, int year)
        {
            volMgr.ToggleApplication(id, shouldBeOn, year);

            return volMgr.FindVolunteer(id);
        }
    }
}