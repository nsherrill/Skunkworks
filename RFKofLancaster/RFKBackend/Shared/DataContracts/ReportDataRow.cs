using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class ReportDataRow
    {
        public string[] Headers { get; set; }
        public string[] Values { get; set; }
    }
}
