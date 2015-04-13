using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class FantasyPlayerRanking : FantasyPlayer
    {
        public double NormCalc { get; set; }

        public int Hits { get; set; }

        public string TeamName { get; set; }

        public int HRs { get; set; }

        public double AVG { get; set; }

        public double ERA { get; set; }

        public bool IsHome { get; set; }
    }
}
