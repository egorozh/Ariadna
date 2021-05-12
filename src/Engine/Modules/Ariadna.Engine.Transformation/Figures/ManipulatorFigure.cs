using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AKIM.Maths;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.Transformation
{
    internal class ManipulatorFigure : IManipulatorFigure
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;
        private readonly Ellipse _shape;
        private readonly Path _xArrowShape;
        private readonly Path _yArrowShape;
        private readonly Path _zRotateShape;

        private MouseOverMode _manipulatorMode = MouseOverMode.None;
        private Point _location;
        private readonly Cursor _rotateCursor;
        private bool _isManipulate;
        private Point _prevPoint;
        private Point _initCanvasPoint;

        private bool _isRotateVisible;
       
        #endregion

        #region Public Properties

        public Point Location => _location;

        public bool IsRotateVisible
        {
            get => _isRotateVisible;
            set
            {
                _isRotateVisible = value;
                _zRotateShape.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        
        #endregion

        #region Events

        public event EventHandler<ManipulateEventArgs>? Manipulate;

        public event EventHandler<ManipulateEventArgs>? ManipulateEnded;

        #endregion

        #region Constructor
        
        public ManipulatorFigure(IAriadnaEngine ariadnaEngine, bool isRotateVisible)
        {
            _ariadnaEngine = ariadnaEngine;

            #region Create & Init Shapes

            _shape = ShapeFactory.CreateEditPoint();
            _xArrowShape = ShapeFactory.CreateArrow();
            _yArrowShape = ShapeFactory.CreateArrow();
            _zRotateShape = ShapeFactory.CreateRotateArc();
            
            _shape.SetZIndex(ZOrder.EditRectanglePoint);
            _xArrowShape.SetZIndex(ZOrder.EditRectanglePoint - 1);
            _yArrowShape.SetZIndex(ZOrder.EditRectanglePoint - 1);
            _zRotateShape.SetZIndex(ZOrder.EditRectanglePoint - 2);

            #endregion

            var rotateCursorResource =
                Application.GetResourceStream(
                    new Uri("/Ariadna.Engine.Transformation;component/Resources/rotate_cursor.cur",
                        UriKind.Relative));

            _rotateCursor = rotateCursorResource != null ? new Cursor(rotateCursorResource.Stream) : Cursors.Arrow;

            ResetColorShapes();

            _ariadnaEngine.Canvas.AddElements(_shape, _xArrowShape, _yArrowShape, _zRotateShape);

            IsRotateVisible = isRotateVisible;
         
            ariadnaEngine.Canvas.MouseLeftButtonDown += CanvasLeftButtonDown;
            ariadnaEngine.Canvas.MouseMove += CanvasMouseMove;
            ariadnaEngine.Canvas.MouseLeftButtonUp += CanvasMouseLeftButtonUp;
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            _ariadnaEngine.Canvas.MouseLeftButtonDown -= CanvasLeftButtonDown;
            _ariadnaEngine.Canvas.MouseMove -= CanvasMouseMove;
            _ariadnaEngine.Canvas.MouseLeftButtonUp -= CanvasMouseLeftButtonUp;

            _ariadnaEngine.Canvas.RemoveElements(_shape, _xArrowShape, _yArrowShape, _zRotateShape);
        }

        public void Update(Matrix transform)
        {
            var locationX = transform.OffsetX;
            var locationY = transform.OffsetY;

            _location = new Point(locationX, locationY);

            var coordinateSystem = _ariadnaEngine.CoordinateSystem;

            var canvasP = coordinateSystem.GetCanvasPoint(_location);

            var canvasX = canvasP.X;
            var canvasY = canvasP.Y;

            _shape?.SetPosition(canvasX, canvasY);

            _xArrowShape.RenderTransform = new TranslateTransform(canvasX, canvasY);
            _zRotateShape.RenderTransform = new TranslateTransform(canvasX, canvasY);
            
            _yArrowShape.RenderTransform = new TransformGroup()
            {
                Children = new TransformCollection()
                {
                    new RotateTransform(-90),
                    new TranslateTransform(canvasX, canvasY),
                }
            };
        }

        #endregion

        #region Private Methods
        
        #region Down,Move,Up

        private void CanvasLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _initCanvasPoint = _prevPoint;

            if (_manipulatorMode != MouseOverMode.None)
            {
                _ariadnaEngine.SelectHelper.OffSelectedHelper();

                _isManipulate = true;
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            var canvasPoint = e.GetPosition(_ariadnaEngine.Canvas);

            if (_isManipulate)
            {
                var vector = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(canvasPoint) -
                             _ariadnaEngine.CoordinateSystem.GetGlobalPoint(_prevPoint);

                var matrix = Matrix.Identity;

                switch (_manipulatorMode)
                {
                    case MouseOverMode.DragX:

                        SetCursor(Cursors.Hand);

                        matrix.Translate(vector.X, 0);

                        Manipulate?.Invoke(this, new ManipulateEventArgs(matrix));

                        break;

                    case MouseOverMode.DragY:

                        SetCursor(Cursors.Hand);

                        matrix.Translate(0, vector.Y);

                        Manipulate?.Invoke(this, new ManipulateEventArgs(matrix));

                        break;

                    case MouseOverMode.DragXY:

                        SetCursor(Cursors.Hand);

                        matrix.Translate(vector.X, vector.Y);

                        Manipulate?.Invoke(this, new ManipulateEventArgs(matrix));

                        break;

                    case MouseOverMode.RotateZ:

                        SetCursor(_rotateCursor);

                        var prevVector = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(_prevPoint) - _location;
                        var curVector = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(canvasPoint) - _location;

                        var angle = Vector.AngleBetween(curVector, prevVector);

                        matrix.RotateAt(-angle, _location.X, _location.Y);

                        Manipulate?.Invoke(this, new ManipulateEventArgs(matrix));

                        break;
                }
            }
            else
            {
                ResetColorShapes();


                _manipulatorMode = GetMode(canvasPoint);

                switch (_manipulatorMode)
                {
                    case MouseOverMode.DragX:
                        _xArrowShape.Fill = Brushes.Yellow;
                        _xArrowShape.Stroke = Brushes.GreenYellow;

                        SetCursor(Cursors.Hand);

                        break;

                    case MouseOverMode.DragY:
                        _yArrowShape.Fill = Brushes.Yellow;
                        _yArrowShape.Stroke = Brushes.GreenYellow;

                        SetCursor(Cursors.Hand);

                        break;
                    case MouseOverMode.DragXY:
                        _shape.Fill = Brushes.Yellow;
                        _shape.Stroke = Brushes.GreenYellow;

                        SetCursor(Cursors.Hand);

                        break;
                    case MouseOverMode.RotateZ:
                        _zRotateShape.Fill = Brushes.Yellow;
                        _zRotateShape.Stroke = Brushes.GreenYellow;

                        SetCursor(_rotateCursor);

                        break;
                    case MouseOverMode.None:
                        break;
                }
            }

            _prevPoint = canvasPoint;
        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isManipulate)
            {
                var vector = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(_prevPoint) -
                             _ariadnaEngine.CoordinateSystem.GetGlobalPoint(_initCanvasPoint);

                var matrix = Matrix.Identity;

                switch (_manipulatorMode)
                {
                    case MouseOverMode.DragX:

                        matrix.Translate(vector.X, 0);

                        ManipulateEnded?.Invoke(this, new ManipulateEventArgs(matrix));

                        break;

                    case MouseOverMode.DragY:

                        matrix.Translate(0, vector.Y);

                        ManipulateEnded?.Invoke(this, new ManipulateEventArgs(matrix));

                        break;

                    case MouseOverMode.DragXY:

                        matrix.Translate(vector.X, vector.Y);

                        ManipulateEnded?.Invoke(this, new ManipulateEventArgs(matrix));

                        break;

                    case MouseOverMode.RotateZ:

                        var prevVector = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(_initCanvasPoint) - _location;
                        var curVector = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(_prevPoint) - _location;

                        var angle = Vector.AngleBetween(curVector, prevVector);

                        matrix.RotateAt(-angle, _location.X, _location.Y);

                        ManipulateEnded?.Invoke(this, new ManipulateEventArgs(matrix));

                        break;
                }


                _isManipulate = false;
                _ariadnaEngine.SelectHelper.OnSelectedHelper();
            }
        }

        #endregion

        private MouseOverMode GetMode(Point canvasPoint)
        {
            if (PointIsOverGeometry(canvasPoint, _shape.RenderedGeometry, 0,
                new Point(-_shape.Width / 2, -_shape.Height / 2)))
                return MouseOverMode.DragXY;

            if (PointIsOverGeometry(canvasPoint, _yArrowShape.Data, -90))
                return MouseOverMode.DragY;

            if (PointIsOverGeometry(canvasPoint, _zRotateShape.Data) && IsRotateVisible)
                return MouseOverMode.RotateZ;

            if (PointIsOverGeometry(canvasPoint, _xArrowShape.Data))
                return MouseOverMode.DragX;
            
            return MouseOverMode.None;
        }

        private bool PointIsOverGeometry(Point canvasPoint, Geometry geometry, double angle = 0,
            Point translate = new Point())
        {
            var pos = _ariadnaEngine.CoordinateSystem.GetCanvasPoint(_location);

            var ellipseGeometry = new EllipseGeometry(canvasPoint, 2, 2);

            var matrix = Matrix.Identity;
            matrix.Rotate(angle);
            matrix.Translate(pos.X, pos.Y);
            matrix.Translate(translate.X, translate.Y);

            var hitGeometry = PathGeometry.CreateFromGeometry(geometry).ToTransform(matrix);

            var result = hitGeometry.FillContainsWithDetail(ellipseGeometry);

            return result == IntersectionDetail.Intersects ||
                   result == IntersectionDetail.FullyContains ||
                   result == IntersectionDetail.FullyInside;
        }

        private void ResetColorShapes()
        {
            _xArrowShape.Fill = Brushes.Red;
            _xArrowShape.Stroke = Brushes.MediumVioletRed;

            _yArrowShape.Fill = Brushes.Green;
            _yArrowShape.Stroke = Brushes.ForestGreen;
            
            _zRotateShape.Fill = Brushes.Blue;
            _zRotateShape.Stroke = Brushes.RoyalBlue;

            _shape.Stroke = Brushes.DeepSkyBlue;
            _shape.Fill = Brushes.White;

            SetCursor(Cursors.Arrow);
        }

        private void SetCursor(Cursor cursor)
        {
            Mouse.SetCursor(cursor);
        }

        private Point CalcMagnetPoint(Point point, bool isShiftMode = false, Point prevPoint = new Point()) =>
            point.GetMagnetPoint(_ariadnaEngine.Settings, _ariadnaEngine.GridChart,
                _ariadnaEngine.Figures, _ariadnaEngine.CoordinateSystem,
                isShiftMode && Keyboard.IsKeyDown(Key.LeftShift), prevPoint);

        #endregion

        #region Structs

        private enum MouseOverMode
        {
            None,

            DragX,

            DragY,

            DragXY,

            RotateZ
        }

        #endregion
    }
}