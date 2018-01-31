using DigitalBoardGamer.Shared.SettlersShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigialBoardGamer.Engine.SettlersEngine
{
    public class BoardValidationEngine : IBoardValidationEngine
    {
        public bool IsBoardValid(GeneratedBoard board)
        {
            return true;

            bool isValid = false;

            if (board != null)
            {
                bool hexValid = true;

                for (int row = 0; row < board.RowCount && hexValid; row++)
                {
                    for (int col = 0; col < board.ColumnCount && hexValid; col++)
                    {
                        var myHex = board.AllHexes.FirstOrDefault(x => x.RowIndex == row && x.ColumnIndex == col);
                        if (myHex != null)
                            hexValid = IsHexValid(myHex, board.AllHexes, row, col);
                        else hexValid = false;
                    }
                }
                isValid = hexValid;
            }

            return isValid;
        }

        public bool IsHexValid(HexDefinition myHex, HexDefinition[] allHexes, int row, int col)
        {
            return true;

            bool hexValid = true;
            for (int hextant = 0; hextant < 6 && hexValid; hextant++)
            {
                hexValid = IsHextantValid(myHex, allHexes, row, col, hextant);
            }

            return hexValid;
        }

        private bool IsHextantValid(HexDefinition myHex, HexDefinition[] allHexes, int row, int col, int hextant)
        {
            List<int> currentProbabilities = new List<int>();
            currentProbabilities.Clear();
            var myProb = GetHexProbabilityCount(allHexes, row, col);

            currentProbabilities.Add(myProb);
            switch (hextant)
            {
                case 0:
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row + 2, col));
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row + 1, col + 1));
                    break;

                case 1:
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row + 1, col + 1));
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row - 1, col + 1));
                    break;

                case 2:
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row - 1, col + 1));
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row - 2, col));
                    break;

                case 3:
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row - 2, col));
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row - 1, col - 1));
                    break;

                case 4:
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row, col - 1));
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row, col - 1));
                    break;

                case 5:
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row + 1, col - 1));
                    currentProbabilities.Add(GetHexProbabilityCount(allHexes, row + 2, col));
                    break;

                default: return false;
            }

            currentProbabilities = currentProbabilities.Where(x => x > 0).ToList();

            if (currentProbabilities.Count > 0)
            {
                int maxValue =
                    currentProbabilities.Count == 3 ? 12 :
                    currentProbabilities.Count == 2 ? 9 : 5;

                int valuation = currentProbabilities.Sum(x => x);

                bool isValid = valuation <= maxValue;
                return isValid;
            }
            return true;
        }

        private int GetHexProbabilityCount(HexDefinition[] allHexes, int row, int col)
        {
            if (row >= 0
                && col >= 0)
            {
                var myHex = allHexes.FirstOrDefault(x => x.RowIndex == row && x.ColumnIndex == col);
                if (myHex != null)
                {
                    return myHex.MyHexValue.DiceProbabilityCount;
                }
            }
            return -1;
        }
    }
}
