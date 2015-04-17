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

        private Random random = new Random(DateTime.Now.Millisecond);

        public ConfigType GetConfigType(int index, int max)
        {
            ConfigType result = ConfigType.Aggressive;
            if ((double)index / (double)max <
                this.ConservativePercent / 100.0)
            {
                var which = random.Next(1000) % 3;
                switch (which)
                {
                    case 0: return ConfigType.Conservative;
                    case 1: return ConfigType.Conservative_MostExpensive;
                    case 2: return ConfigType.Conservative_MostExpensiveHomePlayer;
                }
            }
            else if ((double)index / (double)max <
                (this.ConservativePercent + this.AggressivePercent) / 100.0)
            {
                var which = random.Next(1000) % 2;
                switch (which)
                {
                    case 0: return ConfigType.Aggressive;
                    case 1: return ConfigType.Aggressive_PitcherFirst;
                }
            }

            return result;
        }
    }
}
