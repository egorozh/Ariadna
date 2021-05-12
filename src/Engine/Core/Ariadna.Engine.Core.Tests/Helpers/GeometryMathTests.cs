using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.Windows.Media;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.Core.Tests
{
    [TestClass()]
    public class GeometryMathTests
    {
        [TestMethod]
        public void GetMiddlePointTest()
        {
            var random = new Random();

            double RandDouble() => random.NextDouble() * random.Next(0, 100000);

            var startPoint = new Point(RandDouble(), RandDouble());

            var radius = RandDouble();

            var size = new Size(radius, radius);

            var middlePoint = startPoint + new Vector(radius, radius);
            var endPoint = startPoint + new Vector(2 * radius, 0);

            var arcSegment = new ArcSegment(endPoint, size, 0, false, SweepDirection.Counterclockwise, true);

            var actual = GeometryMath.GetMiddlePoint(arcSegment, startPoint); 
            actual = new Point(Math.Round(actual.X, 6), Math.Round(actual.Y, 2));
            middlePoint = new Point(Math.Round(middlePoint.X, 6), Math.Round(actual.Y, 2));
            var message = $"ArcSegment: Point: {arcSegment.Point} \r\nRadius: {radius}";
            message += $"\r\nStartPoint: {startPoint}";
            message += $"\r\nEndPoint: {endPoint}";
            Assert.AreEqual(middlePoint, actual, message);
        }

        [TestMethod]
        public void GetRadiusTest()
        {
            var random = new Random();

            double RandDouble() => random.NextDouble() * random.Next(0, 100000);

            var expected = RandDouble();

            var startPoint = new Point(RandDouble(), RandDouble());

            var middlePoint = startPoint + new Vector(expected, expected);

            var endPoint = startPoint + new Vector(2 * expected, 0);

            var actual = GeometryMath.GetRadius(middlePoint, endPoint, startPoint);

            Assert.AreEqual(expected, actual, 0.000001);
        }

        [TestMethod()]
        public void GetArcSegmentTest()
        {
            var random = new Random();

            double RandDouble() => random.NextDouble() * random.Next(0, 100000);

            var startPoint = new Point(RandDouble(), RandDouble());

            var radius = RandDouble();
            var size = new Size(radius, radius);

            var middlePoint = startPoint + new Vector(radius, radius);
            var endPoint = startPoint + new Vector(2 * radius, 0);

            var expected = new ArcSegment(endPoint, size, 0, false, SweepDirection.Counterclockwise, true);

            var actual = GeometryMath.GetArcSegment(startPoint, middlePoint, endPoint);


            Assert.AreEqual(expected.Size.Height, actual.Size.Height, 0.000001);
        }

        [TestMethod()]
        public void IsPointOnLineTest()
        {
            var random = new Random();

            double RandDouble() => random.NextDouble() * random.Next(-100000, 100000);

            var startPoint = new Point(RandDouble(), RandDouble());

            var vector = new Vector(RandDouble(), RandDouble());
            vector.Normalize();

            var length = RandDouble();

            var pointOnLine = startPoint + vector * length;

            var endPoint = pointOnLine + vector * length;

            var actual = GeometryMath.IsPointOnLine(pointOnLine, startPoint, endPoint);

            Assert.IsTrue(actual, $"{startPoint}   {pointOnLine}  {endPoint}");
        }

        [TestMethod()]
        public void IsThreePointsOnOneLineTest()
        {
            var random = new Random();

            double RandDouble() => random.NextDouble() * random.Next(-100000, 100000);

            var vector = new Vector(RandDouble(), RandDouble());
            vector.Normalize();

            var point1 = new Point(RandDouble(), RandDouble());
            var point2 = point1 + vector * RandDouble();
            var point3 = point1 + vector * RandDouble();
            var err = Math.Abs((point2.X - point1.X) * (point3.Y - point1.Y) -
                               (point3.X - point1.X) * (point2.Y - point1.Y));
            var actual = GeometryMath.IsThreePointsOnOneLine(point1, point2, point3);

            var message = $"Point1: {point1}\r\nPoint2: {point2}\r\nPoint3: {point3} \n err: {err}";
            Assert.IsTrue(actual, message);
        }
    }
}