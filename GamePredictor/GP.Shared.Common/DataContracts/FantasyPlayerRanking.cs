using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class FantasyPlayerRanking : FantasyPlayer
    {
        public string TeamName { get; set; }
        public int PlayerId { get; set; }

        public int HR { get; set; }
        public int AB { get; set; }
        public int Singles { get; set; }
        public int Doubles { get; set; }
        public int Triples { get; set; }
        public int RBI { get; set; }
        public int R { get; set; }
        public int SB { get; set; }
        public int BB { get; set; }
        public int HBP { get; set; }
        public double PPG { get; set; }
        public int ABLast7 { get; set; }
        public int HRLast7 { get; set; }
        public double AVGLast7 { get; set; }
        public double OBPLast7 { get; set; }
        public double WOBALast7 { get; set; }
        public double OPSLast7 { get; set; }
        public double PointsPerABLast7 { get; set; }

    }
}
