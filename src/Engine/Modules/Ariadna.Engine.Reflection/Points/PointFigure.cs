using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.Reflection
{
    internal sealed class PointFigure : Figure
    {
        #region Private Fields

        private readonly Shape _shape = ShapeFactory.CreateEditNodePoint();

        private bool _isEdgePoint;
        private Point _point;

        #endregion

        #region Public Properties

        public Point Point
        {
            get => _point;
            set => SetPoint(value);
        }

        /// <summary>
        /// Точка является краевой
        /// </summary>
        public bool IsEdgePoint
        {
            get => _isEdgePoint;
            set => SetIsEdgePoint(value);
        }

        #endregion

        #region Constructor

        public PointFigure(Point point, ReflectionWorkspace workspace) : base(workspace)
        {
            _point = point;
            _shape.AddToCanvas(Workspace.ReflectionManager.DrawingControl.Canvas);

            Update();

            SetColor(IsEdgePoint);
        }

        #endregion

        #region Public Methods

       

        public override Geometry GetGeometry() =>
            new EllipseGeometry(Point, Workspace.ReflectionManager.CoordinateSystem.GetGlobalLength(_shape.Width),
                Workspace.ReflectionManager.CoordinateSystem.GetGlobalLength(_shape.Width));

        public override void Update()
        {
            _shape.SetPosition(Workspace.ReflectionManager.CoordinateSystem.GetCanvasPoint(Point));
        }

        public override void Dispose()
        {
            Workspace.ReflectionManager.DrawingControl.Canvas.RemoveElements(_shape);
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
                SetColor(IsEdgePoint);
        }

        public override void UnDispose()
        {
            Workspace.ReflectionManager.DrawingControl.Canvas.AddElements(_shape);

            IsSelected = false;

            Update();
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
                SetColor(IsEdgePoint);
        }

        #endregion

        #region Private Methods

        private void SetIsEdgePoint(bool isEdgePoint)
        {
            _isEdgePoint = isEdgePoint;

            if (IsSelected)
            {
                _shape.Stroke = Brushes.Orange;
                _shape.Fill = Brushes.Orange;
            }
            else
                SetColor(isEdgePoint);
        }

        private void SetColor(bool isEdge)
        {
            if (isEdge)
            {
                _shape.Stroke = Brushes.Red;
                _shape.Fill = Brushes.Red;
                _shape.Width = 12;
                _shape.Height = 12;
            }
            else
            {
                _shape.Stroke = Brushes.DodgerBlue;
                _shape.Fill = Brushes.DodgerBlue;
                _shape.Width = 10;
                _shape.Height = 10;
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