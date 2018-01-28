using DigitalBoardGamer.Shared.SettlersShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.ResourceAccessor.SettlersAccessor
{
    public class BoardDefinitionAccessor : IBoardDefinitionAccessor
    {
        IDictionary<string, BoardDefinition> boardDefinitionCache = new Dictionary<string, BoardDefinition>();
        public BoardDefinition GetBoardDefinition(long boardId, int playerCount)
        {
            string key = string.Format("{0}_{1}", boardId, playerCount);
            if (!boardDefinitionCache.ContainsKey(key))
            {




            }
            return boardDefinitionCache[key];
        }
    }
}
