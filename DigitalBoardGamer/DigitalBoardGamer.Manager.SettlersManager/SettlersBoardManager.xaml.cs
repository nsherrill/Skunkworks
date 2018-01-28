﻿using DigialBoardGamer.Engine.SettlersEngine;
using DigitalBoardGamer.ResourceAccessor.SettlersAccessor;
using DigitalBoardGamer.Shared.SettlersShared;
using DigitalBoardGamer.Shared.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DigitalBoardGamer.Manager.SettlersManager
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SettlersBoardManager : UserControl, IGameBoardManager
    {
        BoardGenerationEngine boardGenEng = new BoardGenerationEngine();
        BoardDrawingEngine boardDrawingEng = new BoardDrawingEngine();
        public SettlersBoardManager()
        {
            InitializeComponent();
        }

        public UserControl GetGameBoard(long boardId)
        {
            int playerCount = 3;
            var boardDef = boardGenEng.GetRandomizedBoardDefinition(boardId, playerCount);
            var newBoardControl = boardDrawingEng.DrawNewBoard(boardDef);
            return newBoardControl;
        }
    }
}
