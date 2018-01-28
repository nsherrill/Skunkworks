using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public class GeneratedBoard
    {
        public HexDefinition[] AllHexes { get; set; }

        public int ColumnCount { get { if (AllHexes != null) return AllHexes.Max(e => e.ColumnIndex) + 1; return -1; } }
        public int RowCount { get { if (AllHexes != null) return AllHexes.Max(e => e.RowIndex) + 1; return -1; } }
    }
}
