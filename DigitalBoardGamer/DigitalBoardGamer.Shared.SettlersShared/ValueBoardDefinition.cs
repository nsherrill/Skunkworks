using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public class ValueBoardDefinition : HexValue
    {
        public int ValCount { get; set; }

        public ValueBoardDefinition() { }
        public ValueBoardDefinition(ValueBoardDefinition original)
        {
            this.ValCount = original.ValCount;
            this.DiceValue = original.DiceValue;
            this.HexValueId = original.HexValueId;
            this.DiceProbabilityCount = original.DiceProbabilityCount;
        }
    }
}
