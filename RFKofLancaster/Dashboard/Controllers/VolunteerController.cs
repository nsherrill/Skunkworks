using Dashboard.Models;
using RFKBackend.Managers;
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
    }
}