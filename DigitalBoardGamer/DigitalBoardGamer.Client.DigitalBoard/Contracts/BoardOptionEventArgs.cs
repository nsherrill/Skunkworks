using DigitalBoardGamer.Shared.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Client.DigitalBoard
{
    public class BoardOptionEventArgs : EventArgs
    {
        public Game Game { get; set; }
        public BoardOption BoardOption { get; set; }

        public BoardOptionEventArgs(Game game, BoardOption boardOption)
        {
            this.Game = game;
            this.BoardOption = boardOption;
        }
    }
}
