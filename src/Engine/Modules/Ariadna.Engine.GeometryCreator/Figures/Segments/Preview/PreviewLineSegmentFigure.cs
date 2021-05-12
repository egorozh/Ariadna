using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.GeometryCreator
{
    internal sealed class PreviewLineSegmentFigure : PreviewSegment
    {
        private readonly Point _prevPoint;
        private readonly GeometryWorkspace _workspace;

        private readonly Line _shape;
        private readonly Rectangle _editEndPoint;

        public PreviewLineSegmentFigure(Point endPoint, Point prevPoint, GeometryWorkspace workspace)
        {
            EndPoint = endPoint;
            _prevPoint = prevPoint;
            _workspace = workspace;

            _shape = ShapeFactory.CreateEditLineSegment(true);

            _editEndPoint = ShapeFactory.CreateMouseCreatePoint();
            _editEndPoint.Visibility = Visibility.Visible;
            

            _shape.AddToCanvas(_workspace.GeometryCreator.DrawingControl.Canvas);
            _editEndPoint.AddToCanvas(_workspace.GeometryCreator.DrawingControl.Canvas);


            UpdatePoint(endPoint);
        }

        public override void Dispose()
        {
            _workspace.GeometryCreator.DrawingControl.Canvas.RemoveElements(_shape, _editEndPoint);
        }

        public override void UpdatePoint(Point newPoint)
        {
            var point = _workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(newPoint);

            _editEndPoint.SetPosition(point.X,point.Y);

            EndPoint = newPoint;

            Update();
        }

        public override void Update()
        {
            var prevPoint = _workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(_prevPoint);

            var endPoint = _workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(EndPoint);

            _shape.SetPoints(prevPoint.X,prevPoint.Y,endPoint.X,endPoint.Y);
        }

        public override PathSegment GetSegment(bool isFlip = false)
        {
            return isFlip ? new LineSegment(_prevPoint, true) : new LineSegment(EndPoint, true);
        }
    }
}