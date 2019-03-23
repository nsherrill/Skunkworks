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
    }
}
