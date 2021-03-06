﻿using DigitalBoardGamer.ResourceAccessor.SettlersAccessor;
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
        BoardValidationEngine boardValidationEngine = new BoardValidationEngine();
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
                            myBoardDef.ValueBoardDefinition[i].MaxValCount--;
                    }
                    allHexes.Add(new HexDefinition(stat));
                }

                int attempts = 0;
                Random r = new Random(DateTime.Now.Millisecond);
                for (int row = 0; row <= maxRow; row++)
                {
                    for (int col = 0; col <= maxCol; col++)
                    {//generate the rest!
                        if (allHexes.Any(h => h.RowIndex == row && h.ColumnIndex == col))
                            continue; // ignore spot if it's already a static!

                        var newHex = DeterminePosition(r, myBoardDef, row, col);
                        bool isValid = boardValidationEngine.IsHexValid(newHex, allHexes.ToArray(), row, col);
                        while (!isValid)
                        {
                            if (attempts > 100)
                            {
                                //                                return null;
                                break;
                            }

                            // put the count back
                            myBoardDef.HexBoardDefinition.FirstOrDefault(x => x.TypeId == newHex.MyHexType.TypeId).MaxHexCount++;
                            myBoardDef.ValueBoardDefinition.FirstOrDefault(x => x.HexValueId == newHex.MyHexValue.HexValueId).MaxValCount++;

                            newHex = DeterminePosition(r, myBoardDef, row, col);
                            attempts++;
                            isValid = boardValidationEngine.IsHexValid(newHex, allHexes.ToArray(), row, col);
                        }
                        newHex.IsValid = isValid;

                        allHexes.Add(newHex);
                        attempts = 0;
                    }
                }

                result.AllHexes = allHexes.ToArray();
                return result;
            }
            return null;
        }

        long[] HEX_IDS_WITHOUT_VALUES = new long[] { 1, 8 };
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

            var currValIndex = 0;
            if (HEX_IDS_WITHOUT_VALUES.Contains(boardDef.HexBoardDefinition[currHexIndex].TypeId))
            {
                for (currValIndex = 0; currValIndex < boardDef.ValueBoardDefinition.Length; currValIndex++)
                {
                    if (boardDef.ValueBoardDefinition[currValIndex].HexValueId == -1)
                        break;
                }
            }
            else
            {
                var totalRemainingVals = boardDef.ValueBoardDefinition.Sum(h => h.MaxValCount);
                do
                {
                    var valIndex = r.Next(0, totalRemainingVals);
                    currValIndex = 0;

                    while (valIndex >= boardDef.ValueBoardDefinition[currValIndex].MaxValCount)
                    {
                        valIndex -= boardDef.ValueBoardDefinition[currValIndex].MaxValCount;
                        currValIndex++;
                    }
                }
                while (boardDef.ValueBoardDefinition[currValIndex].HexValueId < 0);
            }

            var result = new HexDefinition()
            {
                ColumnIndex = col,
                RowIndex = row,
                MyHexType = new HexType(boardDef.HexBoardDefinition[currHexIndex]),
                MyHexValue = new HexValue(boardDef.ValueBoardDefinition[currValIndex]),
            };

            boardDef.HexBoardDefinition[currHexIndex].MaxHexCount--;
            boardDef.ValueBoardDefinition[currValIndex].MaxValCount--;
            return result;
        }

        public HexDefinition SwapHex(GeneratedBoard generatedBoard, HexDefinition sourceHex, int intendedRow, int intendedCol)//HexDefinition sourceHex2)
        {
            for (int i = 0; i < generatedBoard.AllHexes.Length; i++)
            {
                if (generatedBoard.AllHexes[i].ColumnIndex == sourceHex.ColumnIndex
                    && generatedBoard.AllHexes[i].RowIndex == sourceHex.RowIndex)
                {
                    var destHex = new HexDefinition(sourceHex);
                    destHex.ColumnIndex = intendedCol;
                    destHex.RowIndex = intendedRow;
                    generatedBoard.AllHexes[i] = destHex;

                    return destHex;
                }
            }

            return null;
        }

        public void SwapValue(GeneratedBoard generatedBoard, HexValue sourceValue, int intendedRow, int intendedCol)//HexDefinition sourceHex2)
        {
            for (int i = 0; i < generatedBoard.AllHexes.Length; i++)
            {
                if (generatedBoard.AllHexes[i].ColumnIndex == intendedCol
                    && generatedBoard.AllHexes[i].RowIndex == intendedRow)
                {
                    generatedBoard.AllHexes[i].MyHexValue = new HexValue(sourceValue);
                    break;
                }
            }
        }

        public void UpdateHex(GeneratedBoard generatedBoard, HexDefinition desiredHex, System.Windows.Input.Key key)
        {
            string hexName = string.Empty;
            switch (key)
            {
                case System.Windows.Input.Key.W:
                    hexName = "Wood";
                    break;
                case System.Windows.Input.Key.A:
                    hexName = "Water";
                    break;
                case System.Windows.Input.Key.S:
                    hexName = "Sheep";
                    break;
                case System.Windows.Input.Key.G:
                    hexName = "Gold";
                    break;
                case System.Windows.Input.Key.H:
                    hexName = "Wheat";
                    break;
                case System.Windows.Input.Key.O:
                    hexName = "Ore";
                    break;
                case System.Windows.Input.Key.D:
                    hexName = "Desert";
                    break;
                case System.Windows.Input.Key.B:
                    hexName = "Brick";
                    break;
            }

            if (!string.IsNullOrEmpty(hexName))
            {
                var newHexType = new HexType(generatedBoard.AllHexes
                    .FirstOrDefault(x => x.MyHexType.Name.Equals(hexName, StringComparison.InvariantCultureIgnoreCase)).MyHexType);

                for (int i = 0; i < generatedBoard.AllHexes.Length; i++)
                {
                    if (generatedBoard.AllHexes[i].ColumnIndex == desiredHex.ColumnIndex
                        && generatedBoard.AllHexes[i].RowIndex == desiredHex.RowIndex)
                    {
                        generatedBoard.AllHexes[i].MyHexType = newHexType;
                        break;
                    }
                }
            }
        }

        public void UpdateValue(GeneratedBoard generatedBoard, HexDefinition desiredHex, System.Windows.Input.Key key)
        {
            int desiredHexValue = 0;
            switch (key)
            {
                case System.Windows.Input.Key.Up:
                    desiredHexValue = int.Parse(desiredHex.MyHexValue.DiceValue) + 1;
                    if (desiredHexValue > 12)
                        desiredHexValue = 2;
                    if (desiredHexValue == 7)
                        desiredHexValue = 8;
                    break;
                case System.Windows.Input.Key.Down:
                    desiredHexValue = int.Parse(desiredHex.MyHexValue.DiceValue) - 1;
                    if (desiredHexValue < 2)
                        desiredHexValue = 12;
                    if (desiredHexValue == 7)
                        desiredHexValue = 6;
                    break;
            }

            if (desiredHexValue > 0)
            {
                var newHexValue = new HexValue(generatedBoard.AllHexes
                    .FirstOrDefault(x => x.MyHexValue.DiceValue == desiredHexValue.ToString()).MyHexValue);

                for (int i = 0; i < generatedBoard.AllHexes.Length; i++)
                {
                    if (generatedBoard.AllHexes[i].ColumnIndex == desiredHex.ColumnIndex
                        && generatedBoard.AllHexes[i].RowIndex == desiredHex.RowIndex)
                    {
                        generatedBoard.AllHexes[i].MyHexValue = newHexValue;
                        break;
                    }
                }
            }
        }
    }
}
