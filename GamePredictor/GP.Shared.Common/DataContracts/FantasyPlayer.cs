using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class FantasyPlayer : Player
    {
        public string ForeignLeagueId { get; set; }

        public double Value { get; set; }

        public BaseballPosition Position { get; set; }

        public double PPG { get; set; }

        public int GamesPlayed { get; set; }
        
        public string TeamAbr{ get; set; }

        public bool IsHome { get; set; }
    }
}
