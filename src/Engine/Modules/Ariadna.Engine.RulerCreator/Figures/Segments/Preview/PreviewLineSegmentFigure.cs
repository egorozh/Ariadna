using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal sealed class PreviewLineSegmentFigure : PreviewSegment
    {
        private readonly Line _shape;

        public PreviewLineSegmentFigure(Point endPoint, Point prevPoint, RulerWorkspace workspace) : base(workspace)
        {
            EndPoint = endPoint;
            PrevPoint = prevPoint;
            _shape = ShapeFactory.CreateEditLineSegment(true);
            EditEndPoint = ShapeFactory.CreateMouseCreatePoint();
            EditEndPoint.Visibility = Visibility.Visible;
            workspace.RulerCreator.DrawingControl.Canvas.AddElements(_shape, EditEndPoint, LengthTextBlock);

            UpdatePoint(endPoint);
        }

        public override void Dispose()
        {
            Workspace.RulerCreator.DrawingControl.Canvas.RemoveElements(_shape, EditEndPoint, LengthTextBlock);
        }

        public override void UpdatePoint(Point newPoint)
        {
            var cPoint = Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(newPoint);

            EditEndPoint.SetPosition(cPoint);

            EndPoint = newPoint;

            Update();
        }

        public override double GetLength()
        {
            var point1 = PrevPoint;
            var point2 = EndPoint;


            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        public override void Update()
        {
            base.Update();

            var prev = Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(PrevPoint);
            var end = Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(EndPoint);

            _shape.SetPoints(prev.X,prev.Y,end.X,end.Y);
        }

        public Point GetCenterPoint()
        {
            var point1 = PrevPoint;
            var point2 = EndPoint;

            return new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
        }

        public override PathSegment GetSegment(bool isFlip = false)
        {
            return isFlip ? new LineSegment(PrevPoint, true) : new LineSegment(EndPoint, true);
        }
    }
}