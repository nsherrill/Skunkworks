﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SharedContracts
{
    public interface IBoardOptionManager
    {
        BoardOption[] FindAllBoardOptions(long gameId);
    }
}
