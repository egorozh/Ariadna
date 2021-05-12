using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.GeometryCreator
{
    /// <summary>
    /// Фабрика создания примитивов для движка
    /// </summary>
    internal class ShapeFactory
    {
        /// <summary>
        /// Создание значка указателя мыши для режима создания фигур
        /// </summary>
        /// <returns></returns>
        public static Rectangle CreateMouseCreatePoint()
        {
            var point = new Rectangle()
            {
                Fill = new SolidColorBrush(Color.FromArgb(70, 0, 0, 170)),
                Stroke = Brushes.DeepSkyBlue,
                Width = 10,
                Height = 10
            };

            point.SetZIndex((int) ZOrder.SelectedRectangle);

            return point;
        }

        public static Ellipse CreateEditNodePoint()
        {
            var shape = new Ellipse()
            {
                Stroke = Brushes.DodgerBlue,
                Fill = Brushes.DodgerBlue,
                StrokeThickness = 1,
                Width = 10,
                Height = 10
            };
            shape.SetZIndex((int) ZOrder.EditNodePoint);

            return shape;
        }

        public static Ellipse CreateEditMiddlePoint()
        {
            var shape = new Ellipse()
            {
                Stroke = Brushes.RoyalBlue,
                Fill = Brushes.RoyalBlue,
                StrokeThickness = 1,
                Width = 8,
                Height = 8
            };
            shape.SetZIndex((int) ZOrder.EditNodePoint);

            return shape;
        }
        
        public static Ellipse CreateEditHelpNodePoint()
        {
            var shape = new Ellipse()
            {
                Stroke = Brushes.LimeGreen,
                Fill = Brushes.LimeGreen,
                StrokeThickness = 1,
                Width = 8,
                Height = 8
            };
            shape.SetZIndex((int) ZOrder.EditNodePoint);

            return shape;
        }

        public static Line CreateEditLineSegment(bool isPreview)
        {
            var line = new Line
            {
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = 1.5
            };

            if (isPreview)
            {
                line.StrokeDashArray = new DoubleCollection() {4, 4};
            }

            line.SetZIndex((int) ZOrder.EditSegment);
            return line;
        }

        public static Path CreateEditArcSegment(bool isPreview)
        {
            var shape = new Path
            {
                StrokeThickness = 1.5,
                Stroke = Brushes.DodgerBlue,
                StrokeLineJoin = PenLineJoin.Bevel
            };

            if (isPreview)
            {
                shape.StrokeDashArray = new DoubleCollection() {4, 4};
            }

            shape.SetZIndex((int) ZOrder.EditSegment);

            return shape;
        }

        public static Path CreateFilledArea()
        {
            var shape = new Path
            {
                StrokeThickness = 1.5,
                Fill = Brushes.DodgerBlue,
                Opacity = 0.2,
                StrokeLineJoin = PenLineJoin.Bevel
            };

            shape.SetZIndex((int) ZOrder.EditSegment);

            return shape;
        }

        public static Line CreateClosedSegment()
        {
            var line = new Line
            {
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = 1.7,
                StrokeDashArray = new DoubleCollection() {4, 4},
                Visibility = Visibility.Collapsed
            };

            line.SetZIndex((int) ZOrder.EditSegment);
            return line;
        }

        public static Line CreateHelpLine()
        {
            var line = new Line
            {
                Stroke = Brushes.BlueViolet,
                StrokeDashArray = new DoubleCollection() { 4, 4 },
                StrokeThickness = 1.5,
                Visibility = Visibility.Collapsed
            };

            line.SetZIndex((int) ZOrder.HelpLine);
            return line;
        }

        public static Path CreateFilledBoundsArea()
        {
            var shape = new Path
            {
                StrokeThickness = 1.5,
                Fill = Brushes.DeepSkyBlue,
                Opacity = 0.3,
                StrokeLineJoin = PenLineJoin.Bevel
            };

            shape.SetZIndex((int)ZOrder.EditSegment - 1);

            return shape;
        }
    }
}