﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SharedContracts
{
    public class Game
    {
        public long GameId { get; set; }
        public string Name { get; set; }
        public string DllName { get; set; }

        public Game(long gameId, string gameName, string dllName)
        {
            this.GameId = gameId;
            this.Name = gameName;
            this.DllName = dllName;
        }
    }
}
