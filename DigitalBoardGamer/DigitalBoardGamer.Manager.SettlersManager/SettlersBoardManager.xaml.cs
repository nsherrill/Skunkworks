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
        BoardValidationEngine boardValidationEng = new BoardValidationEngine();
        public SettlersBoardManager()
        {
            InitializeComponent();
        }

        GeneratedBoard currentBoard = null;
        double lastWidth, lastHeight;
        public UserControl GetGameBoard(long boardId, double maxWidth, double maxHeight)
        {
            this.lastWidth = maxWidth;
            this.lastHeight = maxHeight;

            currentBoard = boardGenEng.GetRandomizedBoardDefinition(boardId);
            int attempts = 0;
            while (!boardValidationEng.IsBoardValid(currentBoard)
                && attempts < 5)
            {
                currentBoard = boardGenEng.GetRandomizedBoardDefinition(boardId);
                attempts++;
            }
            var valid = boardValidationEng.IsBoardValid(currentBoard);
            if (currentBoard != null)
                DrawBoard(currentBoard, maxWidth, maxHeight);
            //var newBoardControl = boardDrawingEng.DrawNewBoard(genBoard);
            return this;
        }

        private void DrawBoard(GeneratedBoard genBoard, double maxWidth, double maxHeight)
        {
            this.CanvasToDraw.Children.Clear();
            allLabels.Clear();

            this.CanvasToDraw.Height = maxHeight;
            this.CanvasToDraw.Width = maxWidth;

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

                        newPath.DataContext = desiredHex;

                        this.CanvasToDraw.Children.Add(newPath);

                        if (!string.IsNullOrEmpty(desiredHex.MyHexValue.DiceValue)
                            && desiredHex.MyHexValue.DiceValue != "0")
                        {
                            AddLabelToCanvas(this.CanvasToDraw, desiredHex, centerX, centerY);
                        }
                    }
                }
            }
        }

        List<Label> allLabels = new List<Label>();
        private void AddLabelToCanvas(Canvas canvas, HexDefinition desiredHex, double centerX, double centerY)
        {
            Ellipse textBack = new Ellipse()
            {
                Height = 40,
                Width = 40,
                //Fill = desiredHex.IsValid ? Brushes.White : Brushes.DarkRed,
                Fill = Brushes.White,
            };
            Canvas.SetLeft(textBack, centerX + textBack.Width / 1.25);
            Canvas.SetTop(textBack, centerY + textBack.Height / 1.25);
            Canvas.SetZIndex(textBack, 900);
            canvas.Children.Add(textBack);

            Brush labelForeground = desiredHex.MyHexValue.DiceProbabilityCount == 5
                    ? Brushes.Red
                    : Brushes.Black;
            Label text = new Label()
            {
                Content = desiredHex.MyHexValue.DiceValue,
                Foreground = labelForeground,
                FontSize = 24,
                Width = 40,
                Height = 40,
                DataContext = desiredHex,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center
            };
            allLabels.Add(text);
            Canvas.SetLeft(text, centerX + text.Width / 1.25);
            Canvas.SetTop(text, centerY + text.Height / 1.25);
            Canvas.SetZIndex(text, 1000);
            canvas.Children.Add(text);
        }

        HexDefinition firstHexToSwap = null;
        HexDefinition firstLabelToSwap = null;
        private void CanvasToDraw_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Path)
            {
                var currentPath = (e.Source as Path);
                if (currentPath != null)
                    HandlePathClick(currentPath);
            }
            else if (e.Source is Label)
            {
                var currentLabel = (e.Source as Label);
                if (currentLabel != null)
                    HandleLabelClick(currentLabel);
            }
        }

        private void HandlePathClick(Path currentPath)
        {
            HexDefinition currentHex = currentPath.DataContext as HexDefinition;
            if (firstHexToSwap == null)
            {//selected first hex
                firstHexToSwap = new HexDefinition(currentHex);
                currentPath.Stroke = Brushes.Yellow;
                currentPath.StrokeThickness *= 2;
            }
            else
            {
                if (firstHexToSwap.ColumnIndex == currentHex.ColumnIndex
                   && firstHexToSwap.RowIndex == currentHex.RowIndex)
                {//selected same hex
                    // do nothing!
                }
                else
                {//selected second hex!
                    var originalRow1 = currentHex.RowIndex;
                    var originalCol1 = currentHex.ColumnIndex;
                    var originalRow2 = firstHexToSwap.RowIndex;
                    var originalCol2 = firstHexToSwap.ColumnIndex;

                    currentHex = boardGenEng.SwapHex(this.currentBoard, currentHex, -1, -1);
                    boardGenEng.SwapHex(this.currentBoard, firstHexToSwap, originalRow1, originalCol1);
                    boardGenEng.SwapHex(this.currentBoard, currentHex, originalRow2, originalCol2);
                    DrawBoard(currentBoard, lastWidth, lastHeight);
                }

                currentPath.Stroke = Brushes.Black;
                currentPath.StrokeThickness /= 2;
                firstHexToSwap = null;
            }
        }

        private void HandleLabelClick(Label currentLabel)
        {
            HexDefinition currentHex = currentLabel.DataContext as HexDefinition;
            if (firstLabelToSwap == null)
            {//selected first label
                firstLabelToSwap = new HexDefinition(currentHex);
                currentLabel.Foreground = Brushes.Yellow;
            }
            else
            {
                if (firstLabelToSwap.ColumnIndex == currentHex.ColumnIndex
                   && firstLabelToSwap.RowIndex == currentHex.RowIndex)
                {//selected same hex
                    // do nothing!
                }
                else
                {//selected second hex!
                    var sourceValue1 = new HexValue(currentHex.MyHexValue);
                    var sourceValue2 = new HexValue(firstLabelToSwap.MyHexValue);

                    boardGenEng.SwapValue(this.currentBoard, sourceValue2, currentHex.RowIndex, currentHex.ColumnIndex);
                    boardGenEng.SwapValue(this.currentBoard, sourceValue1, firstLabelToSwap.RowIndex, firstLabelToSwap.ColumnIndex);
                    DrawBoard(currentBoard, lastWidth, lastHeight);
                }

                currentLabel.Foreground = firstLabelToSwap.MyHexValue.DiceProbabilityCount == 5 ? Brushes.Red : Brushes.Black;
                firstLabelToSwap = null;
            }
        }

        public void RotateLabels()
        {
            foreach (var label in allLabels)
            {
                if (label.RenderTransform == null
                    || !(label.RenderTransform is RotateTransform))
                {
                    label.RenderTransform = new RotateTransform(90);
                }
                else
                {
                    (label.RenderTransform as RotateTransform).Angle += 90;
                }
            }
        }
    }
}
