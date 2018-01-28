using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SharedContracts
{
    public class BoardOption
    {
        public long BoardId { get; set; }
        public string BoardOptionName { get; set; }
        public int PlayerCount { get; set; }

        public BoardOption(long boardId, string boardOptionName, int playerCount)
        {
            this.BoardId = boardId;
            this.BoardOptionName = boardOptionName;
            this.PlayerCount = playerCount;
        }
    }
}
