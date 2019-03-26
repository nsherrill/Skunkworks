using RFKBackend.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RFKBackend.Shared.DataContracts
{
    public class ReportCollection
    {
        public ReportModel[] Reports { get; set; }
    }
    public class ReportModel
    {
        public string ReportName { get; set; }
        public ReportType ReportType { get; set; }
        public int Year { get; set; }
    }
}