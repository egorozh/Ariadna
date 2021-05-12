using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using AKIM.Maths;

namespace Ariadna.Engine.Core
{
    public static class GeometryMath
    {
        /// <summary>
        /// Поиск радиуса по трём точкам
        /// </summary>
        /// <param name="middlePoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="startPoint"></param>
        /// <returns></returns>
        public static double GetRadius(Point middlePoint, Point endPoint, Point startPoint)
        {
            var d = 2 * (startPoint.X - endPoint.X) * (endPoint.Y - middlePoint.Y) +
                    2 * (middlePoint.X - endPoint.X) * (startPoint.Y - endPoint.Y);
            var m1 = (Math.Pow(startPoint.X, 2) - Math.Pow(endPoint.X, 2) + Math.Pow(startPoint.Y, 2) -
                      Math.Pow(endPoint.Y, 2));
            var m2 = (Math.Pow(endPoint.X, 2) - Math.Pow(middlePoint.X, 2) + Math.Pow(endPoint.Y, 2) -
                      Math.Pow(middlePoint.Y, 2));
            var nx = m1 * (endPoint.Y - middlePoint.Y) + m2 * (endPoint.Y - startPoint.Y);
            var ny = m1 * (middlePoint.X - endPoint.X) + m2 * (startPoint.X - endPoint.X);
            var centerX = nx / d;
            var centerY = ny / d;
            var dx = centerX - startPoint.X;
            var dy = centerY - startPoint.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);

            return distance;
        }

        /// <summary>
        /// Поиск средней точки на дуге
        /// </summary>
        /// <param name="arcSegment"></param>
        /// <param name="startPoint"></param>
        /// <returns></returns>
        public static Point GetMiddlePoint(ArcSegment arcSegment, Point startPoint)
        {
            var endPoint = arcSegment.Point;
            var radius = arcSegment.Size.Width;
            var isLarge = arcSegment.IsLargeArc;
            var sweepDirection = arcSegment.SweepDirection;


            var center = CalcCenterArc(startPoint, endPoint, radius,
                sweepDirection == SweepDirection.Clockwise ? 1 : -1,
                isLarge);

            var startCenterVector = startPoint - center;
            var endCenterVector = endPoint - center;

            var angle = Vector.AngleBetween(startCenterVector, endCenterVector) / 2.0;

            if (Math.Abs(angle) == 90)
            {
                var angle2 = angle;

                var x = center.X + (startPoint.X - center.X) * Math.Cos(angle2 * Math.PI / 180.0) -
                        (startPoint.Y - center.Y) * Math.Sin(angle2 * Math.PI / 180.0);
                var y = center.Y + (startPoint.Y - center.Y) * Math.Cos(angle2 * Math.PI / 180.0) +
                        (startPoint.X - center.X) * Math.Sin(angle2 * Math.PI / 180.0);

                var point1 = new Point(x, y);

                angle2 = -angle;

                x = center.X + (startPoint.X - center.X) * Math.Cos(angle2 * Math.PI / 180.0) -
                    (startPoint.Y - center.Y) * Math.Sin(angle2 * Math.PI / 180.0);
                y = center.Y + (startPoint.Y - center.Y) * Math.Cos(angle2 * Math.PI / 180.0) +
                    (startPoint.X - center.X) * Math.Sin(angle2 * Math.PI / 180.0);

                var point2 = new Point(x, y);

                var figure = new PathFigure(startPoint, new[] {arcSegment}, true);
                var geometry = new PathGeometry(new[] {figure});

                //if (geometry.FillContains(point1))
                //    return point1;
                //if (geometry.FillContains(point2))
                //    return point2;
                if (point1.Y > point2.Y && sweepDirection == SweepDirection.Clockwise)
                    return point2;
                else if (point2.Y <= point1.Y && sweepDirection == SweepDirection.Counterclockwise)
                    return point1;
                return new Point(0, 0);
            }
            else
            {
                if (isLarge)
                    angle = 180.0 + angle;

                var x = center.X + (startPoint.X - center.X) * Math.Cos(angle * Math.PI / 180.0) -
                        (startPoint.Y - center.Y) * Math.Sin(angle * Math.PI / 180.0);
                var y = center.Y + (startPoint.Y - center.Y) * Math.Cos(angle * Math.PI / 180.0) +
                        (startPoint.X - center.X) * Math.Sin(angle * Math.PI / 180.0);

                return new Point(x, y);
            }
        }

