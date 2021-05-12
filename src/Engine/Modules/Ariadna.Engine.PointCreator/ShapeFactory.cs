using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.PointCreator
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
    }
}