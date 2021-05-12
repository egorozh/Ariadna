using System.Windows;
using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    public static class DxfHelpers
    {
        public static Geometry GetData(this DxfLine line) => GetData(line.StartPoint, line.EndPoint);

        #region Private Methods

        private static Geometry GetData(Point startPoint, Point endPoint)
        {
            var lineSegment = new LineSegment(endPoint, true);

            var pathFigure = new PathFigure
            {
                StartPoint = startPoint,
                IsClosed = false,
                Segments = new PathSegmentCollection {lineSegment}
            };

            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            return pathGeometry;
        }

        #endregion
    }
}