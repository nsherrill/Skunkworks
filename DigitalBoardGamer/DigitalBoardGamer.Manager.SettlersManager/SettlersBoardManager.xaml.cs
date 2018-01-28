using DigialBoardGamer.Engine.SettlersEngine;
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
        public SettlersBoardManager()
        {
            InitializeComponent();
        }

        public UserControl GetGameBoard(long boardId, double maxWidth, double maxHeight)
        {
            int playerCount = 3;
            var genBoard = boardGenEng.GetRandomizedBoardDefinition(boardId, playerCount);
            DrawBoard(genBoard, maxWidth, maxHeight);
            //var newBoardControl = boardDrawingEng.DrawNewBoard(genBoard);
            return this;
        }

        private void DrawBoard(GeneratedBoard genBoard, double maxWidth, double maxHeight)
        {
            this.CanvasToDraw.Height = maxHeight;
            this.CanvasToDraw.Width = maxWidth;

            //this.GridToDraw.ColumnDefinitions.Clear();
            //for (int i = 0; i < genBoard.ColumnCount * 2; i++)
            //{
            //    this.GridToDraw.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //}

            //this.GridToDraw.RowDefinitions.Clear();
            //for (int i = 0; i < genBoard.RowCount; i++)
            //{
            //    //if (i % 2 == 0)
            //    //    this.GridToDraw.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
            //    //else
            //    this.GridToDraw.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            //}

            var hexGenerator = new HexagonGenerator();
            var size = Math.Min(maxWidth, maxHeight) / (Math.Min(genBoard.RowCount, genBoard.ColumnCount)) / 2;
            double centerX, centerY;

            for (int row = 0; row < genBoard.RowCount; row++)
            {
                centerY = maxHeight * 2 / 3 - (row + 0.75) * size / 2 + size;

                for (int col = 0; col < genBoard.ColumnCount; col++)
                {
                    var desiredHex = genBoard.AllHexes.FirstOrDefault(h => h.ColumnIndex == col && h.RowIndex == row);
                    if (desiredHex != null)
                    {
                        if (row % 2 == 0)
                            centerX = (col * 1.5) * size + size;
                        else
                            centerX = (1.5 * col + .75) * size + size;

                        var newPath = hexGenerator.CreateDataPath(centerX, centerY, size, size, 2);
                        if (!string.IsNullOrEmpty(desiredHex.MyHexType.BackupColor))
                            newPath.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(desiredHex.MyHexType.BackupColor));

                        this.CanvasToDraw.Children.Add(newPath);

                        if (desiredHex.MyHexValue.DiceValue != "0")
                        {
                            Ellipse textBack = new Ellipse()
                            {
                                Height = 40,
                                Width = 40,
                                Fill = Brushes.White,
                            };
                            Canvas.SetLeft(textBack, centerX + textBack.Width / 1.25);
                            Canvas.SetTop(textBack, centerY + textBack.Height / 1.25);
                            Canvas.SetZIndex(textBack, 900);
                            this.CanvasToDraw.Children.Add(textBack);

                            Brush labelForeground =  desiredHex.MyHexValue.DiceValue == "6" 
                                    || desiredHex.MyHexValue.DiceValue == "8" 
                                    ? Brushes.Red
                                    :Brushes.Black;
                            Label text = new Label()
                            {
                                Content = desiredHex.MyHexValue.DiceValue,
                                Foreground = labelForeground,
                                FontSize = 24,
                                Width = 40,
                                Height = 40,
                                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                                VerticalContentAlignment = System.Windows.VerticalAlignment.Center
                            };
                            Canvas.SetLeft(text, centerX + text.Width / 1.25);
                            Canvas.SetTop(text, centerY + text.Height / 1.25);
                            Canvas.SetZIndex(text, 1000);
                            this.CanvasToDraw.Children.Add(text);
                        }
                    }
                }
            }
        }
    }
}
