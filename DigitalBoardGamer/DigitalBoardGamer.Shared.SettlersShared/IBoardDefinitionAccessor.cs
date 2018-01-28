using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public interface IBoardDefinitionAccessor
    {
        BoardDefinition GetBoardDefinition(long boardId);
    }
}
