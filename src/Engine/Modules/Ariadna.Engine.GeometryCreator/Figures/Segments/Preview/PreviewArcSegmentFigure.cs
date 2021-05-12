using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.GeometryCreator
{
    internal sealed class PreviewArcSegmentFigure : PreviewSegment
    {
        #region Private Fields

        private readonly Point _middleArcPoint;

        private readonly Point _prevPoint;
        private readonly GeometryWorkspace _workspace;


        private readonly Shape _editEndPoint;
        private ArcSegment _arcSegment;
        private readonly Path _shape;
        private readonly Ellipse _editMiddlePoint;

        #endregion

        #region Constructor

        public PreviewArcSegmentFigure(Point middleArcPoint, Point endPoint, Point prevPoint,
            GeometryWorkspace workspace)
        {
            _middleArcPoint = middleArcPoint;
            EndPoint = endPoint;
            _prevPoint = prevPoint;
            _workspace = workspace;

            _arcSegment = GeometryMath.GetArcSegment(prevPoint, middleArcPoint, endPoint);
            _shape = ShapeFactory.CreateEditArcSegment(true);

            _editMiddlePoint = ShapeFactory.CreateEditHelpNodePoint();
            _editEndPoint = ShapeFactory.CreateMouseCreatePoint();
            _editEndPoint.Visibility = Visibility.Visible;

            workspace.GeometryCreator.DrawingControl.Canvas.AddElements(_shape, _editMiddlePoint, _editEndPoint);

            Update();
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            _workspace.GeometryCreator.DrawingControl.Canvas.RemoveElements(_shape, _editMiddlePoint, _editEndPoint);
        }

        public override void UpdatePoint(Point newPoint)
        {
            if (GeometryMath.IsThreePointsOnOneLine(_middleArcPoint, _prevPoint, newPoint))
                return;

            EndPoint = newPoint;
            Update();
        }

        public override void Update()
        {
            _arcSegment = GeometryMath.GetArcSegment(_prevPoint, _middleArcPoint, EndPoint);

            var endPoint = _workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(EndPoint);
            var middlePoint = _workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(_middleArcPoint);

            _editEndPoint.SetPosition(endPoint);
            _editMiddlePoint.SetPosition(middlePoint);
            
            var canvasArcSegment = new ArcSegment(_workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(EndPoint),
                _workspace.GeometryCreator.CoordinateSystem.GetCanvasSize(_arcSegment.Size), _arcSegment.RotationAngle,
                _arcSegment.IsLargeArc, GetCanvasSweepDirection(_arcSegment.SweepDirection),
                _arcSegment.IsStroked);

            IEnumerable<PathSegment> segments = new[]
            {
                canvasArcSegment
            };

            var figures =
                new PathFigure(_workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(_prevPoint),
                    segments, false);
            var pathGeometry = new PathGeometry(new[] {figures});
            _shape.Data = pathGeometry;
        }

        public override PathSegment GetSegment(bool isFlip = false)
        {
            return isFlip
                ? GeometryMath.GetArcSegment(EndPoint, _middleArcPoint, _prevPoint)
                : GeometryMath.GetArcSegment(_prevPoint, _middleArcPoint, EndPoint);
        }

        #endregion

        #region Private Methods

        private static SweepDirection GetCanvasSweepDirection(SweepDirection sweepDirection)
        {
            return sweepDirection == SweepDirection.Clockwise
                ? SweepDirection.Counterclockwise
                : SweepDirection.Clockwise;
        }

        #endregion
    }
}