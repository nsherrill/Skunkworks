using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public interface IBoardValidationEngine
    {
        bool IsBoardValid(GeneratedBoard board);

        bool IsHexValid(HexDefinition myHex, HexDefinition[] allHexes, int row, int col);
    }
}
