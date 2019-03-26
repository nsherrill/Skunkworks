using RFKBackend.Accessors;
using RFKBackend.Shared;
using RFKBackend.Shared.Contracts;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Managers
{
    public class ReportManager : IReportManager
    {
        private IReportDataAccessor reportAccessor;

        public ReportManager()
        {
            this.reportAccessor = new ReportDataAccessor();
        }

        public ReportDataCollection ExecuteReport(ReportType reportType, int year, string optionalParams = null)
        {
            return this.reportAccessor.ExecuteReport(reportType, year, optionalParams);
        }

        public ReportModel[] GenerateModels(string report)
        {
            List<ReportModel> result = new List<ReportModel>();

            var currYear = RFKUtilities.DetermineCurrentYear();

            for (int i = 0; i < 4; i++)
            {
                ReportType myReportType = (ReportType)Enum.Parse(typeof(ReportType), report, true);
                result.Add(new ReportModel()
                {
                    ReportName = myReportType.GetDescription() ?? myReportType.ToString(),
                    ReportType = myReportType,
                    Year = (currYear) + 1 - i,
                });
            }
            return result.ToArray();
        }
    }
}
