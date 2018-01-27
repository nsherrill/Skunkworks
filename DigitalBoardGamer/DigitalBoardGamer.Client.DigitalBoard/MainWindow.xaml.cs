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
        public MainWindow()
        {
            InitializeComponent();
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
                this.boardOptionsPage.Init(e.Game.GameId);
            }
        }

        void boardOptionsPage_OnBoardOptionSelected(object sender, BoardOptionEventArgs e)
        {
            
        }
    }
}
