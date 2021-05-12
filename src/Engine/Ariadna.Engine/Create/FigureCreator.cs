using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    internal class FigureCreator : IFigureCreator
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;

        #endregion

        #region Constructor

        /// <summary>
        /// Дефолтный конструктор
        /// </summary>
        public FigureCreator(IAriadnaEngine ariadnaEngine)
        {
            _ariadnaEngine = ariadnaEngine;
        }

        #endregion

        #region Implemented Methods

        public IGeometryFigure CreateGeometryFigure(Geometry data, Brush stroke,
            Matrix transform, int zOrder = 2, Brush fill = null,
            FrameworkElement? startShape = null, Point startPoint = new(), bool isArrowOnEnd = false)
        {
            var pathFigure = new GeometryFigure(_ariadnaEngine)
            {
                StartPoint = startPoint,
                StartShape = startShape,
                Fill = fill,
                Stroke = stroke,
                Geometry = PathGeometry.CreateFromGeometry(data),
                ZOrder = zOrder,
                Transform = transform,
                IsArrowOnEnd = isArrowOnEnd
            };

            return pathFigure;
        }

        public IGroupFigure CreateGroupElementFigure(Matrix transform, IReadOnlyList<IFigure2D> figures)
        {
            var groupFigure = new GroupFigure(_ariadnaEngine)
            {
                Transform = transform,
                Figures = figures,
            };

            return groupFigure;
        }

        public IPointFigure CreatePointFigure(Matrix transform, FrameworkElement shape, int zOrder = 2)
        {
            var pointFigure = new PointFigure(_ariadnaEngine)
            {
                Transform = transform,
                ZOrder = zOrder,
                Shape = shape
            };

            return pointFigure;
        }
     
        #endregion

        public void Init()
        {
        }
    }
}