using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class FutureGameEvent
    {
        public string ForeignId { get; set; }

        public SportType Sport { get; set; }

        public DateTime Date { get; set; }

        public Team[] Teams { get; set; }

        public string Stadium { get; set; }

        public double? Weather_HighDegrees { get; set; }
        public double? Weather_LowDegrees { get; set; }

        public Referee[] Referees { get; set; }

        public double? PrecipitationChance { get; set; }

        public PlayerEventStats[] StartingPitchers { get; set; }
    }
}
