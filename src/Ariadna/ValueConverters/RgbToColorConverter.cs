using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Ariadna
{
    public class RgbToColorConverter : BaseValueConverter<RgbToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string rgb)) return Brushes.BlueViolet;

            var color = ColorConverter.ConvertFromString(rgb);

            if (color is Color color1)
            {
                //var grid = new Grid();

                //var rect1 = new Rectangle()
                //{
                //    Fill = CheckerBoard()
                //};

                //var rect2 = new Rectangle()
                //{
                //    Fill = Brushes.Black
                //};

                //grid.Children.Add(rect1);
                //grid.Children.Add(rect2);

                //return new VisualBrush(grid);

                return new SolidColorBrush(color1);
            }

            return Brushes.BlueViolet;
        }

        private static Brush CheckerBoard()
        {
            var geometryDrawing1 = new GeometryDrawing
            {
                Brush = Brushes.White,
                Geometry = new RectangleGeometry(new Rect(new Point(0, 0), new Point(10, 10)))
            };

            var geometryDrawing2 = new GeometryDrawing
            {
                Brush = new SolidColorBrush("#d7d7d7".ToColor()),
                Geometry = new GeometryGroup
                {
                    Children = new GeometryCollection
                    {
                        new RectangleGeometry(new Rect(new Point(0, 0), new Point(5, 5))),
                        new RectangleGeometry(new Rect(new Point(5, 5), new Point(5, 5))),
                    }
                }
            };

            var drawGroup = new DrawingGroup
            {
                Children = new DrawingCollection
                {
                    geometryDrawing1,
                    geometryDrawing2
                }
            };

            var drawingBrush = new DrawingBrush
            {
                Viewport = new Rect(new Point(0, 0), new Point(0.1, 0.4)),
                TileMode = TileMode.Tile,
                Drawing = drawGroup
            };

            return drawingBrush;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}