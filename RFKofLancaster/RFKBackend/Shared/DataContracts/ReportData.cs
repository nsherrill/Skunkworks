using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class ReportData
    {
        public string[] Headers { get; set; }
        public ReportDataRow[] Data { get; set; }
    }
}
