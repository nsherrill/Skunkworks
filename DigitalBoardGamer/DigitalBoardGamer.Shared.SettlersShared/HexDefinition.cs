using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public class HexDefinition
    {
        public HexType MyHexType { get; set; }
        public HexValue MyHexValue { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public bool IsValid { get; set; }

        public HexDefinition() { }
        public HexDefinition(HexDefinition original)
        {
            this.RowIndex = original.RowIndex;
            this.ColumnIndex = original.ColumnIndex;
            this.MyHexType = new HexType(original.MyHexType);
            this.MyHexValue = new HexValue(original.MyHexValue);
        }
    }
}
