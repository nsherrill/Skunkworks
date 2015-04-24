using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class CurrentPlayerStats
    {
        public long PlayerId { get; set; }

        public long TeamId { get; set; }

        public SportType Sport { get; set; }

        public string ForeignTeamId { get; set; }

        public ValuePair[] Data { get; set; }

        public PlayerDataType DataType { get; set; }

        public string ForeignPlayerName { get; set; }

        public string SessionId { get; set; }
    }
}
