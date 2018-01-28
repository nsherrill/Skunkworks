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
    public class GameManager : IGameManager
    {
        IGameAccessor gameAcc = new GameAccessor();
        public Game[] FindAllGames()
        {
            return gameAcc.GetAllGames();
            //List<Game> games = new List<Game>();
            //games.Add(new Game(1, "test game", "DigitalBoardGamer.Manager.SettlersManager.dll"));
            //games.Add(new Game(2, "test game2", "DigitalBoardGamer.Manager.SettlersManager.dll"));
            //games.Add(new Game(3, "test game3", "DigitalBoardGamer.Manager.SettlersManager.dll"));
            //return games.ToArray();
        }
    }
}
