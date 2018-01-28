using DigitalBoardGamer.ResourceAccessor.GameAccessor;
using DigitalBoardGamer.Shared.SharedContracts;
using DigitalBoardGamer.Shared.SharedContracts.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Manager.GameManager
{
    public class BoardManager : IBoardOptionManager
    {
        IBoardAccessor boardAcc = new GameAccessor();
        public BoardOption[] FindAllBoardOptions(long gameId)
        {
            return boardAcc.GetAllBoardOptions(gameId);
            //List<BoardOption> results = new List<BoardOption>();
            //results.Add(new BoardOption(1, "test board option"));
            //results.Add(new BoardOption(2, "test board option2"));
            //results.Add(new BoardOption(3, "test board option3"));
            //return results.ToArray();
        }
    }
}
