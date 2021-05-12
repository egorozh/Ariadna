using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Класс с методами расширений для движка
    /// </summary>
    public static class Extensions2D
    {
        /// <summary>
        /// Установка ZIndex для <see cref="UIElement"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="zOrder"></param>
        public static void SetZIndex(this UIElement element, int zOrder) => Panel.SetZIndex(element, zOrder);

        /// <summary>
        /// Установка координат для линии
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public static void SetPoints(this Line line, double x1, double y1, double x2, double y2)
        {
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
        }

        /// <summary>
        /// Установка координат для <see cref="Shape"/>
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="x">X центра <see cref="Shape"/></param>
        /// <param name="y">Y центра <see cref="Shape"/></param>
        public static void SetPosition(this FrameworkElement shape, double x, double y) =>
            shape.SetLeftAndTop(x - shape.Width / 2, y - shape.Height / 2);

        /// <summary>
        /// Установка координат для <see cref="Shape"/>
        /// </summary>
        /// <param name="shape"></param>    
        /// <param name="x">X центра <see cref="Shape"/></param>
        /// <param name="y">Y центра <see cref="Shape"/></param>
        public static void SetPosition(this FrameworkElement shape, Point center) =>
            shape.SetLeftAndTop(center.X - shape.Width / 2, center.Y - shape.Height / 2);

        public static Point GetPosition(this FrameworkElement shape)
        {
            var left = Canvas.GetLeft(shape);
            var top = Canvas.GetTop(shape);

            return new Point(left + shape.Width / 2, top + shape.Height / 2);
        }

        /// <summary>
        /// Установка позиции и размера для <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="leftTop"></param>
        /// <param name="rightBottom"></param>
        public static void SetPositionAndSize(this Rectangle rectangle, Point leftTop, Point rightBottom, Point center)
        {
            rectangle.Width = Math.Abs(leftTop.X - rightBottom.X);
            rectangle.Height = Math.Abs(leftTop.Y - rightBottom.Y);

            rectangle.SetPosition(center);
        }

        /// <summary>
        /// Установка координат для элемента
        /// </summary>
        /// <param name="element"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public static void SetLeftAndTop(this UIElement element, double left, double top)
        {
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
        }

        /// <summary>
        /// Добавление на <see cref="Canvas"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static UIElement AddToCanvas(this UIElement element, ICanvas canvas)
        {
            canvas.Children.Add(element);
            return element;
        }

        /// <summary>
        /// Изменение цвета для <see cref="Path"/>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="color"></param>
        /// <param name="isFilled"></param>
        public static void ChangeColor(this Shape path, string color, bool isFilled = false)
        {
            var converter = new BrushConverter();

            try
            {
                var stroke = (Brush) converter.ConvertFromString(color);
                path.Stroke = stroke;
                if (isFilled)
                    path.Fill = GetFillBrush(stroke);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Изменение цвета для <see cref="Path"/>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="color"></param>
        /// <param name="isFilled"></param>
        public static void ChangeColor(this Path path, Brush color, bool isFilled = false)
        {
            path.Stroke = color;
            if (isFilled)
                path.Fill = GetFillBrush(color);
        }

        /// <summary>
        /// Конвертация цвета в формате #FFFFFF в <see cref="Brush"/>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Brush ToBrush(this string color)
        {
            if (color == null) return Brushes.Blue;

            var converter = new BrushConverter();

            try
            {
                return (Brush) converter.ConvertFromString(color);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Получение цвета заливки фигуры с заданной степенью прозрачности
        /// </summary>
        /// <param name="strokeBrush"></param>
        /// <returns></returns>
        private static Brush GetFillBrush(Brush strokeBrush)
        {
            var fill = strokeBrush.Clone();
            fill.Opacity = 0.2;
            return fill;
        }

        /// <summary>
        /// Переопределение границ
        /// </summary>
        /// <param name="figureBounds"></param>
        /// <param name="overridedBounds"></param>
        public static void OverrideBorders(this Bounds figureBounds, ref Bounds overridedBounds)
        {
            if (figureBounds.Left < overridedBounds.Left)
                overridedBounds.Left = figureBounds.Left;
            if (figureBounds.Right > overridedBounds.Right)
                overridedBounds.Right = figureBounds.Right;
            if (figureBounds.Top > overridedBounds.Top)
                overridedBounds.Top = figureBounds.Top;
            if (figureBounds.Bottom < overridedBounds.Bottom)
                overridedBounds.Bottom = figureBounds.Bottom;
        }

        public static Point GetMagnetPoint(this Point pos, EngineSettings settings, IGridChart gridChart,
            IFigure2DCollection figure2DCollection, ICoordinateSystem coordinateSystem, bool isShiftMode = false,
            Point prevPoint = new Point(), int bitCount = 24)
        {
            if (isShiftMode)
            {
                var vector = pos - prevPoint;

                var theta = Math.Atan2(vector.Y, vector.X);
                var len = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

                theta = Math.Round((bitCount / 2) * theta / Math.PI) * Math.PI / (bitCount / 2);
                var p = new Point(prevPoint.X + len * Math.Cos(theta), prevPoint.Y + len * Math.Sin(theta));

                return p;
            }


            if (!settings.MagnetMode) return gridChart.SearchClosePointToGridNode(pos);

            foreach (var figure in figure2DCollection)
            {
                if (!figure.IsShow) continue;

                if (figure is IGeometryFigure pathGeometryFigure)
                {
                    var geometry = pathGeometryFigure.Geometry;

                    var approximateGeometry = geometry.GetFlattenedPathGeometry();

                    var points = GetPoints(approximateGeometry, coordinateSystem);

                    foreach (var point in points)
                    {
                        if (Math.Abs(pos.X - point.X) < 10 &&
                            Math.Abs(pos.Y - point.Y) < 10)
                            return point;
                    }
                }
            }

            return gridChart.SearchClosePointToGridNode(pos);
        }

        public static Point GetMagnetPoint(this Point pos, IAriadnaEngine ariadnaEngine, bool isShiftMode = false,
            Point prevPoint = new Point(), int bitCount = 24)
        {
            if (isShiftMode)
            {
                var vector = pos - prevPoint;

                var theta = Math.Atan2(vector.Y, vector.X);
                var len = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

                theta = Math.Round((bitCount / 2) * theta / Math.PI) * Math.PI / (bitCount / 2);
                var p = new Point(prevPoint.X + len * Math.Cos(theta), prevPoint.Y + len * Math.Sin(theta));

                return p;
            }


            if (!ariadnaEngine.Settings.MagnetMode) return ariadnaEngine.GridChart.SearchClosePointToGridNode(pos);

            foreach (var figure in ariadnaEngine.Figures)
            {
                if (!figure.IsShow) continue;

                if (figure is IGeometryFigure pathGeometryFigure)
                {
                    var geometry = pathGeometryFigure.Geometry;

                    var approximateGeometry = geometry.GetFlattenedPathGeometry();

                    var points = GetPoints(approximateGeometry, ariadnaEngine.CoordinateSystem);

                    foreach (var point in points)
                    {
                        if (Math.Abs(pos.X - point.X) < 10 &&
                            Math.Abs(pos.Y - point.Y) < 10)
                            return point;
                    }
                }
            }

            return ariadnaEngine.GridChart.SearchClosePointToGridNode(pos);
        }


        /// <summary>
        /// Получение точек геометрии в координатах <see cref="Canvas"/>
        /// </summary>
        /// <param name="approximateGeometry"></param>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        private static List<Point> GetPoints(PathGeometry approximateGeometry, ICoordinateSystem coordinateSystem)
        {
            var points = new List<Point>();

            if (approximateGeometry.Figures == null || approximateGeometry.Figures.Count < 1) return points;

            points.Add(coordinateSystem.GetCanvasPoint(approximateGeometry.Figures.First().StartPoint));

            var segments = approximateGeometry.Figures.First().Segments;

            for (var i = 0; i < segments.Count; i++)
            {
                switch (segments[i])
                {
                    case LineSegment lineSegment:
                        points.Add(coordinateSystem.GetCanvasPoint(lineSegment.Point));
                        break;
                    case PolyLineSegment polyLineSegment:
                        points.AddRange(coordinateSystem.GetCanvasPoints(polyLineSegment.Points));
                        break;
                }
            }

            return points;
        }
    }
}