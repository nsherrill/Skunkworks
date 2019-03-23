using Dashboard.Models;
using RFKBackend.Managers;
using RFKBackend.Shared;
using RFKBackend.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        IReportManager reportManager = new ReportManager();

        public ActionResult Index()
        {
            var reportModel = new ReportModel();
            List<string> reports = new List<string>();

            var methods = typeof(ReportsController).GetMethods();
            var goodMethods = methods.Where(m => m.Name != "Index" && m.ReturnType.Name == "ActionResult");
            reports.AddRange(goodMethods.Select(m => m.Name));
            reportModel.Reports = reports.ToArray();

            return View(reportModel);
        }

        public ActionResult StaffSheet(int year = 2019)
        {
            var data = reportManager.ExecuteReport(ReportType.StaffSheet, year);

            return View(data);
        }
    }
}