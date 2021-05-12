using System.Windows;
using System.Windows.Media;

namespace Ariadna.Engine.Transformation
{
    internal static class Extensions
    {
        public static Point Rotate(this Point point, double angle, Point center)
        {
            RotateTransform transform = new (angle, center.X, center.Y);

            return transform.Transform(point);
        }

        public static Point Translate(this Point point, double offsetX, double offsetY)
        {
            TranslateTransform transform = new (offsetX, offsetY);

            return transform.Transform(point);
        }

        public static Point Scale(this Point point, Point scale, Point center)
        {
            ScaleTransform transform = new (scale.X, scale.Y, center.X, center.Y);

            return transform.Transform(point);
        }

        public static Vector ToVector(this Point point) => new (point.X, point.Y);
        public static Point ToPoint(this Vector point) => new (point.X, point.Y);
    }
}