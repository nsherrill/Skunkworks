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
        public BoardOption BoardOption { get; set; }

        public BoardOptionEventArgs(BoardOption boardOption)
        {
            this.BoardOption = boardOption;
        }
    }
}
