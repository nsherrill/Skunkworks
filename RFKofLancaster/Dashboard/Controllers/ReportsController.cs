using Dashboard.Models;
using RFKBackend.Managers;
using RFKBackend.Shared;
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
    public class ReportsController : RFKController
    {
        IReportManager reportManager = new ReportManager();
        IVolunteerManager volunteerManager = new VolunteerManager();

        public ActionResult Index()
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            List<ReportModel> allModels = new List<ReportModel>();
            List<string> reports = new List<string>();

            var methods = typeof(ReportsController).GetMethods();
            var goodMethods = methods.Where(m => m.Name != "Index" && m.ReturnType.Name == "ActionResult");
            reports.AddRange(goodMethods.Select(m => m.Name));
            foreach (var report in reports)
                allModels.AddRange(reportManager.GenerateModels(report));

            ReportCollection result = new ReportCollection()
            {
                Reports = allModels.ToArray(),
            };

            return View(result);
        }

        public ActionResult StaffSheet(int year = 2019)
        {
            base.SetMyUser();
            if (!base.MyUser.CanRead)
                return RedirectToAction("Index", "Home", null);

            var data = reportManager.ExecuteReport(ReportType.StaffSheet, year);

            List<SelectListItem> roleList = new List<SelectListItem>();
            var allRoles = volunteerManager.GetAllRoles();
            roleList.Add(new SelectListItem() { Value = "", Text = "", Selected = true });
            allRoles.ToList().ForEach(r => roleList.Add(new SelectListItem()
            {
                Text = r.DisplayName,
                Value = r.RoleId.ToString(),
            }));
            ViewData.Add("RoleList", roleList);
            return View(data);
        }
    }
}