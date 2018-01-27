using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SharedContracts
{
    public class Game
    {
        public int GameId { get; set; }
        public string Name { get; set; }

        public Game(int gameId, string gameName)
        {
            this.GameId = gameId;
            this.Name = gameName;
        }
    }
}
