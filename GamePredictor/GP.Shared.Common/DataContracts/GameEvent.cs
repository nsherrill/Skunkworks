using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class GameEvent
    {
        public long Id { get; set; }

        public string ForeignId { get; set; }

        public SportType Sport { get; set; }

        public DateTime Date { get; set; }

        public Team[] Teams { get; set; }

        public PlayerEventStats[] PlayerStats { get; set; }

        public string Stadium { get; set; }

        public int Attendence { get; set; }

        public double? Weather_Degrees { get; set; }

        public double WindSpeed { get; set; }

        public Referee[] Referees { get; set; }

        public WeatherType Weather_Type { get; set; }

        public string GameNotes { get; set; }
    }
}