        /// <summary>
        /// Получение сегмента-дуги по трём точкам
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="middlePoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static ArcSegment GetArcSegment(Point startPoint, Point middlePoint, Point endPoint)
        {
            var radius = GetRadius(middlePoint, endPoint, startPoint);
            var ac = new Vector(middlePoint.X - startPoint.X, middlePoint.Y - startPoint.Y);
            var ab = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
            var angle = Vector.AngleBetween(ac, ab);

            // Определяем условие, больше ли 180 градусов угол дуги
            var bc = new Vector(middlePoint.X - endPoint.X, middlePoint.Y - endPoint.Y);
            var acb = Vector.AngleBetween(ac, bc);
            var isLargeArc = Math.Abs(acb) < 90;

            return new ArcSegment
            {
                Point = endPoint,
                Size = new Size(Math.Abs(radius), Math.Abs(radius)),
                IsLargeArc = isLargeArc,
                SweepDirection = angle > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise
            };
        }

        /// <summary>
        /// Точность при операциях сравнения на равенство
        /// </summary>
        private const double FixingEquals = 0.00001;

        /// <summary>
        /// Проверяет, лежит ли точка на отрезке
        /// </summary>
        /// <param name="point">Точка</param>
        /// <param name="a">Первая точка отрезка</param>
        /// <param name="b">Вторая точка отрезка</param>
        /// <returns></returns>
        public static bool IsPointOnLine(Point point, Point a, Point b)
        {
           
            return ((IsPointsEqual(point, a) || IsPointsEqual(point, b) || IsPointsEqual(a, b)) ||
                    (point.X > a.X == point.X < b.X &&
                     point.Y > a.Y == point.Y < b.Y &&
                     Math.Abs((point.X - b.X) * (a.Y - b.Y) - (a.X - b.X) * (point.Y - b.Y)) < FixingEquals));
        }

        /// <summary>
        /// Лежат ли три точки на одной прямой
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        public static bool IsThreePointsOnOneLine(Point point1, Point point2, Point point3)
        {
            if (IsPointOnLine(point1, point2, point3) ||
                IsPointOnLine(point2, point1, point3) ||
                IsPointOnLine(point3, point2, point1))
                return true;

            return false;
        }

