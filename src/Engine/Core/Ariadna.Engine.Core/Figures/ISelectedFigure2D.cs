using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Интерфейс фигуры, которую можно выделять
    /// </summary>
    public interface ISelectedFigure2D : IFigure2D
    {
        #region Properties

        bool IsSelected { get; }

        bool IsFilled { get; set; }

        #endregion

        #region Methods

        bool IsHitTest(Geometry intersectGeometry, IntersectSelectionMode mode = IntersectSelectionMode.Intersect);

        #endregion
    }
}