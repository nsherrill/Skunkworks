using DigitalBoardGamer.Shared.SharedContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DigitalBoardGamer.Client.DigitalBoard.Controls
{
    /// <summary>
    /// Interaction logic for GameItemControl.xaml
    /// </summary>
    public partial class GameItemControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<GameEventArgs> OnGameSelected;
        public string GameId { get; set; }
        public string GameName { get; set; }

        private Game myGame { get; set; }

        public GameItemControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void Init(Game game)
        {
            this.myGame = game;
            this.GameId = game.GameId.ToString();
            this.GameName = game.Name;
            RaisePropertyChanged();
        }

        public void RaisePropertyChanged(string propName = null)
        {
            if (propName != null)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
            else
            {
                RaisePropertyChanged("GameId");
                RaisePropertyChanged("GameName");
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.OnGameSelected != null)
            {
                this.OnGameSelected(this, new GameEventArgs(this.myGame));
            }
        }
    }
}
