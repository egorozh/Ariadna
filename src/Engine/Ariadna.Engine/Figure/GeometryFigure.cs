using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using AKIM.Maths;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <summary>
    /// Фигура, основанная на <see cref="PathGeometry"/>
    /// </summary>
    public class GeometryFigure : SelectedFigure2D, IGeometryFigure
    {
        #region Private Fields

        private readonly Path _shape;

        private Path? _arrowShape;

        private PathGeometry? _geometry;
        private Brush? _fill;
        private Brush _stroke = new SolidColorBrush(Colors.Transparent);
        private Point _startPoint;
        private bool _isArrowOnEnd;

        #endregion

        #region Public Properties

        public Brush? Fill
        {
            get => _fill;
            set => SetBrush(value);
        }

        public Brush Stroke
        {
            get => _stroke;
            set => SetStroke(value);
        }

        public FrameworkElement? StartShape { get; init; }

        /// <summary>
        /// Геометрия фигуры в глобальных координатах
        /// </summary>
        public PathGeometry? Geometry
        {
            get => _geometry;
            set => SetGeometry(value);
        }

        public Point StartPoint
        {
            get => _startPoint;
            set
            {
                _startPoint = value;

                Update();
            }
        }

        public bool IsArrowOnEnd
        {
            get => _isArrowOnEnd;
            init => SetIsArrowOnEnd(value);
        }

        #endregion

        #region Events

        public event EventHandler<FigureDataEventArgs>? DataChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Дефолтный конструктор
        /// </summary>
        public GeometryFigure(IAriadnaEngine ariadnaEngine) : base(ariadnaEngine)
        {
            _shape = new Path
            {
                StrokeThickness = 2,
                Uid = Id,
                StrokeLineJoin = PenLineJoin.Bevel
            };

            _shape.SetZIndex(ZOrder);

            TransformAxis = new TransformAxis(true, true, true);
        }

        #endregion

        #region Public Methods

        protected override void OnShow()
        {
            base.OnShow();
            _shape.Visibility = Visibility.Visible;

            if (StartShape != null)
                StartShape.Visibility = Visibility.Visible;

            if (_arrowShape != null)
                _arrowShape.Visibility = Visibility.Visible;
        }

        protected override void OnHide()
        {
            base.OnHide();
            _shape.Visibility = Visibility.Collapsed;

            if (StartShape != null)
                StartShape.Visibility = Visibility.Collapsed;

            if (_arrowShape != null)
                _arrowShape.Visibility = Visibility.Collapsed;
        }

        public override Bounds GetCanvasBorders()
        {
            var transfGeometry = GetCanvasTransformGeometry();

            if (transfGeometry == null || transfGeometry.IsEmpty())
                return base.GetCanvasBorders();

            return new Bounds(transfGeometry.Bounds.Left, transfGeometry.Bounds.Bottom, transfGeometry.Bounds.Right,
                transfGeometry.Bounds.Top);
        }

        public override Bounds GetBorders()
        {
            var transfGeometry = GetTransformGeometry();

            if (transfGeometry == null || transfGeometry.IsEmpty())
                return base.GetBorders();

            return new Bounds(transfGeometry.Bounds.Left, transfGeometry.Bounds.Bottom, transfGeometry.Bounds.Right,
                transfGeometry.Bounds.Top);
        }

        public override void Draw()
        {
            _shape.AddToCanvas(AriadnaEngine.Canvas);
            _arrowShape?.AddToCanvas(AriadnaEngine.Canvas);
            StartShape?.AddToCanvas(AriadnaEngine.Canvas);

            if (IsShow)
                OnShow();
            else
                OnHide();

            base.Draw();
        }

        public override void Remove()
        {
            base.Remove();

            AriadnaEngine.Canvas.RemoveElements(_shape);

            if (StartShape != null)
                AriadnaEngine.Canvas.RemoveElements(StartShape);

            if (_arrowShape != null)
                AriadnaEngine.Canvas.RemoveElements(_arrowShape);
        }

        public override IntersectionDetail FillHitTest(Geometry geometry)
        {
            var transformedGeometry = GetTransformGeometry();

            if (transformedGeometry == null)
                return IntersectionDetail.NotCalculated;

            return transformedGeometry.FillContainsWithDetail(geometry);
        }

        public override IntersectionDetail StrokeHitTest(Geometry geometry, Pen pen)
        {
            var transformedGeometry = GetTransformGeometry();

            if (transformedGeometry == null)
                return IntersectionDetail.NotCalculated;

            return transformedGeometry.StrokeContainsWithDetail(pen, geometry);
        }

        #endregion

        #region Internal Methods

        protected override void Update()
        {
            base.Update();

            if (_geometry == null || _geometry.IsEmpty())
                return;

            _shape.Data = GetCanvasTransformGeometry();

            StartShape?.SetPosition(AriadnaEngine.CoordinateSystem.GetCanvasPoint(Transform.Transform(_startPoint)));

            if (_arrowShape != null)
                _arrowShape.Data = GetArrowGeometry();
        }

        protected override void SelectionAction(bool isAllfiguresUnselect)
        {
            if (_geometry == null || _geometry.IsEmpty())
                return;

            _shape.ChangeColor(AriadnaEngine.Settings.SelectedColor, IsFilled);

            _shape.SetZIndex(Core.ZOrder.SelectedFigure);

            if (StartShape != null)
            {
                StartShape.Opacity = 1;
                StartShape.Effect = new OrangescaleEffect();
                StartShape.SetZIndex(Core.ZOrder.SelectedFigure);
            }

            if (_arrowShape != null)
            {
                _arrowShape.Effect = new OrangescaleEffect();
                _arrowShape.Stroke.Opacity = 1;
                _arrowShape.SetZIndex(Core.ZOrder.SelectedFigure);
            }
        }

        protected override void UnselectionAction(bool isAllfiguresUnselect)
        {
            SetMainColor();

            if (!isAllfiguresUnselect)
            {
                _shape.Stroke.Opacity = 0.4;

                if (_fill != null)
                    _shape.Fill.Opacity = 0.4;
            }

            _shape.SetZIndex(ZOrder);

            if (StartShape != null)
            {
                StartShape.SetZIndex(ZOrder);

                if (isAllfiguresUnselect)
                {
                    StartShape.Effect = null;
                    StartShape.Opacity = 1;
                }
                else
                {
                    StartShape.Effect = null;
                    StartShape.Opacity = 0.4;
                }
            }

            if (_arrowShape != null)
            {
                _arrowShape.SetZIndex(ZOrder);

                if (isAllfiguresUnselect)
                {
                    _arrowShape.Effect = null;
                    _arrowShape.Opacity = 1;
                }
                else
                {
                    _arrowShape.Effect = null;
                    _arrowShape.Opacity = 0.4;
                }
            }
        }

        #endregion

        #region Private Methods

        private Geometry? GetArrowGeometry()
        {
            var pathGeometry = PathGeometry.CreateFromGeometry(GetTransformGeometry());
            var flatG = pathGeometry.GetFlattenedPathGeometry();

            var endPoint = AriadnaEngine.CoordinateSystem.GetCanvasPoint(flatG.GetEndPoint());
            var prevEndPoint = AriadnaEngine.CoordinateSystem.GetCanvasPoint(flatG.GetEndPoint(1));

            var vector = prevEndPoint - endPoint;

            vector.Normalize();

            const double delta = 16;

            var matrix = Matrix.Identity;
            matrix.Rotate(15);

            var vector1 = matrix.Transform(vector);

            matrix = Matrix.Identity;
            matrix.Rotate(-15);

            var vector2 = matrix.Transform(vector);

            vector1.Normalize();
            vector2.Normalize();

            var figures = new[]
            {
                new PathFigure(endPoint, new[]
                {
                    new LineSegment(endPoint + delta * vector1, true),
                    new LineSegment(endPoint + delta * vector2, true),
                }, true)
            };

            return new PathGeometry(figures);
        }

        private Geometry? GetTransformGeometry()
        {
            if (_geometry == null)
                return null;

            var transformedGeometry = _geometry.Clone();

            transformedGeometry.Transform = new MatrixTransform(Transform);

            return transformedGeometry;
        }

        private Geometry? GetCanvasTransformGeometry()
        {
            if (_geometry == null)
                return null;

            var transformedGeometry = _geometry.Clone();
            transformedGeometry.Transform = new MatrixTransform(GetCanvasTransform());

            return transformedGeometry;
        }

        private void SetBrush(Brush? brush)
        {
            _fill = brush;

            IsFilled = _fill != null;

            if (!IsSelected)
                SetMainColor();
        }

        private void SetStroke(Brush stroke)
        {
            _stroke = stroke;

            if (!IsSelected)
                SetMainColor();
        }

        private void SetGeometry(PathGeometry? geometry)
        {
            _geometry = geometry;

            Update();

            DataChanged?.Invoke(this, new FigureDataEventArgs());
        }

        private void SetMainColor()
        {
            _shape.Stroke = _stroke;
            _shape.Stroke.Opacity = 1;

            if (_fill != null)
            {
                _shape.Fill = _fill;
                _shape.Fill.Opacity = 1;
            }

            if (_arrowShape != null)
            {
                _arrowShape.Stroke = _stroke;
                _arrowShape.Fill = _stroke;
                _arrowShape.Stroke.Opacity = 1;
            }
        }

        private void SetIsArrowOnEnd(bool isArrowOnEnd)
        {
            _isArrowOnEnd = isArrowOnEnd;

            if (isArrowOnEnd)
            {
                _arrowShape = new Path
                {
                    StrokeThickness = 2,
                    Uid = Id,
                    StrokeLineJoin = PenLineJoin.Miter,
                };

                _arrowShape.SetZIndex(ZOrder);

                SetMainColor();
            }
        }

        #endregion
    }
}