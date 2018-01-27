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
        IGameManager myGameManager = null;
        public MainWindow()
        {
            InitializeComponent();
            this.myGameManager = new GameManager();
        }

        Game[] gamesCache = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gamesCache = myGameManager.FindAllGames();

            if(gamesCache != null)
            {
                foreach (var game in gamesCache)
                {
                    var newItem = new GameItemControl();
                    newItem.Init(game);
                    newItem.OnGameSelected += newItem_OnGameSelected;
                    this.GameOptionsPanel.Children.Add(newItem);
                }
            }
        }

        void newItem_OnGameSelected(object sender, GameEventArgs e)
        {
            MessageBox.Show(string.Format("Game selected! [{0}]", e.Game.Name));
        }
    }
}
