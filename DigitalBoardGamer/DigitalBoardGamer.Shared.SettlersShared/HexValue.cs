using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public class HexValue
    {
        public long HexValueId { get; set; }
        public string DiceValue { get; set; }

        public HexValue() { }
        public HexValue(HexValue original)
        {
            this.HexValueId = original.HexValueId;
            this.DiceValue = original.DiceValue;
        }
    }
}
