using System;
using System.Windows;
using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    public interface IGeometryFigure : ISelectedFigure2D, IColorFigure
    {
        #region Properties

        /// <summary>
        /// Геометрия
        /// </summary>
        PathGeometry? Geometry { get; set; }

        Point StartPoint { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Геометрия изменилась
        /// </summary>
        event EventHandler<FigureDataEventArgs> DataChanged;

        #endregion
    }
}