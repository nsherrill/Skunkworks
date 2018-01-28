using DigitalBoardGamer.Shared.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Client.DigitalBoard
{
    public class GameEventArgs : EventArgs
    {
        public Game Game { get; set; }

        public GameEventArgs(Game game)
        {
            this.Game = game;
        }
    }
}
