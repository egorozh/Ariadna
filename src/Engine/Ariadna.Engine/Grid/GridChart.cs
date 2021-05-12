using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    /// <summary>
    /// Координатная сетка
    /// </summary>
    internal sealed class GridChart : IGridChart
    {
        #region PrivateFields

        private readonly IAriadnaEngine _ariadnaEngine;

        /// <summary>
        /// Примитив оси абсцисс
        /// </summary>
        private readonly Line _ortX = ShapeFactory.CreateOrtLine();

        /// <summary>
        /// Примитив оси ординат
        /// </summary>
        private readonly Line _ortY = ShapeFactory.CreateOrtLine();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public GridChart(IAriadnaEngine ariadnaEngine)
        {
            _ariadnaEngine = ariadnaEngine;
        }

        #endregion

        public void Init()
        {
            _ariadnaEngine.CoordinateSystem.CoordinateChanged += CoordinateSystemChanged;

            // Отображение сетки в зависимости от настроек:
            _ariadnaEngine.Settings.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_ariadnaEngine.Settings.ShowGrid))
                    ChangeGridState(_ariadnaEngine.Settings);

                if (e.PropertyName == nameof(_ariadnaEngine.Settings.CountLittleRects))
                    Update(_ariadnaEngine.GridCanvas, _ariadnaEngine.CoordinateSystem.DeltaX, _ariadnaEngine.CoordinateSystem.DeltaY,
                        _ariadnaEngine.CoordinateSystem.Resolution, _ariadnaEngine.CoordinateSystem.Angle);
            };

            ChangeGridState(_ariadnaEngine.Settings);

            EngineGlobalSettings.Instance.PropertyChanged += Instance_PropertyChanged;

            UpdateBackgroundColor();
        }

        #region Implemented Methods

        /// <summary>
        /// Поиск <see cref="Point"/>, ближайшей к какому-то узлу сетки
        /// </summary>
        /// <param name="canvasPoint">Позиция указателя мыши</param>
        /// <returns></returns>
        public Point SearchClosePointToGridNode(Point canvasPoint)
        {
            if (!_ariadnaEngine.Settings.MagnetGridMode) return canvasPoint;

            var xCoef = Math.Round((canvasPoint.X - _ariadnaEngine.CoordinateSystem.DeltaX) /
                                   (_ariadnaEngine.DrawingControl.GridCanvas.DpiInOneSquare /
                                    _ariadnaEngine.Settings.CountLittleRects));
            var yCoef = Math.Round((canvasPoint.Y - _ariadnaEngine.CoordinateSystem.DeltaY) /
                                   (_ariadnaEngine.DrawingControl.GridCanvas.DpiInOneSquare /
                                    _ariadnaEngine.Settings.CountLittleRects));

            var x = _ariadnaEngine.CoordinateSystem.DeltaX +
                    xCoef * _ariadnaEngine.DrawingControl.GridCanvas.DpiInOneSquare / _ariadnaEngine.Settings.CountLittleRects;
            var y = _ariadnaEngine.CoordinateSystem.DeltaY +
                    yCoef * _ariadnaEngine.DrawingControl.GridCanvas.DpiInOneSquare / _ariadnaEngine.Settings.CountLittleRects;

            if (double.IsNaN(x) || double.IsNaN(y))
                return canvasPoint;

            return new Point(x, y);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Показ или скрытие сетки в зависимости от настроек
        /// </summary>
        /// <param name="settings">Настройки движка</param>
        private void ChangeGridState(EngineSettings settings)
        {
            if (settings.ShowGrid)
                Show();
            else
                Hide();
        }

        /// <summary>
        /// Показать сетку
        /// </summary>
        private void Show()
        {
            _ariadnaEngine.DrawingControl.GridCanvas.ShowGrid();

            Update(_ariadnaEngine.GridCanvas, _ariadnaEngine.CoordinateSystem.DeltaX, _ariadnaEngine.CoordinateSystem.DeltaY,
                _ariadnaEngine.CoordinateSystem.Resolution, _ariadnaEngine.CoordinateSystem.Angle);

            _ariadnaEngine.Canvas.SizeChanged += CanvasSizeChanged;
            _ariadnaEngine.Canvas.AddElements(_ortX, _ortY);

            _ariadnaEngine.Settings.ShowGrid = true;

            UpdateBackgroundColor();
        }

        /// <summary>
        /// Скрыть сетку
        /// </summary>
        private void Hide()
        {
            _ariadnaEngine.DrawingControl.GridCanvas.HideGrid();

            _ariadnaEngine.Canvas.SizeChanged -= CanvasSizeChanged;
            _ariadnaEngine.Canvas.RemoveElements(_ortX, _ortY);

            _ariadnaEngine.Settings.ShowGrid = false;
        }

        /// <summary>
        /// Происходит при изменении масштаба и перемещении системы координат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordinateSystemChanged(object sender, CoordinateChangedArgs e)
        {
            Update(_ariadnaEngine.GridCanvas, e.DeltaX, e.DeltaY, e.Resolution, e.NewAngle);
        }

        /// <summary>
        /// Обновление сетки
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        /// <param name="resolution"></param>
        /// <param name="angle"></param>
        private void Update(IGridCanvas canvas, double deltaX, double deltaY, decimal resolution, double angle)
        {
            _ariadnaEngine.DrawingControl.GridCanvas.UpdateGridBrush(deltaX, deltaY, _ariadnaEngine.Settings, resolution,
                _ariadnaEngine.CoordinateSystem, angle);

            //TODO Заменить на нормальный расчет точек линий
            _ortX.SetPoints(-10000, deltaY, 10000, deltaY);
            _ortY.SetPoints(deltaX, -10000, deltaX, 10000);

            _ortX.RenderTransform = new RotateTransform(angle, deltaX, deltaY);
            _ortY.RenderTransform = new RotateTransform(angle, deltaX, deltaY);

            UpdateBackgroundColor();
        }

        /// <summary>
        /// Происходит при изменении размеров <see cref="Canvas"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _ortX.X2 = e.NewSize.Width;
            _ortY.Y2 = e.NewSize.Height;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateBackgroundColor();
        }

        private void UpdateBackgroundColor()
        {
            var majorGridLineColor = Brushes.LightGray;

            if (ColorConverter.ConvertFromString(EngineGlobalSettings.Instance.GridMajorColor) is Color color1)
                majorGridLineColor = new SolidColorBrush(color1);

            var minorGridLineColor = Brushes.DarkGray;

            if (ColorConverter.ConvertFromString(EngineGlobalSettings.Instance.GridMinorColor) is Color color2)
                minorGridLineColor = new SolidColorBrush(color2);

            _ariadnaEngine.DrawingControl.GridCanvas.UpdateMajorPen(majorGridLineColor);
            _ariadnaEngine.DrawingControl.GridCanvas.UpdateMinorPen(minorGridLineColor);

            var ortLineColor = Brushes.Blue;

            if (ColorConverter.ConvertFromString(EngineGlobalSettings.Instance.OrtLineColor) is Color color3)
                ortLineColor = new SolidColorBrush(color3);

            _ortX.Stroke = ortLineColor;
            _ortY.Stroke = ortLineColor;

            if (_ariadnaEngine.Settings.ShowGrid)
                _ariadnaEngine.DrawingControl.GridCanvas.ShowGrid();
            else
                _ariadnaEngine.DrawingControl.GridCanvas.HideGrid();
        }

        #endregion
    }
}