using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class PlayerEventStats
    {
        public long PlayerId { get; set; }

        public long GameEventId { get; set; }

        public long TeamId { get; set; }

        public int ForeignPlayerId { get; set; }

        public SportType Sport { get; set; }

        public int ForeignGameEventId { get; set; }

        public string ForeignTeamId { get; set; }

        public ValuePair[] Data { get; set; }

        public PlayerDataType DataType { get; set; }

        public string ForeignPlayerName { get; set; }
    }
}
