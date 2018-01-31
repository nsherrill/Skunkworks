using DigitalBoardGamer.Client.DigitalBoard.Controls;
using DigitalBoardGamer.Manager.GameManager;
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

namespace DigitalBoardGamer.Client.DigitalBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameLoader gameLoader = null;
        IGameBoardManager currentManager = null;
        public MainWindow()
        {
            InitializeComponent();
            gameLoader = new GameLoader();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.gameOptionsPage.OnGameSelected += GameOptionsGrid_OnGameSelected;
            this.boardOptionsPage.OnBoardOptionSelected += boardOptionsPage_OnBoardOptionSelected;
        }

        void GameOptionsGrid_OnGameSelected(object sender, GameEventArgs e)
        {
            if (e != null
                && e.Game != null)
            {
                this.gameOptionsPage.Visibility = System.Windows.Visibility.Collapsed;
                this.boardOptionsPage.Init(e.Game);
            }
        }

        void boardOptionsPage_OnBoardOptionSelected(object sender, BoardOptionEventArgs e)
        {
            ProcessEvent(e);
        }

        BoardOptionEventArgs lastBoard = null;
        private void ProcessEvent(BoardOptionEventArgs e)
        {
            this.BoardGrid.Children.Clear();
            lastBoard = e;
            currentManager = gameLoader.LoadGameBoardManager(e.Game.DllName);
            var gameBoard = currentManager.GetGameBoard(e.BoardOption.BoardId, this.ActualWidth, this.ActualHeight);
            this.BoardGrid.Children.Add(gameBoard);
            this.BoardGrid.Visibility = System.Windows.Visibility.Visible;
            this.refreshButton.Visibility = System.Windows.Visibility.Visible;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessEvent(lastBoard);
        }
    }
}
