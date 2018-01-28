using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DigitalBoardGamer.Manager.SettlersManager
{
    public class HexagonGenerator //: Path
    {
        public HexagonGenerator()
        {
            //CreateDataPath(0, 0);
        }

        public Path CreateDataPath(double centerX, double centerY, double width, double height, double strokeThickness)
        {
            height -= strokeThickness;
            width -= strokeThickness;

            //Prevent layout loop
            //if (lastWidth == width && lastHeight == height)
            //    return;

            lastWidth = width;
            lastHeight = height;

            PathGeometry geometry = new PathGeometry();
            figure = new PathFigure();

            //See for figure info http://etc.usf.edu/clipart/50200/50219/50219_area_hexagon_lg.gif
            figure.StartPoint = new Point(centerX + 0.25 * width, centerY + 0);
            AddPoint(centerX + 0.75 * width, centerY + 0, strokeThickness);
            AddPoint(centerX + width, centerY + 0.5 * height, strokeThickness);
            AddPoint(centerX + 0.75 * width, centerY + height, strokeThickness);
            AddPoint(centerX + 0.25 * width, centerY + height, strokeThickness);
            AddPoint(centerX + 0, centerY + 0.5 * height, strokeThickness);
            figure.IsClosed = true;
            geometry.Figures.Add(figure);

            Path result = new Path()
            {
                Data = geometry,
            };
            //this.Data = geometry;
            return result;
        }

        private void AddPoint(double x, double y, double strokeThickness)
        {
            LineSegment segment = new LineSegment();
            segment.Point = new Point(x + 0.5 * strokeThickness,
                y + 0.5 * strokeThickness);
            figure.Segments.Add(segment);
        }

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    return availableSize;
        //}

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    CreateDataPath(finalSize.Width, finalSize.Height);
        //    return finalSize;
        //}

        #region FieldsAndProperties
        private double lastWidth = 0;
        private double lastHeight = 0;
        private PathFigure figure;
        #endregion
    }
}
