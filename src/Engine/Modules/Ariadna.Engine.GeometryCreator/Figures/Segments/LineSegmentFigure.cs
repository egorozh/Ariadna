using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.GeometryCreator
{
    /// <summary>
    /// Линейный сегмент
    /// </summary>
    internal sealed class LineSegmentFigure : SegmentFigure
    {
        #region Private Fields

        private readonly Line _shape;

        #endregion

        #region Constructor

        public LineSegmentFigure(Point endPoint, SegmentFigure prevSegment, GeometryWorkspace workspace) : base(
            new PointFigure(endPoint, workspace), prevSegment, workspace)
        {
            _shape = ShapeFactory.CreateEditLineSegment(false);
            _shape.AddToCanvas(Workspace.GeometryCreator.DrawingControl.Canvas);

            HelpPointFigure = new HelpPointFigure(Workspace, GetCenterPoint(), this);

            Update();
        }

        #endregion

        #region Public Methods

        public override PathSegment GetSegment() => new LineSegment(PointFigure.Point, true);

        public override void UnDispose()
        {
            base.UnDispose();

            Workspace.GeometryCreator.DrawingControl.Canvas.AddElements(_shape);

            Update();
        }

        public override void Highlight(bool isHighlight)
        {
            if (IsSelected)
                return;

            if (isHighlight)
            {
                _shape.Stroke = Brushes.LightSkyBlue;
            }
            else
                SetColor();
        }

        public override Geometry GetGeometry()
        {
            var start = PreviewPoint();
            var figure = new PathFigure(start, new[] {GetSegment()}, false);
            var pathGeometry = new PathGeometry(new[] {figure});
            return pathGeometry;
        }

        public override void Update()
        {
            base.Update();

            var start = Workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(PreviewPoint());
            
            var point = Workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(PointFigure.Point);

            _shape.SetPoints(start.X, start.Y, point.X, point.Y);

            HelpPointFigure.Point = GetCenterPoint();
            HelpPointFigure?.Update();
        }

        public override void Dispose()
        {
            base.Dispose();

            Workspace.GeometryCreator.DrawingControl.Canvas.RemoveElements(_shape);
        }

        #endregion

        #region Protected Methods

        protected override void SelectChanged(bool isSelected)
        {
            if (isSelected)
            {
                _shape.Stroke = Brushes.Orange;
            }
            else
                SetColor();
        }

        #endregion

        #region Private Methods

        private void SetColor()
        {
            _shape.Stroke = Brushes.DodgerBlue;
        }

        #endregion
    }
}