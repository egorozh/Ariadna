using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <summary>
    /// Фабрика создания примитивов для движка
    /// </summary>
    internal class ShapeFactory
    {
        /// <summary>
        /// Создание линии для оси системы координат
        /// </summary>
        /// <returns></returns>
        public static Line CreateOrtLine()
        {
            var line = new Line
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 1.5
            };

            line.SetZIndex((int) ZOrder.OrtLine);
            return line;
        }

        /// <summary>
        /// Создание прямоугольника для области выделения
        /// </summary>
        /// <returns></returns>
        public static Path CreateSelectedRectangle()
        {
            var rect = new Path
            {
                Fill = Brushes.LightBlue,
                Stroke = Brushes.DodgerBlue,
                Opacity = 0.5
            };

            rect.SetZIndex((int) ZOrder.SelectedRectangle);

            return rect;
        }

        /// <summary>
        /// Создание фигуры предпросмотра при трансформации фигуры
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static Path CreatePreviewFigure(Geometry geometry)
        {
            var shape = new Path
            {
                Stroke = Brushes.LawnGreen,
                StrokeLineJoin = PenLineJoin.Bevel,

                Data = geometry,
            };

            shape.SetZIndex((int) ZOrder.EditPreviewFigure);

            return shape;
        }
    }
}