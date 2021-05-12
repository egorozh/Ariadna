using System.Windows;
using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Создатель фигур для 2D движка
    /// </summary>
    public interface IFigureCreator : IEngineComponent
    {
        /// <summary>
        /// Создание фигуры в виде геометрии
        /// </summary>
        /// <returns></returns>
        IGeometryFigure CreateGeometryFigure(Geometry data, Brush stroke,
            Matrix transform, int zOrder = 2, Brush? fill = null,
            FrameworkElement? startShape = null, Point startPoint = new(), bool isArrowOnEnd = false);

        IPointFigure CreatePointFigure(Matrix transform, FrameworkElement shape, int zOrder = 2);
    }
}