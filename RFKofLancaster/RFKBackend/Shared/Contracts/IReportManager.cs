using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.Contracts
{
    public interface IReportManager
    {
        ReportDataCollection ExecuteReport(ReportType reportType, int year, string optionalParams = null);
    }
}
