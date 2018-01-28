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

        public HexBoardDefinition[] HexBoardDefinition { get; set; }

        public ValueBoardDefinition[] ValueBoardDefinition { get; set; }

        public BoardDefinition() { }
        public BoardDefinition(BoardDefinition original)
        {
            this.ColumnCount = original.ColumnCount;
            this.RowCount = original.RowCount;

            List<HexBoardDefinition> newHexes = new List<HexBoardDefinition>();
            foreach (var hex in original.HexBoardDefinition)
                newHexes.Add(new HexBoardDefinition(hex));
            this.HexBoardDefinition = newHexes.ToArray();

            List<ValueBoardDefinition> newVals = new List<ValueBoardDefinition>();
            foreach (var val in original.ValueBoardDefinition)
                newVals.Add(new ValueBoardDefinition(val));
            this.ValueBoardDefinition = newVals.ToArray();

            List<HexDefinition> newStat = new List<HexDefinition>();
            foreach (var stat in original.StaticHexes)
                newStat.Add(new HexDefinition(stat));
            this.StaticHexes = newStat.ToArray();
        }
    }
}
