using Dashboard.Models;
using RFKBackend;
using RFKBackend.Managers;
using RFKBackend.Shared.Contracts;
using RFKBackend.Shared.DataContracts;
using RFKBackend.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    [Authorize]
    public class VolunteerController : RFKController
    {
        IVolunteerManager volMgr = new VolunteerManager();

        public VolunteerController() : base()
        {

        }

        public ActionResult Index(string userMessage = null)
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            var result = volMgr.FindAllVolunteers();

            List<VolunteerModel> models = new List<VolunteerModel>();
            result.ToList().ForEach(v => models.Add(new VolunteerModel(v)));

            return View(models.ToArray());
        }

        [HttpPost]
        public ActionResult Create(VolunteerModel createModel)
        {
            base.SetMyUser();
            if (!base.MyUser.CanWrite)
                return RedirectToAction("Index", "Home", null);

            ViewBag.UserMessage = string.Empty;

            var saveResult = volMgr.SaveVolunteer(createModel);
            if (saveResult?.IsSuccess ?? false)
            {
                ViewBag.UserMessage = Messages.CreateVolunteer_Success(saveResult.Data.Name);
                return RedirectToAction("Index");
            }

            ViewBag.UserMessage = saveResult?.ErrorMessage ?? Messages.CreateVolunteer_Fail();
            return View(createModel);
        }

        public ActionResult Create()
        {
            base.SetMyUser();
            if (!base.MyUser.CanWrite)
                return RedirectToAction("Index", "Home", null);

            var result = volMgr.CreateNewVolunteer();
            return View(new VolunteerModel(result));
        }

        public ActionResult Details(int id)
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            var result = volMgr.FindVolunteer(id);
            if (result != null)
                return View(result);

            return Create();
        }

        [HttpPost]
        public ActionResult Details(VolunteerModel volunteerModelToSave)
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            throw new NotImplementedException();

            ViewBag.UserMessage = string.Empty;

            var saveResult = volMgr.SaveVolunteer(volunteerModelToSave);
            if (saveResult?.IsSuccess ?? false)
            {
                ViewBag.UserMessage = Messages.CreateVolunteer_Success(saveResult.Data.Name);
                return RedirectToAction("Index");
            }

            ViewBag.UserMessage = saveResult?.ErrorMessage ?? Messages.CreateVolunteer_Fail();
            return View(volunteerModelToSave);
        }

        [HttpPost]
        public VolunteerSnapshot ToggleVerbal(int id, bool shouldBeOn, int year)
        {
            base.SetMyUser();
            if (!base.MyUser.CanWrite)
                return null;

            volMgr.ToggleVerbal(id, shouldBeOn, year);

            return volMgr.FindVolunteer(id);
        }

        [HttpPost]
        public VolunteerSnapshot ToggleApplication(int id, bool shouldBeOn, int year)
        {
            base.SetMyUser();
            if (!base.MyUser.CanWrite)
                return null;

            volMgr.ToggleApplication(id, shouldBeOn, year);

            return volMgr.FindVolunteer(id);
        }

        [HttpPost]
        public VolunteerSnapshot AddRoleToUser(int id, int? roleId, int year)
        {
            base.SetMyUser();
            if (!base.MyUser.CanWrite)
                return null;

            volMgr.AddRoleToUser(id, roleId, year);

            return volMgr.FindVolunteer(id);
        }
    }
}