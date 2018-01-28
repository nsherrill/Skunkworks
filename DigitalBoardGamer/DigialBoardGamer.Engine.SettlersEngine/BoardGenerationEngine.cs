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

        public GeneratedBoard GetRandomizedBoardDefinition(long boardId, int playerCount)
        {
            string key = string.Format("{0}_{1}", boardId, playerCount);
            if (!boardDefinitionCache.ContainsKey(key))
            {
                BoardDefinition currBoardDefinition = boardDefAcc.GetBoardDefinition(boardId, playerCount);
                boardDefinitionCache.Add(key, currBoardDefinition);
            }
            var myBoardDef = new BoardDefinition(boardDefinitionCache[key]);

            var result = new GeneratedBoard();

            var maxCol = myBoardDef.ColumnCount - 1; // 0 based
            var maxRow = myBoardDef.RowCount - 1; // 0 based

            List<HexDefinition> allHexes = new List<HexDefinition>();
            Random r = new Random(DateTime.Now.Millisecond);
            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    allHexes.Add(DeterminePosition(r, myBoardDef, row, col));
                }
            }

            result.AllHexes = allHexes.ToArray();
            return result;
        }

        private HexDefinition DeterminePosition(Random r, BoardDefinition boardDef, int row, int col)
        {
            var stat = boardDef.StaticHexes.FirstOrDefault(s => s.RowIndex == row && s.ColumnIndex == col);

            if (stat != null)
            {
                for (int i = 0; i < boardDef.HexBoardDefinition.Length; i++)
                {
                    if (boardDef.HexBoardDefinition[i].TypeId == stat.MyHexType.TypeId)
                        boardDef.HexBoardDefinition[i].MaxHexCount--;
                }

                for (int i = 0; i < boardDef.ValueBoardDefinition.Length; i++)
                {
                    if (boardDef.ValueBoardDefinition[i].HexValueId == stat.MyHexValue.HexValueId)
                        boardDef.ValueBoardDefinition[i].ValCount--;
                }

                return stat;
            }

            var totalRemainingHexes = boardDef.HexBoardDefinition.Sum(h => h.MaxHexCount);
            var hexIndex = r.Next(0, totalRemainingHexes);

            var currHexIndex = 0;
            while (hexIndex > boardDef.HexBoardDefinition[currHexIndex].MaxHexCount)
            {
                hexIndex -= boardDef.HexBoardDefinition[currHexIndex].MaxHexCount;
                currHexIndex++;
            }

            var totalRemainingVals = boardDef.ValueBoardDefinition.Sum(h => h.ValCount);
            var valIndex = r.Next(0, totalRemainingVals);
            var currValIndex = 0;
            while (valIndex > boardDef.ValueBoardDefinition[currValIndex].ValCount)
            {
                valIndex -= boardDef.ValueBoardDefinition[currValIndex].ValCount;
                currValIndex++;
            }

            var result = new HexDefinition()
            {
                ColumnIndex = col,
                RowIndex = row,
                MyHexType = new HexType(boardDef.HexBoardDefinition[currHexIndex]),
                MyHexValue = new HexValue(boardDef.ValueBoardDefinition[currValIndex]),
            };

            boardDef.HexBoardDefinition[currHexIndex].MaxHexCount--;
            boardDef.ValueBoardDefinition[currValIndex].ValCount--;
            return result;
        }
    }
}
