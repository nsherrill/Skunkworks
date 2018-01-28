using DigitalBoardGamer.ResourceAccessor.SettlersAccessor;
using DigitalBoardGamer.Shared.SettlersShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigialBoardGamer.Engine.SettlersEngine
{
    public class BoardGenerationEngine
    {
        IDictionary<string, BoardDefinition> boardDefinitionCache = new Dictionary<string, BoardDefinition>();
        IBoardDefinitionAccessor boardDefAcc = new BoardDefinitionAccessor();

        public BoardDefinition GetRandomizedBoardDefinition(long boardId, int playerCount)
        {
            string key = string.Format("{0}_{1}", boardId, playerCount);
            if (!boardDefinitionCache.ContainsKey(key))
            {
                BoardDefinition currBoardDefinition = boardDefAcc.GetBoardDefinition(boardId, playerCount);
                boardDefinitionCache.Add(key, currBoardDefinition);
            }
            return boardDefinitionCache[key];
        }
    }
}
