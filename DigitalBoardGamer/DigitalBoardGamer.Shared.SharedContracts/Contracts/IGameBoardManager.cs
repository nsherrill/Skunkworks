﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalBoardGamer.Shared.SharedContracts
{
    public interface IGameBoardManager
    {
        UserControl GetGameBoard(long boardId, double maxWidth, double maxHeight);

        void RotateLabels();

        void ProcessKeyDown(System.Windows.Input.Key key);
    }
}
