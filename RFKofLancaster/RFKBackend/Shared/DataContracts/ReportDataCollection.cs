using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class ReportDataCollection
    {
        public int Year { get; set; }
        public ReportData[] Data { get; set; }
    }
}
