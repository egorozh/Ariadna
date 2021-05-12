using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal sealed class ArcSegmentFigure : SegmentFigure
    {
        #region Private Fields

        private readonly RulerWorkspace _workspace;

        private readonly Path _shape;
        private bool _isElliptic;
        private ArcSegment _arcSegment;
        private Point _middlePoint;

        #endregion

        #region Public Properties

        public Point MiddlePoint
        {
            get => _middlePoint;
            set => SetMiddlePoint(value);
        }

        #endregion

        #region Constructors

        public ArcSegmentFigure(Point middlePoint, Point endPoint, SegmentFigure prevSegment,
            RulerWorkspace workspace) : base(new PointFigure(endPoint, workspace), prevSegment, workspace)
        {
            _workspace = workspace;

            _middlePoint = middlePoint;

            _shape = ShapeFactory.CreateEditArcSegment(false);

            _workspace.RulerCreator.DrawingControl.Canvas.AddElements(_shape);
            HelpPointFigure = new HelpPointFigure(Workspace, MiddlePoint, this);

            Update();
        }

        public override Point GetCenterPoint()
        {
            return _middlePoint;
        }

        public ArcSegmentFigure(ArcSegment arcSegment, SegmentFigure prevSegment, RulerWorkspace workspace) : base(
            new PointFigure(arcSegment.Point, workspace), prevSegment, workspace)
        {
            _workspace = workspace;

            if (Math.Abs(arcSegment.Size.Width - arcSegment.Size.Height) > 0.000001)
            {
                // Дуга - эллиптическая

                _isElliptic = true;
                _arcSegment = arcSegment;
            }
            else
            {
                _middlePoint = GeometryMath.GetMiddlePoint(arcSegment, PreviewPoint());

                HelpPointFigure = new HelpPointFigure(Workspace, _middlePoint, this);
            }

            _shape = ShapeFactory.CreateEditArcSegment(false);
            _workspace.RulerCreator.DrawingControl.Canvas.AddElements(_shape);

            Update();
        }

        #endregion

        #region Public Methods

        public override PathSegment GetSegment()
        {
            if (_isElliptic)
            {
                return _arcSegment;
            }

            return GeometryMath.GetArcSegment(PreviewPoint(), MiddlePoint, PointFigure.Point);
        }

        public override void SetCenterPoint(Point point)
        {
            var centerPoint = GetCenterPoint();

            var vector = point - centerPoint;

            PointFigure.Point += vector;

            if (PreviewSegment == null)
            {
                Workspace.StartPoint.Point += vector;
            }
            else
            {
                PreviewSegment.PointFigure.Point += vector;
            }

            MiddlePoint += vector;

            Update();
        }
        
        public override double GetLength()
        {
            var point1 = PreviewPoint();
            var point2 = PointFigure.Point;
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

        public override void UnDispose()
        {
            base.UnDispose();

            _workspace.RulerCreator.DrawingControl.Canvas.AddElements(_shape);

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
            var arcSegment = (ArcSegment) GetSegment();

            var figure = new PathFigure(start, new[] {arcSegment}, false);

            var pathRuler = new PathGeometry(new[] {figure});
            return pathRuler;
        }

        public override void Update()
        {
            base.Update();

            var arcSegment = (ArcSegment) GetSegment();

            var canvasArcSegment = new ArcSegment(
                _workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(PointFigure.Point),
                _workspace.RulerCreator.CoordinateSystem.GetCanvasSize(arcSegment.Size), arcSegment.RotationAngle,
                arcSegment.IsLargeArc, GetCanvasSweepDirection(arcSegment.SweepDirection),
                arcSegment.IsStroked);

            IEnumerable<PathSegment> segments = new[]
            {
                canvasArcSegment
            };

            var figures =
                new PathFigure(_workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(PreviewPoint()),
                    segments, false);
            var pathRuler = new PathGeometry(new[] {figures});
            _shape.Data = pathRuler;

            HelpPointFigure?.Update();
        }

        public override void Dispose()
        {
            base.Dispose();

            _workspace.RulerCreator.DrawingControl.Canvas.RemoveElements(_shape);
        }

        public bool IsValidNewPoint(Point point)
        {
            return !GeometryMath.IsThreePointsOnOneLine(MiddlePoint, PreviewPoint(), point);
        }

        public bool IsValidPrevPoint(Point nextPoint)
        {
            return !GeometryMath.IsThreePointsOnOneLine(MiddlePoint, nextPoint, PointFigure.Point);
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

        private void SetMiddlePoint(Point middlePoint)
        {
            if (GeometryMath.IsThreePointsOnOneLine(middlePoint, PreviewPoint(), PointFigure.Point))
                return;

            _middlePoint = middlePoint;
            HelpPointFigure.Point = middlePoint;
            //HelpPointFigure?.Update();

            Update();
        }

        private static SweepDirection GetCanvasSweepDirection(SweepDirection sweepDirection)
        {
            return sweepDirection == SweepDirection.Clockwise
                ? SweepDirection.Counterclockwise
                : SweepDirection.Clockwise;
        }

        private void SetColor()
        {
            _shape.Stroke = Brushes.DodgerBlue;
        }

        #endregion
    }
}