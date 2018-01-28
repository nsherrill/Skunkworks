using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public class BoardDefinition
    {
        public HexDefinition[] StaticHexes { get; set; }

        public int ColumnCount { get; set; }

        public int RowCount { get; set; }

        public HexBoardDefinition[] HexDefinition { get; set; }

        public HexBoardDefinition[] ValueDefinition { get; set; }
    }
}
