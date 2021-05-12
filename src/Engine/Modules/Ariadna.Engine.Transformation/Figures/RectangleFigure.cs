using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Ariadna.Engine.Transformation
{
    internal class RectangleFigure : IRectangleFigure
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;

        private readonly Point _center;
        private readonly Point _leftTop;
        private readonly Point _rightBottom;
        private readonly Point _leftBottom;
        private readonly Point _rightTop;

        private readonly bool _visible;
        private readonly bool _centerVisible;

        private Matrix _transform;

        #region Shapes

        private readonly Path _mainRectangle;

        private readonly Rectangle _leftBottomShape;
        private readonly Rectangle _leftTopShape;
        private readonly Rectangle _rightBottomShape;
        private readonly Rectangle _rightTopShape;

        private readonly Rectangle _leftShape;
        private readonly Rectangle _topShape;
        private readonly Rectangle _rightShape;
        private readonly Rectangle _bottomShape;

        private readonly Ellipse _centerShape;

        #endregion

        #region Cursors

        private readonly Path _rotateCursor;
        private readonly Path _scaleCursor;

        #endregion

        private MouseOverMode _mode = MouseOverMode.None;
        private Point _lastCanvasPosition;
        private Point _cashPoint;
        private Point _cashCornerPoint;

        #endregion

        #region Events

        public event EventHandler<ManipulateEventArgs>? Manipulate;
        public event EventHandler<ManipulateEventArgs>? ManipulateEnded;

        #endregion

        #region Constructor

        public RectangleFigure(IAriadnaEngine ariadnaEngine, bool isVisible, bool centerVisible, double width, double height,
            Point center)
        {
            _ariadnaEngine = ariadnaEngine;
            _visible = isVisible;
            _centerVisible = centerVisible;

            _transform = Matrix.Identity;

            _center = ariadnaEngine.CoordinateSystem.GetGlobalPoint(center);

            Point leftTop = new(center.X - width / 2, center.Y - height / 2);
            Point rightBottom = new(center.X + width / 2, center.Y + height / 2);
            Point leftBottom = new(center.X - width / 2, center.Y + height / 2);
            Point rightTop = new(center.X + width / 2, center.Y - height / 2);

            _leftTop = ariadnaEngine.CoordinateSystem.GetGlobalPoint(leftTop);
            _rightBottom = ariadnaEngine.CoordinateSystem.GetGlobalPoint(rightBottom);
            _leftBottom = ariadnaEngine.CoordinateSystem.GetGlobalPoint(leftBottom);
            _rightTop = ariadnaEngine.CoordinateSystem.GetGlobalPoint(rightTop);

            // Rotate Cursor:
            _rotateCursor = ShapeFactory.CreateRotateCursor();
            _scaleCursor = ShapeFactory.CreateScaleCursor();

            #region Create Shapes

            // Rectangle:
            _mainRectangle = ShapeFactory.CreateEditMainRectangle();

            // Corners:
            _leftBottomShape = ShapeFactory.CreateEditRectanglePoint();
            _leftTopShape = ShapeFactory.CreateEditRectanglePoint();

            _rightBottomShape = ShapeFactory.CreateEditRectanglePoint();
            _rightTopShape = ShapeFactory.CreateEditRectanglePoint();

            // Center Side Points:
            _leftShape = ShapeFactory.CreateEditRectanglePoint();
            _topShape = ShapeFactory.CreateEditRectanglePoint();
            _rightShape = ShapeFactory.CreateEditRectanglePoint();
            _bottomShape = ShapeFactory.CreateEditRectanglePoint();

            // Center Point:
            _centerShape = ShapeFactory.CreateEditCenterPoint();

            #endregion

            // Add Elements:
            _ariadnaEngine.Canvas.AddElements(_rotateCursor, _scaleCursor, _mainRectangle, _leftBottomShape, _leftTopShape,
                _rightBottomShape, _rightTopShape, _leftShape, _topShape, _rightShape, _bottomShape, _centerShape);

            if (!centerVisible)
                _centerShape.Visibility = Visibility.Collapsed;

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

            _ariadnaEngine.Canvas.RemoveElements(_rotateCursor, _scaleCursor, _mainRectangle, _leftBottomShape, _leftTopShape,
                _rightBottomShape, _rightTopShape, _leftShape, _topShape, _rightShape, _bottomShape, _centerShape);
        }

        public void Update()
        {
            if (!_visible)
                return;

            var canvasTransform = _transform;
            canvasTransform.Append(_ariadnaEngine.CoordinateSystem.GetCanvasMatrixTransform());

            var center = canvasTransform.Transform(_center);

            var leftTop = canvasTransform.Transform(_leftTop);
            var rightBottom = canvasTransform.Transform(_rightBottom);
            var leftBottom = canvasTransform.Transform(_leftBottom);
            var rightTop = canvasTransform.Transform(_rightTop);

            _mainRectangle.Data = new PathGeometry(new[]
            {
                new PathFigure(leftTop, new[]
                {
                    new PolyLineSegment(new[]
                    {
                        rightTop, rightBottom, leftBottom
                    }, true)
                }, true)
            });

            _leftBottomShape.SetPosition(leftBottom);
            _leftTopShape.SetPosition(leftTop);
            _rightBottomShape.SetPosition(rightBottom);
            _rightTopShape.SetPosition(rightTop);

            _leftShape.SetPosition(new((leftTop.X + leftBottom.X) / 2, (leftTop.Y + leftBottom.Y) / 2));
            _rightShape.SetPosition(new((rightBottom.X + rightTop.X) / 2, (rightBottom.Y + rightTop.Y) / 2));
            _topShape.SetPosition(new((leftTop.X + rightTop.X) / 2, (leftTop.Y + rightTop.Y) / 2));
            _bottomShape.SetPosition(new((rightBottom.X + leftBottom.X) / 2, (rightBottom.Y + leftBottom.Y) / 2));

            _centerShape.SetPosition(new(center.X, center.Y));

            var angle = Vector.AngleBetween(leftTop - rightTop, new Vector(1, 0));

            RotateShapes(angle, _leftBottomShape, _leftTopShape, _rightBottomShape,
                _rightTopShape, _leftShape, _topShape, _rightShape, _bottomShape, _centerShape);
        }

        public void ChangeTransform(Matrix transform)
        {
            _transform.Append(transform);
            Update();
        }

        #endregion

        #region Private Methods

        private void CanvasLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cursorPos = e.GetPosition(_ariadnaEngine.Canvas);

            var mode = GetMode(cursorPos);

            if (mode != MouseOverMode.None)
            {
                if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                    _ariadnaEngine.SelectHelper.OffSelectedHelper();

                _ariadnaEngine.CoordinateSystem.OnMoving();

                _cashPoint = cursorPos;
                _lastCanvasPosition = cursorPos;

                _cashCornerPoint = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(GetCorner(mode));
            }

            _mode = mode;
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            var canvasPoint = e.GetPosition(_ariadnaEngine.Canvas);

            switch (_mode)
            {
                case MouseOverMode.None:
                    DefaultMoveAction(canvasPoint);
                    break;
                case MouseOverMode.Drag:
                    DragMoveAction(canvasPoint);
                    break;
                case MouseOverMode.Rotate:
                    RotateMoveAction(canvasPoint);
                    break;
                default:
                    ScaleMoveAction(canvasPoint);
                    break;
            }
        }

        private void CanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cursorPoint = e.GetPosition(_ariadnaEngine.Canvas);

            if (_mode != MouseOverMode.None)
            {
                switch (_mode)
                {
                    case MouseOverMode.Drag:
                        DragUpAction();
                        break;
                    case MouseOverMode.Rotate:
                        RotateUpAction(cursorPoint);
                        break;
                    default:
                        ScaleUpAction(cursorPoint);
                        break;
                }

                NonDefaultUpAction();
            }
        }

        #endregion

        #region Move

        private void DefaultMoveAction(Point cursorPoint)
        {
            var centerPoint = _centerShape.GetPosition();

            var mode = GetMode(cursorPoint);

            switch (mode)
            {
                case MouseOverMode.Drag:
                    SetCursor(Cursors.SizeAll);
                    break;
                case MouseOverMode.Rotate:
                    SetRotateCursor(cursorPoint, centerPoint);
                    break;
                case MouseOverMode.None:
                    SetCursor(Cursors.Arrow);
                    break;
                default:
                    SetScaleCursor(cursorPoint, mode);
                    break;
            }
        }

        private void DragMoveAction(Point canvasPoint)
        {
            canvasPoint = Keyboard.IsKeyDown(Key.LeftShift)
                ? canvasPoint.GetMagnetPoint(_ariadnaEngine, true, _cashPoint, 4)
                : canvasPoint;

            if (canvasPoint != _lastCanvasPosition)
            {
                var gCanvasPoint = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(canvasPoint);
                var glastCanvasPoint = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(_lastCanvasPosition);

                var offsetX = gCanvasPoint.X - glastCanvasPoint.X;
                var offsetY = gCanvasPoint.Y - glastCanvasPoint.Y;

                var matrix = Matrix.Identity;

                matrix.Translate(offsetX, offsetY);

                ChangeTransform(matrix);
                Manipulate?.Invoke(this, new ManipulateEventArgs(matrix));
            }

            SetCursor(Cursors.SizeAll);

            _lastCanvasPosition = canvasPoint;
        }

        private void RotateMoveAction(Point canvasPoint)
        {
            var centerPoint = _centerShape.GetPosition();

            var vector = canvasPoint - centerPoint;

            if (canvasPoint != _lastCanvasPosition)
            {
                var angle = Vector.AngleBetween(vector,
                    new Vector(_lastCanvasPosition.X - centerPoint.X, _lastCanvasPosition.Y - centerPoint.Y));

                var matrix = Matrix.Identity;

                var globalCenter = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(centerPoint);

                matrix.RotateAt(angle, globalCenter.X, globalCenter.Y);

                ChangeTransform(matrix);
                Manipulate?.Invoke(this, new ManipulateEventArgs(matrix));
            }

            SetRotateCursor(canvasPoint, centerPoint);

            _lastCanvasPosition = canvasPoint;
        }

        private void ScaleMoveAction(Point canvasPoint)
        {
            var cornerPoint = GetCorner(_mode);
            var scaleCenter = GetScaleCenter(_mode);

            var globalScaleCenter = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(scaleCenter);
            var globalCursor = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(canvasPoint);
            var globalCorner = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(cornerPoint);

            GetScales(globalScaleCenter, globalCursor, globalCorner, out var scaleX, out var scaleY);

            var matrix = Matrix.Identity;

            matrix.ScaleAt(scaleX, scaleY, globalScaleCenter.X, globalScaleCenter.Y);

            ChangeTransform(matrix);
            Manipulate?.Invoke(this, new ManipulateEventArgs(matrix));

            SetScaleCursor(canvasPoint, _mode);
        }

        #endregion

        #region Up Actions

        private void DragUpAction()
        {
            var globalCursorPoint = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(_lastCanvasPosition);
            var globalCashPoint = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(_cashPoint);

            var offsetX = globalCursorPoint.X - globalCashPoint.X;
            var offsetY = globalCursorPoint.Y - globalCashPoint.Y;

            if (!(offsetX == 0 && offsetY == 0))
            {
                var matrix = Matrix.Identity;

                matrix.Translate(offsetX, offsetY);

                ManipulateEnded?.Invoke(this,
                    new ManipulateEventArgs(matrix));
            }
        }

        private void RotateUpAction(Point cursorPoint)
        {
            var centerPoint = _centerShape.GetPosition();
            var vector = cursorPoint - centerPoint;
            var angle = Vector.AngleBetween(vector,
                new Vector(_cashPoint.X - centerPoint.X, _cashPoint.Y - centerPoint.Y));

            if (angle != 0)
            {
                var matrix = Matrix.Identity;

                var globalCenter = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(centerPoint);

                matrix.RotateAt(angle, globalCenter.X, globalCenter.Y);

                ManipulateEnded?.Invoke(this,
                    new ManipulateEventArgs(matrix));
            }
        }

        private void ScaleUpAction(Point cursorPoint)
        {
            var globalScaleCenter = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(GetScaleCenter(_mode));
            var globalCursor = _ariadnaEngine.CoordinateSystem.GetGlobalPoint(cursorPoint);

            GetScales(globalScaleCenter, globalCursor, _cashCornerPoint, out var scaleX, out var scaleY);

            if (!(scaleX == 1 && scaleY == 1))
            {
                var matrix = Matrix.Identity;
                matrix.ScaleAt(scaleX, scaleY, globalScaleCenter.X, globalScaleCenter.Y);

                ManipulateEnded?.Invoke(this,
                    new ManipulateEventArgs(matrix));
            }
        }

        private void NonDefaultUpAction()
        {
            _mode = MouseOverMode.None;
            _ariadnaEngine.SelectHelper.OnSelectedHelper();

            SetCursor(Cursors.Arrow);
        }

        #endregion

        #region Set Cursors

        private void SetCursor(Cursor cursor)
        {
            _scaleCursor.Visibility = Visibility.Collapsed;
            _rotateCursor.Visibility = Visibility.Collapsed;

            Mouse.SetCursor(cursor);
        }

        private void SetRotateCursor(Point cursorPoint, Point centerPoint)
        {
            SetCursor(Cursors.None);
            _rotateCursor.Visibility = Visibility.Visible;

            var vector = cursorPoint - centerPoint;
            var angle = Vector.AngleBetween(vector, new Vector(1, 0));

            _rotateCursor.SetPosition(cursorPoint);
            _rotateCursor.RenderTransform = new RotateTransform(-angle);
        }

        private void SetScaleCursor(Point cursorPoint, MouseOverMode mode)
        {
            SetCursor(Cursors.None);
            _scaleCursor.Visibility = Visibility.Visible;

            _scaleCursor.SetPosition(cursorPoint);

            var angle = Vector.AngleBetween(_leftTopShape.GetPosition() - _rightTopShape.GetPosition(),
                new Vector(1, 0));

            switch (mode)
            {
                case MouseOverMode.ScaleLeftBottom:
                case MouseOverMode.ScaleRightTop:
                    angle += 45;
                    break;

                case MouseOverMode.ScaleLeftTop:
                case MouseOverMode.ScaleRightBottom:
                    angle -= 45;
                    break;

                case MouseOverMode.ScaleBottom:
                case MouseOverMode.ScaleTop:
                    angle += 90;
                    break;
            }

            _scaleCursor.RenderTransform = new RotateTransform(-angle);
        }

        #endregion

        #region Mode Definitions

        private MouseOverMode GetMode(Point cursorPos)
        {
            if (RotateDefinition(cursorPos))
                return MouseOverMode.Rotate;

            if (ScaleRightTopDefinition(cursorPos))
                return MouseOverMode.ScaleRightTop;

            if (ScaleRightBottomDefinition(cursorPos))
                return MouseOverMode.ScaleRightBottom;

            if (ScaleLeftTopDefinition(cursorPos))
                return MouseOverMode.ScaleLeftTop;

            if (ScaleLeftBottomDefinition(cursorPos))
                return MouseOverMode.ScaleLeftBottom;

            if (ScaleTopDefinition(cursorPos))
                return MouseOverMode.ScaleTop;

            if (ScaleBottomDefinition(cursorPos))
                return MouseOverMode.ScaleBottom;

            if (ScaleRightDefinition(cursorPos))
                return MouseOverMode.ScaleRight;

            if (ScaleLeftDefinition(cursorPos))
                return MouseOverMode.ScaleLeft;

            if (DefinitionDragCondition(cursorPos))
                return MouseOverMode.Drag;

            return MouseOverMode.None;
        }

        private bool DefinitionDragCondition(Point canvasPoint)
        {
            if (!_centerVisible)
                return false;

            var center = _centerShape.GetPosition();

            var centerVector = canvasPoint - center;

            if (centerVector.Length < 14)
                return true;

            return false;
        }

        private bool RotateDefinition(Point canvasPoint)
        {
            var leftBottom = _leftBottomShape.GetPosition();
            var rightTop = _rightTopShape.GetPosition();

            var leftTop = _leftTopShape.GetPosition();
            var rightBottom = _rightBottomShape.GetPosition();

            if (RotateCornerDefinition(canvasPoint, leftBottom, leftBottom - rightBottom, rightTop - rightBottom, 90))
                return true;
            if (RotateCornerDefinition(canvasPoint, rightTop, rightTop - leftTop, leftBottom - leftTop, 90))
                return true;

            if (RotateCornerDefinition(canvasPoint, leftTop, rightTop - leftTop, leftBottom - leftTop))
                return true;
            if (RotateCornerDefinition(canvasPoint, rightBottom, leftBottom - rightBottom, rightTop - rightBottom))
                return true;

            return false;
        }

        private static bool RotateCornerDefinition(Point canvasPoint, Point cornerPoint, Vector ortVectorX,
            Vector ortVectorY, double delta = 0)
        {
            var vector = canvasPoint - cornerPoint;

            var angle = Vector.AngleBetween(vector, ortVectorX);
            var angle2 = Vector.AngleBetween(vector, ortVectorY);

            angle += delta;
            angle2 += delta;

            if (angle < 0)
                angle += 360;

            if (angle2 < 0)
                angle2 += 360;

            if (angle > 360)
                angle -= 360;

            if (angle2 > 360)
                angle2 -= 360;

            if (vector.Length < 30 && vector.Length > 15 &&
                (angle >= 45 && angle <= 225) &&
                (angle2 >= 135 && angle2 <= 315))
            {
                return true;
            }

            return false;
        }

        #region Scale

        private bool ScaleRightTopDefinition(Point canvasPoint)
        {
            var rightTop = _rightTopShape.GetPosition();

            var rightTopVector = canvasPoint - rightTop;

            if (rightTopVector.Length < 14)
                return true;

            return false;
        }

        /// <summary>
        /// Определение условия, начинать ли масштабирование при клике по <see cref="System.Windows.Controls.Canvas"/>
        /// </summary>
        /// <param name="canvasPoint"></param>
        /// <returns></returns>
        private bool ScaleLeftTopDefinition(Point canvasPoint)
        {
            var leftTop = _leftTopShape.GetPosition();
            var leftTopVector = canvasPoint - leftTop;

            if (leftTopVector.Length < 14)
                return true;

            return false;
        }

        /// <summary>
        /// Определение условия, начинать ли масштабирование при клике по <see cref="System.Windows.Controls.Canvas"/>
        /// </summary>
        /// <param name="canvasPoint"></param>
        /// <returns></returns>
        private bool ScaleLeftBottomDefinition(Point canvasPoint)
        {
            var leftBottom = _leftBottomShape.GetPosition();

            var leftBottomVector = canvasPoint - leftBottom;


            if (leftBottomVector.Length < 14)
                return true;

            return false;
        }

        /// <summary>
        /// Определение условия, начинать ли масштабирование при клике по <see cref="System.Windows.Controls.Canvas"/>
        /// </summary>
        /// <param name="canvasPoint"></param>
        /// <returns></returns>
        private bool ScaleRightBottomDefinition(Point canvasPoint)
        {
            var rightBottom = _rightBottomShape.GetPosition();

            var rightBottomVector = canvasPoint - rightBottom;

            if (rightBottomVector.Length < 14)
                return true;

            return false;
        }

        /// <summary>
        /// Определение условия, начинать ли масштабирование при клике по <see cref="System.Windows.Controls.Canvas"/>
        /// </summary>
        /// <param name="canvasPoint"></param>
        /// <returns></returns>
        private bool ScaleRightDefinition(Point canvasPoint)
        {
            var right = _rightShape.GetPosition();

            var rightVector = canvasPoint - right;

            if (rightVector.Length < 14)
                return true;

            return false;
        }

        /// <summary>
        /// Определение условия, начинать ли масштабирование при клике по <see cref="System.Windows.Controls.Canvas"/>
        /// </summary>
        /// <param name="canvasPoint"></param>
        /// <returns></returns>
        private bool ScaleLeftDefinition(Point canvasPoint)
        {
            var left = _leftShape.GetPosition();

            var leftVector = canvasPoint - left;

            if (leftVector.Length < 14)
                return true;

            return false;
        }

        /// <summary>
        /// Определение условия, начинать ли масштабирование при клике по <see cref="System.Windows.Controls.Canvas"/>
        /// </summary>
        /// <param name="canvasPoint"></param>
        /// <returns></returns>
        private bool ScaleBottomDefinition(Point canvasPoint)
        {
            var bottom = _bottomShape.GetPosition();

            var bottomVector = canvasPoint - bottom;

            if (bottomVector.Length < 14)
                return true;

            return false;
        }

        /// <summary>
        /// Определение условия, начинать ли масштабирование при клике по <see cref="System.Windows.Controls.Canvas"/>
        /// </summary>
        /// <param name="canvasPoint"></param>
        /// <returns></returns>
        private bool ScaleTopDefinition(Point canvasPoint)
        {
            var top = _topShape.GetPosition();

            var topVector = canvasPoint - top;

            if (topVector.Length < 14)
                return true;

            return false;
        }

        #endregion

        #endregion

        #region Helpers

        private static void RotateShapes(double angle, params Shape[] shapes)
        {
            for (var i = 0; i < shapes.Length; i++)
            {
                shapes[i].RenderTransform =
                    new RotateTransform(-angle);
            }
        }

        private static void GetScales(Point globalScaleCenter, Point globalCursor, Point globalCorner,
            out double scaleX, out double scaleY)
        {
            var vector = globalCursor - globalScaleCenter;
            var vector2 = globalCorner - globalScaleCenter;

            scaleX = vector.X / vector2.X;
            scaleY = vector.Y / vector2.Y;

            if (double.IsInfinity(scaleX) || double.IsNaN(scaleX))
                scaleX = 1;

            if (double.IsInfinity(scaleY) || double.IsNaN(scaleY))
                scaleY = 1;

            var coef2 = scaleX / scaleY;

            if (scaleX == 1)
                scaleX = scaleY;
            else if (scaleY == 1)
                scaleY = scaleX;


            else if (Math.Abs(coef2) > 1)
                scaleY = scaleX * Math.Sign(coef2);
            else
                scaleX = scaleY * Math.Sign(coef2);
        }

        private Point GetScaleCenter(MouseOverMode mode)
        {
            var center = mode switch
            {
                MouseOverMode.ScaleRightTop => _leftBottomShape.GetPosition(),
                MouseOverMode.ScaleLeftTop => _rightBottomShape.GetPosition(),
                MouseOverMode.ScaleLeftBottom => _rightTopShape.GetPosition(),
                MouseOverMode.ScaleRightBottom => _leftTopShape.GetPosition(),
                MouseOverMode.ScaleRight => _leftShape.GetPosition(),
                MouseOverMode.ScaleLeft => _rightShape.GetPosition(),
                MouseOverMode.ScaleBottom => _topShape.GetPosition(),
                MouseOverMode.ScaleTop => _bottomShape.GetPosition(),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };

            return center;
        }

        private Point GetCorner(MouseOverMode mode)
        {
            return mode switch
            {
                MouseOverMode.ScaleRightTop => _rightTopShape.GetPosition(),
                MouseOverMode.ScaleLeftTop => _leftTopShape.GetPosition(),
                MouseOverMode.ScaleLeftBottom => _leftBottomShape.GetPosition(),
                MouseOverMode.ScaleRightBottom => _rightBottomShape.GetPosition(),
                MouseOverMode.ScaleRight => _rightShape.GetPosition(),
                MouseOverMode.ScaleLeft => _leftShape.GetPosition(),
                MouseOverMode.ScaleBottom => _bottomShape.GetPosition(),
                MouseOverMode.ScaleTop => _topShape.GetPosition(),
                _ => new Point()
            };
        }

        #endregion

        #region Structs

        private enum MouseOverMode
        {
            None,

            Drag,

            Rotate,

            ScaleRightTop,

            ScaleLeftTop,

            ScaleLeftBottom,

            ScaleRightBottom,

            ScaleRight,

            ScaleLeft,

            ScaleBottom,

            ScaleTop
        }

        #endregion
    }
}