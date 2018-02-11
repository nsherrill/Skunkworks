using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public class ValueBoardDefinition : HexValue
    {
        public int MaxValCount { get; set; }

        public ValueBoardDefinition() { }
        public ValueBoardDefinition(ValueBoardDefinition original)
        {
            this.MaxValCount = original.MaxValCount;
            this.DiceValue = original.DiceValue;
            this.HexValueId = original.HexValueId;
            this.DiceProbabilityCount = original.DiceProbabilityCount;
        }
    }
}
