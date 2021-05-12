using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
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

        public LineSegmentFigure(Point endPoint, SegmentFigure prevSegment, RulerWorkspace workspace) : base(
            new PointFigure(endPoint, workspace), prevSegment, workspace)
        {
            _shape = ShapeFactory.CreateEditLineSegment(false);
            _shape.AddToCanvas(Workspace.RulerCreator.DrawingControl.Canvas);

            HelpPointFigure = new HelpPointFigure(Workspace, GetCenterPoint(), this);

            Update();
        }

        #endregion

        #region Public Methods

        public override PathSegment GetSegment() => new LineSegment(PointFigure.Point, true);

        public override void UnDispose()
        {
            base.UnDispose();

            Workspace.RulerCreator.DrawingControl.Canvas.AddElements(_shape);

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
            var pathRuler = new PathGeometry(new[] {figure});
            return pathRuler;
        }

        public override double GetLength()
        {
            var point1 = PreviewPoint();
            var point2 = PointFigure.Point;

            var vector = point2 - point1;

            return vector.Length;
        }

        public override void Update()
        {
            base.Update();

            var start = Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(PreviewPoint());
            var end = Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(PointFigure.Point);
            
            _shape.SetPoints(start.X,start.Y,end.X,end.Y);
            
            HelpPointFigure.Point = GetCenterPoint();
            HelpPointFigure?.Update();
        }

        public override void Dispose()
        {
            base.Dispose();

            Workspace.RulerCreator.DrawingControl.Canvas.RemoveElements(_shape);
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