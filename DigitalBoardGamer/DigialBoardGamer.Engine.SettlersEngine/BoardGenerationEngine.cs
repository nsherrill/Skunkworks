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
        IDictionary<long, BoardDefinition> boardDefinitionCache = new Dictionary<long, BoardDefinition>();
        IBoardDefinitionAccessor boardDefAcc = new BoardDefinitionAccessor();

        public GeneratedBoard GetRandomizedBoardDefinition(long boardId)
        {
            if (!boardDefinitionCache.ContainsKey(boardId))
            {
                BoardDefinition currBoardDefinition = boardDefAcc.GetBoardDefinition(boardId);
                boardDefinitionCache.Add(boardId, currBoardDefinition);
            }
            var myBoardDef = new BoardDefinition(boardDefinitionCache[boardId]);
            if (myBoardDef != null
                && myBoardDef.HexBoardDefinition.Length > 0
                && myBoardDef.ValueBoardDefinition.Length > 0)
            {
                var result = new GeneratedBoard();

                var maxCol = myBoardDef.ColumnCount - 1; // 0 based
                var maxRow = myBoardDef.RowCount - 1; // 0 based

                List<HexDefinition> allHexes = new List<HexDefinition>();
                foreach (var stat in myBoardDef.StaticHexes)
                {// remove all static ones from boardDef
                    for (int i = 0; i < myBoardDef.HexBoardDefinition.Length; i++)
                    {
                        if (myBoardDef.HexBoardDefinition[i].TypeId == stat.MyHexType.TypeId)
                            myBoardDef.HexBoardDefinition[i].MaxHexCount--;
                    }

                    for (int i = 0; i < myBoardDef.ValueBoardDefinition.Length; i++)
                    {
                        if (myBoardDef.ValueBoardDefinition[i].HexValueId == stat.MyHexValue.HexValueId)
                            myBoardDef.ValueBoardDefinition[i].ValCount--;
                    }
                    allHexes.Add(new HexDefinition(stat));
                }

                Random r = new Random(DateTime.Now.Millisecond);
                for (int row = 0; row <= maxRow; row++)
                {
                    for (int col = 0; col <= maxCol; col++)
                    {//generate the rest!
                        if (allHexes.Any(h => h.RowIndex == row && h.ColumnIndex == col))
                            continue; // ignore spot if it's already a static!

                        allHexes.Add(DeterminePosition(r, myBoardDef, row, col));
                    }
                }

                result.AllHexes = allHexes.ToArray();
                return result;
            }
            return null;
        }

        private HexDefinition DeterminePosition(Random r, BoardDefinition boardDef, int row, int col)
        {
            var totalRemainingHexes = boardDef.HexBoardDefinition.Sum(h => h.MaxHexCount);
            var hexIndex = r.Next(0, totalRemainingHexes);

            var currHexIndex = 0;
            while (hexIndex >= boardDef.HexBoardDefinition[currHexIndex].MaxHexCount)
            {
                hexIndex -= boardDef.HexBoardDefinition[currHexIndex].MaxHexCount;
                currHexIndex++;
            }

            var totalRemainingVals = boardDef.ValueBoardDefinition.Sum(h => h.ValCount);
            var valIndex = r.Next(0, totalRemainingVals);
            var currValIndex = 0;
            while (valIndex >= boardDef.ValueBoardDefinition[currValIndex].ValCount)
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
