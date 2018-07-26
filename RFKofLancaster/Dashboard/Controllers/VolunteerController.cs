using Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class VolunteerController : Controller
    {
        // GET: Volunteer
        public ActionResult Index()
        {
            List<VolunteerModel> result = new List<VolunteerModel>();
            result.Add(new VolunteerModel()
            {
                VolunteerId = 1,
                Name = "Nick",
                NickName = "Nick"
            });
            result.Add(new VolunteerModel()
            {
                VolunteerId = 2,
                Name = "Kevin",
                NickName = "Kevin"
            });
            return View(result.ToArray());
        }
    }
}