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
    }
}
