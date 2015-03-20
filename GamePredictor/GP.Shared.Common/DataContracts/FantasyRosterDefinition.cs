using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class FantasyRosterDefinition
    {
        public string ForeignId { get; set; }

        public double SalaryCap { get; set; }

        public int StartingP { get; set; }
        public int Starting1B { get; set; }
        public int Starting2B { get; set; }
        public int Starting3B { get; set; }
        public int StartingC { get; set; }
        public int StartingSS { get; set; }
        public int StartingOF { get; set; }
    }
}
