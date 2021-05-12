using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal static class Extensions
    {
        public static string ToStr(this double length)
        {
            return length != 0 ? length.ToString("##.### 'м'") : "";
        }

        public static string ToAreaStr(this double length)
        {
            return length != 0 ? length.ToString("##.### 'м²'") : "";
        }   
        
        public static void SetTextBlockOnCenter(this TextBlock textBlock, Point point1, Point point2,
            ICoordinateSystem coordinateSystem)
        {
            var vector = point2 - point1;
            vector.Normalize();

            var normalVector = new Vector(vector.Y, vector.X);

            var angle = GetAngle(point1, point2);

            var centerPoint = GetCenterPoint(point1, point2);

            var actualWidth = textBlock.ActualWidth == 0 ? 47 : textBlock.ActualWidth;
            var actualHeight = textBlock.ActualHeight == 0 ? 16 : textBlock.ActualHeight;

            var canvasCenter = coordinateSystem.GetCanvasPoint(centerPoint);

            canvasCenter = point1.X > point2.X ? canvasCenter + normalVector * 15 : canvasCenter - normalVector * 15;

            var leftTopPoint = new Point(canvasCenter.X - actualWidth / 2, canvasCenter.Y - actualHeight / 2);

            var angle2 = point1.X > point2.X ? angle - 180 : angle;

            textBlock.SetAngleAndPosition(leftTopPoint, angle2);
        }

        public static void SetTextBlockOnCenter(this TextBlock textBlock, Point point1, Point centerPoint, Point point2,
            ICoordinateSystem coordinateSystem)
        {
            var vector = point2 - point1;
            vector.Normalize();

            var normalVector = new Vector(vector.Y, vector.X);

            var actualWidth = textBlock.ActualWidth == 0 ? 47 : textBlock.ActualWidth;
            var actualHeight = textBlock.ActualHeight == 0 ? 16 : textBlock.ActualHeight;

            var canvasCenter = coordinateSystem.GetCanvasPoint(centerPoint);

            canvasCenter = point1.X > point2.X ? canvasCenter + normalVector * 15 : canvasCenter - normalVector * 15;

            var leftTopPoint = new Point(canvasCenter.X - actualWidth / 2, canvasCenter.Y - actualHeight / 2);

            var angle = GetAngle(point1, point2);
            var angle2 = point1.X > point2.X ? angle - 180 : angle;

            textBlock.SetAngleAndPosition(leftTopPoint, angle2);
        }

        private static void SetAngleAndPosition(this UIElement element, Point leftTopPosition, double angle)
        {
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            element.SetLeftAndTop(leftTopPosition.X, leftTopPosition.Y);
            element.RenderTransform = new RotateTransform(angle);
        }

        private static Point GetCenterPoint(Point point1, Point point2) =>
            new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);

        private static double GetAngle(Point point1, Point point2)
        {
            var vector = point2 - point1;

            var cosAngle = (point2.X - point1.X) / vector.Length;
            var angle = Math.Acos(cosAngle) * 180 / Math.PI;

            return point1.Y < point2.Y ? -angle : angle;
        }
    }
}