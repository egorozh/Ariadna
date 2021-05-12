using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AKIM.Maths;
using Ariadna.Core;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <summary>
    /// Координатная система
    /// </summary>
    internal sealed class CoordinateSystem : BaseViewModel, ICoordinateSystem
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;

        #endregion

        #region Public Properties

        /// <summary>
        /// Расстояние до центра координат по X в координатах холста
        /// </summary>
        public double DeltaX { get; private set; }

        /// <summary>
        /// Расстояние до центра координат по Y в координатах холста
        /// </summary>
        public double DeltaY { get; private set; }

        /// <summary>
        /// Угол поворота системы координат
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Нынешняя позиция курсора мыши в глобальных координатах
        /// </summary>
        public Point MouseGlobalPosition { get; private set; }

        /// <summary>
        /// Разрешение в unit/dp
        /// </summary>
        public decimal Resolution { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Происходит при изменении масштаба и сдвига начала координат
        /// </summary>
        public event EventHandler<CoordinateChangedArgs> CoordinateChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CoordinateSystem(IAriadnaEngine ariadnaEngine)
        {
            _ariadnaEngine = ariadnaEngine;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            DeltaX = _ariadnaEngine.Settings.InitDeltaX;
            DeltaY = _ariadnaEngine.Settings.InitDeltaY;
            Resolution = _ariadnaEngine.Settings.InitResolution;
            Angle = _ariadnaEngine.Settings.InitAngle;

            OnMoving();

            SubscribeSettings();
            SubscribePropertyChanged();
        }

        #region Global Converter Methods

        public Matrix GetGlobalMatrixTransform()
        {
            var matrix = Matrix.Identity;

            matrix.RotateAt(-Angle, DeltaX, DeltaY);

            matrix.Translate(-DeltaX, -DeltaY);

            matrix.Scale((double) Resolution, (double) (-1M * Resolution));

            return matrix;
        }

        public double GetGlobalLength(double canvasLength) => canvasLength * (double) Resolution;

        public Point GetGlobalPoint(Point canvasPoint)
            => GetGlobalMatrixTransform().Transform(canvasPoint);

        public Geometry GetGlobalGeometry(Geometry geometry)
        {
            if (geometry is PathGeometry pathGeometry)
                return pathGeometry.ToTransform(GetGlobalMatrixTransform());

            return Geometry.Empty;
        }

        #endregion

        #region Canvas Converter Methods

        public Matrix GetCanvasMatrixTransform()
        {
            var matrix = Matrix.Identity;

            matrix.Scale((double) (1M / Resolution), (double) (-1M / Resolution));

            matrix.Translate(DeltaX, DeltaY);

            matrix.RotateAt(Angle, DeltaX, DeltaY);

            return matrix;
        }

        public double GetCanvasLength(double localLength) => localLength / (double) Resolution;

        public Point GetCanvasPoint(Point localPoint)
            => GetCanvasMatrixTransform().Transform(localPoint);

        public PointCollection GetCanvasPoints(ICollection<Point> localPoints)
        {
            var res = new PointCollection();

            foreach (var localPoint in localPoints)
                res.Add(GetCanvasPoint(localPoint));

            return res;
        }

        public PathGeometry GetCanvasGeometry(PathGeometry pathGeometry)
            => pathGeometry.ToTransform(GetCanvasMatrixTransform());

        public Size GetCanvasSize(Size globalSize) =>
            new(GetCanvasLength(globalSize.Width), GetCanvasLength(globalSize.Height));

        #endregion

        #region Moving and Scale Methods

        /// <summary>
        /// Центрирование системы координат
        /// </summary>
        public void Centering()
        {
            if (_ariadnaEngine.Figures.Count == 0)
            {
                DeltaX = _ariadnaEngine.Canvas.ActualWidth / 2;
                DeltaY = _ariadnaEngine.Canvas.ActualHeight / 2;
                return;
            }

            var bounds = GetBounds();

            var deltaX = Math.Abs(bounds.Left - bounds.Right);
            var deltaY = Math.Abs(bounds.Bottom - bounds.Top);

            var resolutionX = (deltaX / _ariadnaEngine.Canvas.ActualWidth);
            var resolutionY = (deltaY / _ariadnaEngine.Canvas.ActualHeight);

            var res = resolutionX < resolutionY
                ? resolutionY
                : resolutionX;

            if (res == double.PositiveInfinity || res == double.NegativeInfinity)
                return;

            Resolution = GetValidResolution(res);


            DeltaX = GetCanvasLength(-bounds.Left) + (_ariadnaEngine.Canvas.ActualWidth - GetCanvasLength(deltaX)) / 2;
            DeltaY = GetCanvasLength(bounds.Top) + (_ariadnaEngine.Canvas.ActualHeight - GetCanvasLength(deltaY)) / 2;
        }

        public void Centering(Point position)
        {
            var center = new Point(_ariadnaEngine.Canvas.ActualWidth / 2, _ariadnaEngine.Canvas.ActualHeight / 2);

            var canvasPoint = GetCanvasPoint(position);

            var deltaVector = canvasPoint - center;

            DeltaX -= deltaVector.X;
            DeltaY -= deltaVector.Y;
        }

        /// <summary>
        /// Включение режима перемещения и масштабирования
        /// </summary>
        public void OnMoving()
        {
            if (_isOn)
                return;

            _isOn = true;


            _ariadnaEngine.Canvas.MouseMove += CanvasMouseMove;
            _ariadnaEngine.Canvas.MouseUp += CanvasMouseUp;
            _ariadnaEngine.Canvas.MouseDown += CanvasMouseDown;
            _ariadnaEngine.Canvas.MouseWheel += CanvasMouseWheel;
        }

        /// <summary>
        /// Отключение режима перемещения и масштабирования
        /// </summary>
        public void OffMoving()
        {
            if (!_isOn)
                return;

            _isOn = false;

            _ariadnaEngine.Canvas.MouseMove -= CanvasMouseMove;
            _ariadnaEngine.Canvas.MouseUp -= CanvasMouseUp;
            _ariadnaEngine.Canvas.MouseDown -= CanvasMouseDown;
            _ariadnaEngine.Canvas.MouseWheel -= CanvasMouseWheel;
        }

        #endregion

        #endregion

        #region Private Methods

        #region Scalling

        /// <summary>
        /// Происходит при вращении колёсика мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var cursorPos = e.GetPosition(_ariadnaEngine.Canvas);

            MouseGlobalPosition = GetGlobalPoint(cursorPos);

            Scalling(e.Delta, cursorPos);
        }

        /// <summary>
        /// Масштабирование
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="cursorPos"></param>
        private void Scalling(int sign, Point cursorPos)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                DeltaX += sign;


                return;
            }

            if (Keyboard.IsKeyDown(Key.CapsLock))
            {
                DeltaY += sign;

                return;
            }


            if (sign < 0)
            {
                if (Resolution >= _ariadnaEngine.Settings.MaxResolution) return;

                Resolution += GetAddedDeltaResolution(Resolution);
            }
            else
            {
                if (Resolution <= _ariadnaEngine.Settings.MinResolution) return;

                Resolution -= GetSubtractDeltaResolution(Resolution);
            }

            // Сдвигаем систему координат, чтобы глобальная координата
            // курсора мыши осталась прежней после масштабирования:
            var canvasPoint = GetCanvasPoint(MouseGlobalPosition);
            DeltaX -= canvasPoint.X - cursorPos.X;
            DeltaY -= canvasPoint.Y - cursorPos.Y;
        }

        /// <summary>
        /// Получение дельты при повышении разрешения
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private static decimal GetAddedDeltaResolution(decimal resolution)
        {
            var decLog = Math.Log10((double) resolution);
            var truncate = Math.Truncate(decLog);

            if (decLog < 0 && decLog != truncate) truncate = truncate - 1;

            return (decimal) Math.Pow(10, truncate);
        }

        /// <summary>
        /// Получение дельты при понижении разрешения
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private static decimal GetSubtractDeltaResolution(decimal resolution)
        {
            var decLog = Math.Log10((double) resolution);

            var truncate = Math.Truncate(decLog);

            if (decLog < 0 && decLog != truncate) truncate = truncate - 1;

            return decLog == truncate
                ? (decimal) Math.Pow(10, truncate - 1)
                : (decimal) Math.Pow(10, truncate);
        }

        #endregion

        #region Centering

        private Bounds GetBounds()
        {
            var bounds = new Bounds();

            var generalBorders = new Bounds(double.MaxValue, double.MinValue, double.MinValue, double.MaxValue);
            var selectedBorders = new Bounds(double.MaxValue, double.MinValue, double.MinValue, double.MaxValue);
            var isSelectedFigures = false;

            for (var i = 0; i < _ariadnaEngine.Figures.Count; i++)
            {
                var figure = _ariadnaEngine.Figures[i];

                if (!figure.IsShow) continue;

                var borders = figure.GetBorders();

                borders.OverrideBorders(ref generalBorders);

                if (figure is SelectedFigure2D selectedFigure && selectedFigure.IsSelected)
                {
                    isSelectedFigures = true;

                    borders.OverrideBorders(ref selectedBorders);
                }
            }

            if (isSelectedFigures)
            {
                bounds.Left = selectedBorders.Left;
                bounds.Right = selectedBorders.Right;
                bounds.Bottom = selectedBorders.Bottom;
                bounds.Top = selectedBorders.Top;
            }
            else
            {
                bounds.Left = generalBorders.Left;
                bounds.Right = generalBorders.Right;
                bounds.Bottom = generalBorders.Bottom;
                bounds.Top = generalBorders.Top;
            }

            return bounds;
        }

        /// <summary>
        /// Получение валидного разрешения
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private decimal GetValidResolution(double res)
        {
            if ((decimal) res <= _ariadnaEngine.Settings.MinResolution)
                return _ariadnaEngine.Settings.MinResolution;

            if ((decimal) res >= _ariadnaEngine.Settings.MaxResolution)
                return _ariadnaEngine.Settings.MaxResolution;

            var temp = Math.Floor(Math.Log10(res));
            var mult = Math.Pow(10, temp);

            return (int) Math.Ceiling(res / mult) * (decimal) mult;
        }

        #endregion

        #region MovingCanvas

        /// <summary>
        /// Предыдущее значение позиции указателя мыши
        /// </summary>
        private Point _prevPoint;

        /// <summary>
        /// Перемещение системы координат
        /// </summary>
        private bool _isMoving;

        private bool _isOn;

        /// <summary>
        /// Происходит при щелчке любой кнопки мыши на <see cref="Canvas"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Если нажато колесико мыши:
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                // Двойной клик колесика мыши:
                if (e.ClickCount == 2)
                    Centering();

                _prevPoint = e.GetPosition(_ariadnaEngine.Canvas);

                Mouse.Capture(_ariadnaEngine.Canvas);
                _isMoving = true;
            }
        }

        /// <summary>
        /// Происходит при перемещении указателя мыши над <see cref="Canvas"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            MouseGlobalPosition = GetGlobalPoint(e.GetPosition(_ariadnaEngine.Canvas));

            if (_isMoving)
            {
                var mousePos = e.GetPosition(_ariadnaEngine.Canvas);

                DeltaX += mousePos.X - _prevPoint.X;
                DeltaY += mousePos.Y - _prevPoint.Y;

                _prevPoint = mousePos;

                Mouse.SetCursor(Cursors.Hand);
            }
        }

        /// <summary>
        /// Происходит при отпускании любой кнопки мыши над <see cref="Canvas"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.SetCursor(Cursors.Arrow);
            Mouse.Capture(null);
            _isMoving = false;
        }

        #endregion

        #region Synchronization with AriadnaEngine Settings

        private void SubscribeSettings() => _ariadnaEngine.Settings.PropertyChanged += EngineSettings_PropertyChanged;

        private void UnsubscribeSettings() => _ariadnaEngine.Settings.PropertyChanged -= EngineSettings_PropertyChanged;

        private void SubscribePropertyChanged() => PropertyChanged += CoordinateSystem_PropertyChanged;

        private void CoordinateSystem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Название измененного свойства
            var propName = e.PropertyName;

            // Если было перемещение системы координат или изменения масштаба
            if (propName == nameof(DeltaX) || propName == nameof(DeltaY) ||
                propName == nameof(Resolution) || propName == nameof(Angle))
            {
                double oldAngle = Angle;
                if (propName == nameof(Angle) && e is OldAndNewValuesPropertyChangedEventArgs args)
                    oldAngle = (double) args.OldValue;

                // Отправка события изменения координат
                CoordinateChanged?.Invoke(this, new CoordinateChangedArgs(DeltaX, DeltaY, Resolution, Angle, oldAngle));

                UnsubscribeSettings();

                _ariadnaEngine.Settings.InitDeltaX = DeltaX;
                _ariadnaEngine.Settings.InitDeltaY = DeltaY;
                _ariadnaEngine.Settings.InitResolution = Resolution;
                _ariadnaEngine.Settings.InitAngle = Angle;

                SubscribeSettings();
            }
        }

        private void EngineSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                // Если было перемещение системы координат или изменения масштаба
                case nameof(EngineSettings.InitDeltaX):
                    DeltaX = _ariadnaEngine.Settings.InitDeltaX;
                    break;
                case nameof(EngineSettings.InitDeltaY):
                    DeltaY = _ariadnaEngine.Settings.InitDeltaY;
                    break;
                case nameof(EngineSettings.InitResolution):
                    Resolution = _ariadnaEngine.Settings.InitResolution;
                    break;
                case nameof(EngineSettings.InitAngle):
                    Angle = _ariadnaEngine.Settings.InitAngle;
                    break;
            }
        }

        #endregion

        #endregion
    }
}