        public static bool IsPointsEqual(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) < FixingEquals && Math.Abs(point1.Y - point2.Y) < FixingEquals;
        }

        /// <summary>
        /// Поиск координат центра окружности, образующей дугу
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="radius"></param>
        /// <param name="sign"></param>
        /// <param name="isLarge"></param>
        /// <returns></returns>
        private static Point CalcCenterArc(Point startPoint, Point endPoint, double radius, int sign, bool isLarge)
        {
            var a = startPoint.X;
            var b = startPoint.Y;
            var c = endPoint.X;
            var d = endPoint.Y;

            FindCircleCircleIntersections(a, b, radius, c, d, radius, out var center1, out var center2);

            var ac1 = new Vector(center1.X - startPoint.X, center1.Y - startPoint.Y);
            var ab = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
            var angle = Vector.AngleBetween(ac1, ab);

            if (isLarge)
                return sign == Math.Sign(angle) ? center1 : center2;

            return sign != Math.Sign(angle) ? center1 : center2;
        }

        /// <summary>
        /// Поиск точек пересечения двух окружностей
        /// </summary>
        /// <param name="cx0"></param>
        /// <param name="cy0"></param>
        /// <param name="radius0"></param>
        /// <param name="cx1"></param>
        /// <param name="cy1"></param>
        /// <param name="radius1"></param>
        /// <param name="intersection1"></param>
        /// <param name="intersection2"></param>
        /// <returns></returns>
        private static int FindCircleCircleIntersections(
            double cx0, double cy0, double radius0,
            double cx1, double cy1, double radius1,
            out Point intersection1, out Point intersection2)
        {
            // Find the distance between the centers.
            var dx = cx0 - cx1;
            var dy = cy0 - cy1;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            // See how many solutions there are.
            if (dist - 0.0000001 > radius0 + radius1)
            {
                // No solutions, the circles are too far apart.
                intersection1 = new Point(double.NaN, double.NaN);
                intersection2 = new Point(double.NaN, double.NaN);
                return 0;
            }

            if (dist < Math.Abs(radius0 - radius1))
            {
                // No solutions, one circle contains the other.
                intersection1 = new Point(double.NaN, double.NaN);
                intersection2 = new Point(double.NaN, double.NaN);
                return 0;
            }

            if ((dist == 0) && (radius0 == radius1))
            {
                // No solutions, the circles coincide.
                intersection1 = new Point(double.NaN, double.NaN);
                intersection2 = new Point(double.NaN, double.NaN);
                return 0;
            }

            // Find a and h.
            var a = (radius0 * radius0 -
                radius1 * radius1 + dist * dist) / (2 * dist);
            var h = Math.Sqrt(radius0 * radius0 - a * a);

            if (double.IsNaN(h))
            {
                h = 0;
            }

            // Find P2.
            var cx2 = cx0 + a * (cx1 - cx0) / dist;
            var cy2 = cy0 + a * (cy1 - cy0) / dist;

            // Get the points P3.
            intersection1 = new Point(
                cx2 + h * (cy1 - cy0) / dist,
                cy2 - h * (cx1 - cx0) / dist);
            intersection2 = new Point(
                cx2 - h * (cy1 - cy0) / dist,
                cy2 + h * (cx1 - cx0) / dist);

            // See if we have 1 or 2 solutions.
            if (dist == radius0 + radius1) return 1;

            return 2;
        }

        /// <summary>
        /// Определение начальной геометрии, получаемой из выделенных перед созданием фигур
        /// </summary>
        /// <param name="selectedFigures">Коллекция выделенных фигур</param>
        /// <returns></returns>
        public static PathGeometry GetInitialGeometry(IList<ISelectedFigure2D> selectedFigures)
        {
            // Геометрии выделенных фигур
            var geometries = new List<PathGeometry>();
            //foreach (var selectedFigure in selectedFigures)
            //    geometries.Add((PathGeometry) selectedFigure.GetGlobalGeometry());

            // Берём первую геометрию из списка
            var geometry = geometries.First();
            var startPoint = geometry.GetStartPoint();
            var endPoint = geometry.GetEndPoint();

            // Сегменты начальной геометрии
            var segments = new PathSegmentCollection();

            // Добавляем сегменты 
            segments.AddRange(geometry.GetSegments());

            // Удаляем геометрию из списка
            geometries.Remove(geometry);

            // Находим следующую геометрию
            geometry = GetNextGeometry(geometries, startPoint, endPoint, out var flippedFirstSegments);

            // Если следующая геометрия имеет общую точку со стартовой точкой первой геометрии
            if (flippedFirstSegments)
            {
                // Переворачиваем сегменты первой геометрии
                segments = segments.GetFlippedSegments();
            }

            // Пока находится геометрия с совпадающими точками
            while (geometry != null)
            {
                segments.AddRange(geometry.GetSegments());
                startPoint = geometry.GetStartPoint();
                endPoint = geometry.GetEndPoint();
                geometries.Remove(geometry);

                geometry = GetNextGeometry(geometries, endPoint);
            }

            var figure = new PathFigure(flippedFirstSegments
                ? endPoint
                : startPoint, segments, true);
            var selectedGeometry = new PathGeometry(new[] {figure});
            return selectedGeometry;
        }

        /// <summary>
        /// Поиск следующей присоединенной геометрии
        /// </summary>
        /// <param name="geometries"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="flippedFirstSegments"></param>
        /// <returns></returns>
        private static PathGeometry GetNextGeometry(List<PathGeometry> geometries, Point startPoint, Point endPoint,
            out bool flippedFirstSegments)
        {
            flippedFirstSegments = false;

            foreach (var pathGeometry in geometries)
            {
                if (pathGeometry.GetStartPoint() == (endPoint))
                    return pathGeometry;
                if (pathGeometry.GetEndPoint() == (endPoint))
                    return pathGeometry.FlipSegments();

                if (pathGeometry.GetStartPoint() == (startPoint))
                {
                    flippedFirstSegments = true;
                    return pathGeometry;
                }

                if (pathGeometry.GetEndPoint() == (startPoint))
                {
                    flippedFirstSegments = true;
                    return pathGeometry.FlipSegments();
                }
            }

            return null;
        }

        /// <summary>
        /// Поиск следующей присоединенной геометрии
        /// </summary>
        /// <param name="geometries"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private static PathGeometry GetNextGeometry(List<PathGeometry> geometries, Point endPoint)
        {
            foreach (var pathGeometry in geometries)
            {
                if (pathGeometry.GetStartPoint() == endPoint)
                    return pathGeometry;
                if (pathGeometry.GetEndPoint() == endPoint)
                    return pathGeometry.FlipSegments();
            }

            return null;
        }
    }
}