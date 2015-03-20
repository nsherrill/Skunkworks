using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class FantasyRoster
    {
        public FantasyPlayerRanking[] PlayersToSelect { get; set; }

        public long ForeignLeagueId { get; set; }
    }
}
