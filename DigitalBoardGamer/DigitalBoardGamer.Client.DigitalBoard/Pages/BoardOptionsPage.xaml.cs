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

namespace DigitalBoardGamer.Client.DigitalBoard.Pages
{
    public partial class BoardOptionsPage : UserControl
    {
        public event EventHandler<BoardOptionEventArgs> OnBoardOptionSelected;

        IBoardOptionManager myBoardManager = null;
        BoardOption[] boardsCache = null;

        public BoardOptionsPage()
        {
            InitializeComponent();
            this.myBoardManager = new BoardManager();
        }
        ~BoardOptionsPage()
        {
            try
            {
                foreach (var child in this.BoardOptionsPanel.Children)
                {
                    (child as BoardOptionItemControl).OnBoardOptionSelected -= newItem_OnBoardOptionSelected;
                }
            }
            catch { }
        }

        public void Init(long gameId)
        {
            this.Visibility = System.Windows.Visibility.Visible;

            boardsCache = myBoardManager.FindAllBoardOptions(gameId);

            if (boardsCache != null)
            {
                foreach (var boardOption in boardsCache)
                {
                    var newItem = new BoardOptionItemControl();
                    newItem.Init(boardOption);
                    newItem.OnBoardOptionSelected += newItem_OnBoardOptionSelected;
                    this.BoardOptionsPanel.Children.Add(newItem);
                }
            }
        }

        void newItem_OnBoardOptionSelected(object sender, BoardOptionEventArgs e)
        {
            if (this.OnBoardOptionSelected != null)
            {
                this.OnBoardOptionSelected(sender, e);
            }
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
