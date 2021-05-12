using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.GeometryCreator
{
    internal sealed class HelpPointFigure : Figure
    {
        #region Private Fields

        private readonly Shape _shape = ShapeFactory.CreateEditMiddlePoint();
        private Point _point;
        private readonly SegmentFigure _parent;

        #endregion

        #region Public Properties

        public Point Point
        {
            get => _point;
            set => SetPoint(value);
        }

        #endregion

        #region Constructor

        public HelpPointFigure(GeometryWorkspace workspace, Point point, SegmentFigure parent) : base(workspace)
        {
            _point = point;
            _parent = parent;
            _shape.AddToCanvas(Workspace.GeometryCreator.DrawingControl.Canvas);

            Update();

            SetColor(Workspace.GeometryCreator.CreationMode);
        }

        #endregion

        #region Public Methods

        public override Geometry GetGeometry() => new EllipseGeometry(Point,
            Workspace.GeometryCreator.CoordinateSystem.GetGlobalLength(_shape.Width),
            Workspace.GeometryCreator.CoordinateSystem.GetGlobalLength(_shape.Width));

        public override void Update()
        {
            _shape.SetPosition(Workspace.GeometryCreator.CoordinateSystem.GetCanvasPoint(Point));

            if (_parent is ArcSegmentFigure)
            {
                _shape.Visibility = Workspace.GeometryCreator.CreationMode == CreationMode.Line
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }

            if (!IsSelected)
                SetColor(Workspace.GeometryCreator.CreationMode);
        }

        public override void Dispose()
        {
            Workspace.GeometryCreator.DrawingControl.Canvas.RemoveElements(_shape);
        }

        public override void UnDispose()
        {
            Workspace.GeometryCreator.DrawingControl.Canvas.AddElements(_shape);

            IsSelected = false;

            Update();
        }

        public override void Highlight(bool isHighlight)
        {
            if (IsSelected)
                return;

            if (isHighlight)
            {
                _shape.Stroke = Brushes.LightSkyBlue;
                _shape.Fill = Brushes.LightSkyBlue;
            }
            else
                SetColor(Workspace.GeometryCreator.CreationMode);
        }

        #endregion

        #region Protected Methods

        protected override void SelectChanged(bool isSelected)
        {
            if (isSelected)
            {
                _shape.Stroke = Brushes.Orange;
                _shape.Fill = Brushes.Orange;
            }
            else
                SetColor(Workspace.GeometryCreator.CreationMode);
        }

        #endregion

        #region Private Methods

        private void SetColor(CreationMode mode)
        {
            if (mode == CreationMode.Line)
            {
                _shape.Stroke = Brushes.RoyalBlue;
                _shape.Fill = Brushes.RoyalBlue;
            }
            else
            {
                _shape.Stroke = Brushes.LimeGreen;
                _shape.Fill = Brushes.LimeGreen;
            }
        }

        private void SetPoint(Point value)
        {
            _point = value;

            Update();
        }

        #endregion
    }
}