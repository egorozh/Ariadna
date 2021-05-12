using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal sealed class ClosedSegmentFigure : Figure
    {
        #region Private Fields

        private readonly Line _shape;
        private bool _isVisibility;
        private Point _point1;
        private Point _point2;

        /// <summary>
        /// Длина сегмента
        /// </summary>
        public double Length => GetLength();

        /// <summary>
        /// Текст-блок для отображения длины сегмента
        /// </summary>
        public TextBlock LengthTextBlock { get; set; }

        #endregion

        #region Public Properties

        public bool IsVisibility
        {
            get => _isVisibility;
            set => SetIsVisibility(value);
        }

        public Point Point1
        {
            get => _point1;
            set => SetPoint1(value);
        }

        public Point Point2
        {
            get => _point2;
            set => SetPoint2(value);
        }

        #endregion

        #region Constructor

        public ClosedSegmentFigure(RulerWorkspace workspace) : base(workspace)
        {
            LengthTextBlock = new TextBlock
            {
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            LengthTextBlock.SetZIndex(ZOrder.RulerTextBox);

            LengthTextBlock.AddToCanvas(workspace.RulerCreator.DrawingControl.Canvas);
            _shape = ShapeFactory.CreateClosedSegment();
            _shape.AddToCanvas(Workspace.RulerCreator.DrawingControl.Canvas);

            Update();
        }

        #endregion

        #region Public Methods

        public override Geometry GetGeometry()
        {
            var figure = new PathFigure(Point1, new[]
            {
                new LineSegment(Point2, true)
            }, false);
            var pathRuler = new PathGeometry(new[] {figure});
            return pathRuler;
        }

        public double GetLength()
        {
            var point1 = _point1;
            var point2 = _point2;

            var vector = point2 - point1;

            return vector.Length;
        }


        public override void Update()
        {
            var point1 = Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(_point1);
            var point2 = Workspace.RulerCreator.CoordinateSystem.GetCanvasPoint(_point2);

            _shape.SetPoints(point1.X, (point1.Y), (point2.X), (point2.Y));

            UpdateTextBlock();
        }

        public void UpdateTextBlock()
        {
            LengthTextBlock.Text = Length.ToStr();

            LengthTextBlock.SetTextBlockOnCenter(_point1, GetCenterPoint(), _point2,
                Workspace.RulerCreator.CoordinateSystem);
        }

        public override void Dispose()
        {
            Workspace.RulerCreator.DrawingControl.Canvas.RemoveElements(_shape, LengthTextBlock);
        }

        public override void UnDispose()
        {
            Workspace.RulerCreator.DrawingControl.Canvas.AddElements(_shape, LengthTextBlock);

            Update();
        }

        public Point GetCenterPoint() => new Point((_point1.X + _point2.X) / 2, (_point1.Y + _point2.Y) / 2);

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

        private void SetPoint1(Point value)
        {
            _point1 = value;

            Update();
        }

        private void SetPoint2(Point value)
        {
            _point2 = value;

            Update();
        }

        private void SetColor()
        {
            _shape.Stroke = Brushes.DodgerBlue;
        }

        private void SetIsVisibility(bool value)
        {
            _isVisibility = value;

            _shape.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}