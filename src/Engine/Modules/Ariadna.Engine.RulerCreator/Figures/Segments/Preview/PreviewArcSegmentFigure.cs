using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal sealed class PreviewArcSegmentFigure : PreviewSegment
    {
        #region Private Fields

        private readonly Point _middleArcPoint;

        private ArcSegment _arcSegment;
        private readonly Path _shape;
        private readonly Ellipse _editMiddlePoint;

        #endregion

        #region Constructor

        public PreviewArcSegmentFigure(Point middleArcPoint, Point endPoint, Point prevPoint,
            RulerWorkspace workspace) : base(workspace)
        {
            _middleArcPoint = middleArcPoint;
            EndPoint = endPoint;
            PrevPoint = prevPoint;

            _arcSegment = GeometryMath.GetArcSegment(prevPoint, middleArcPoint, endPoint);
            _shape = ShapeFactory.CreateEditArcSegment(true);

            _editMiddlePoint = ShapeFactory.CreateEditHelpNodePoint();
            EditEndPoint = ShapeFactory.CreateMouseCreatePoint();
            EditEndPoint.Visibility = Visibility.Visible;

            Workspace.RulerCreator.DrawingControl.Canvas.AddElements(_shape, _editMiddlePoint, EditEndPoint,LengthTextBlock);

            Update();
        }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            Workspace.RulerCreator.DrawingControl.Canvas.RemoveElements(_shape, _editMiddlePoint, EditEndPoint,LengthTextBlock);
        }

        public override void UpdatePoint(Point newPoint)
        {
            if (GeometryMath.IsThreePointsOnOneLine(_middleArcPoint, PrevPoint, newPoint))
                return;

            EndPoint = newPoint;
            Update();
        }

        public override Point GetCenterPoint()
        {
            return _middleArcPoint;
        }

        public override double GetLength()
        {
            var point1 = EndPoint;
            var point2 = PrevPoint;
            var radius = GeometryMath.GetRadius(GetCenterPoint(), point1, point2);
            var chord = Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
            var angle = Math.Asin(chord / (2 * radius));
            

            double length;

            if (((ArcSegment) GetSegment()).IsLargeArc)
                length = (2 * radius *
                          Math.PI) - (2 * radius * angle);
            else
                length = (2 * radius) * angle;
            
            return double.IsNaN(length)? 0.0 : length;
        }

        public override void Update()
        {
            base.Update();
            
            _arcSegment = GeometryMath.GetArcSegment(PrevPoint, _middleArcPoint, EndPoint);

            var endPoint = Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(EndPoint);
            var middleArcPoint = Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(_middleArcPoint);

            EditEndPoint.SetPosition(endPoint);
            _editMiddlePoint.SetPosition(middleArcPoint);
            
            var canvasArcSegment = new ArcSegment(Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(EndPoint),
                Workspace.RulerCreator.CoordinateSystem.GetCanvasSize(_arcSegment.Size), _arcSegment.RotationAngle,
                _arcSegment.IsLargeArc, GetCanvasSweepDirection(_arcSegment.SweepDirection),
                _arcSegment.IsStroked);

            IEnumerable<PathSegment> segments = new[]
            {
                canvasArcSegment
            };

            var figures =
                new PathFigure(Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(PrevPoint),
                    segments, false);
            var pathRuler = new PathGeometry(new[] {figures});
            _shape.Data = pathRuler;
        }

        public override PathSegment GetSegment(bool isFlip = false)
        {
            return isFlip
                ? GeometryMath.GetArcSegment(EndPoint, _middleArcPoint, PrevPoint)
                : GeometryMath.GetArcSegment(PrevPoint, _middleArcPoint, EndPoint);
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