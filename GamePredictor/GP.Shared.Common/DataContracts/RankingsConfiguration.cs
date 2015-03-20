using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Shared.Common
{
    public class RankingsConfiguration
    {
        public double ConservativePercent { get; set; }
        public double AggressivePercent { get; set; }

        public ConfigType GetConfigType(int index, int max)
        {
            ConfigType result = ConfigType.Aggressive;
            if ((double)index / (double)max <
                this.ConservativePercent / 100.0)
            {
                result = ConfigType.Conservative;
            }
            else if ((double)index / (double)max <
                (this.ConservativePercent + this.AggressivePercent) / 100.0)
            {
                result = ConfigType.Aggressive;
            }

            return result;
        }
    }
}
