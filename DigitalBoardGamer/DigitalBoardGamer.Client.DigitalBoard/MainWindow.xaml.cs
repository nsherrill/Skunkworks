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
            var gameBoard = currentManager.GetGameBoard(e.BoardOption.BoardId, this.ActualWidth * 1.20, this.ActualHeight * 1.20);
            TransformGroup tg = new TransformGroup();
            tg.Children.Add(new TranslateTransform(-120, -1200));
            tg.Children.Add(new RotateTransform(90));
            gameBoard.RenderTransform = tg;//;
            this.BoardGrid.Children.Add(gameBoard);
            this.BoardGrid.Visibility = System.Windows.Visibility.Visible;
            this.ButtonPanel.Visibility = System.Windows.Visibility.Visible;

        }

        private void RandomizeBoard_Click(object sender, RoutedEventArgs e)
        {
            ProcessEvent(lastBoard);
        }

        private void StartOver_Click(object sender, RoutedEventArgs e)
        {
            BoardGrid.Children.Clear();
            gameOptionsPage.Visibility = System.Windows.Visibility.Visible;
            this.ButtonPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RotateLabels_Click(object sender, RoutedEventArgs e)
        {
            this.currentManager.RotateLabels();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            this.currentManager.ProcessKeyDown(e.Key);
        }
    }
}
