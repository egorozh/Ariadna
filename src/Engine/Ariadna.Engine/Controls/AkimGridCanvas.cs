using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace Ariadna.Engine
{
    internal sealed class AkimGridCanvas : Canvas, IGridCanvas
    {
        #region Private Fields

        private readonly IAriadnaEngine _ariadnaEngine;

        /// <summary>
        /// Кисть с сеткой
        /// </summary>
        private DrawingBrush _gridBrush;

        /// <summary>
        /// Кисть для холста без сетки
        /// </summary>
        private readonly SolidColorBrush _emptyBrush = Brushes.Transparent;

        #endregion

        #region Public Properties

        /// <summary>
        /// Число DPI в одном большом квадрате сетки
        /// </summary>
        public double DpiInOneSquare { get; private set; }

        #endregion

        #region Constructor

        public AkimGridCanvas(IAriadnaEngine ariadnaEngine)
        {
            _ariadnaEngine = ariadnaEngine;
            Background = Brushes.Transparent;
            ClipToBounds = true;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            _gridBrush = CreateGridBrush(_ariadnaEngine.Settings, 0, 0);
        }

        public void ShowGrid()
        {
            Background = _gridBrush;
        }

        public void HideGrid()
        {
            Background = _emptyBrush;
        }

        public void UpdateGridBrush(double deltaX, double deltaY, EngineSettings engineSettings, decimal resolution,
            ICoordinateSystem coordinateSystem, double angle)
        {
            SetDpiInOneGridSquare(resolution, coordinateSystem);

            _gridBrush.Drawing = CreateGeometryDrawing(engineSettings);
            _gridBrush.Viewport = CreateViewport(deltaX, deltaY);

            _gridBrush.Transform = new RotateTransform(angle, deltaX, deltaY);
        }

        public void UpdateMajorPen(SolidColorBrush majorGridLineColor)
        {
            ((_gridBrush.Drawing as DrawingGroup).Children[0] as GeometryDrawing).Pen =
                new Pen(majorGridLineColor, 0.3);
        }

        public void UpdateMinorPen(SolidColorBrush minorGridLineColor)
        {
            ((_gridBrush.Drawing as DrawingGroup).Children[1] as GeometryDrawing).Pen =
                new Pen(minorGridLineColor, 0.2);
        }

        public void AddElements(params UIElement[] elements)
        {
            foreach (var uiElement in elements)
                Children.Add(uiElement);
        }

        public void RemoveElements(params UIElement[] elements)
        {
            foreach (var element in elements)
                Children.Remove(element);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Создание координатной сетки
        /// </summary>
        /// <returns></returns>
        private DrawingBrush CreateGridBrush(EngineSettings engineSettings, double deltaX, double deltaY) =>
            new DrawingBrush
            {
                TileMode = TileMode.Tile,
                Viewport = CreateViewport(deltaX, deltaY),
                ViewportUnits = BrushMappingMode.Absolute,
                Drawing = CreateGeometryDrawing(engineSettings),
            };

        /// <summary>
        /// Создание геометрии внутри квадрата сетки
        /// </summary>
        /// <returns></returns>
        private DrawingGroup CreateGeometryDrawing(EngineSettings engineSettings)
        {
            var drawingGroup = new DrawingGroup();

            var majorGridLineColor = Brushes.LightGray;

            if (ColorConverter.ConvertFromString(EngineGlobalSettings.Instance.GridMajorColor) is Color color)
                majorGridLineColor = new SolidColorBrush(color);

            var minorGridLineColor = Brushes.DarkGray;

            if (ColorConverter.ConvertFromString(EngineGlobalSettings.Instance.GridMinorColor) is Color color2)
                minorGridLineColor = new SolidColorBrush(color2);

            // Добавляем примитив наружнего квадрата сетки
            drawingGroup.Children.Add(new GeometryDrawing
            {
                Geometry = new RectangleGeometry(new Rect(new Point(0, 0),
                    new Point(DpiInOneSquare, DpiInOneSquare))),
                Pen = new Pen(majorGridLineColor, 0.3)
            });


            // Рисуем внутренние квадраты:
            // В одном большом квадрате countLittleRects^2 маленьких квадратов
            var internalLines = new GeometryGroup();
            var delta = DpiInOneSquare / engineSettings.CountLittleRects;
            for (var i = 1; i < engineSettings.CountLittleRects; i++)
            {
                // Вертикальные линии:
                internalLines.Children.Add(new LineGeometry(new Point(i * delta, 0),
                    new Point(i * delta, DpiInOneSquare)));

                // Горизонтальные линии:
                internalLines.Children.Add(new LineGeometry(new Point(0, i * delta),
                    new Point(DpiInOneSquare, i * delta)));
            }

            // Добавляем внутренние линии, образующие маленькие квадраты сетки
            drawingGroup.Children.Add(new GeometryDrawing
                {Pen = new Pen(minorGridLineColor, 0.2), Geometry = internalLines});

            return drawingGroup;
        }

        /// <summary>
        /// Создание области квадрата сетки
        /// </summary>
        /// <returns></returns>
        private Rect CreateViewport(double deltaX, double deltaY) =>
            new Rect(new Point(deltaX, deltaY), new Point(deltaX + DpiInOneSquare, deltaY + DpiInOneSquare));

        /// <summary>
        /// Определение количества dp в одном большом квадрате сетки
        /// </summary>
        private void SetDpiInOneGridSquare(decimal resolution, ICoordinateSystem coordinateSystem)
        {
            var decLog = Math.Log10((double) resolution);
            var truncate = Math.Truncate(decLog);

            if (decLog < 0 && decLog != truncate) truncate = truncate - 1;

            //Число юнитов в одном квадрате сетки
            var unitsInOneSquare = (long) (Math.Pow(10, truncate) * 500d);

            DpiInOneSquare = coordinateSystem.GetCanvasLength(unitsInOneSquare);
        }

        #endregion
    }
}