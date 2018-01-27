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
    public partial class BoardOptionItemControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<BoardOptionEventArgs> OnBoardOptionSelected;
        public string BoardId { get; set; }
        public string BoardOptionName { get; set; }

        private BoardOption myBoardOption { get; set; }

        public BoardOptionItemControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void Init(BoardOption boardOption)
        {
            this.myBoardOption = boardOption;
            this.BoardId = boardOption.BoardId.ToString();
            this.BoardOptionName = boardOption.BoardOptionName;
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
                RaisePropertyChanged("BoardId");
                RaisePropertyChanged("BoardOptionName");
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.OnBoardOptionSelected != null)
            {
                this.OnBoardOptionSelected(this, new BoardOptionEventArgs(this.myBoardOption));
            }
        }
    }
}
