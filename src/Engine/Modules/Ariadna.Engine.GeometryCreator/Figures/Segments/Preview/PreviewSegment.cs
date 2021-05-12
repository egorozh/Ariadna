using System;
using System.Windows;
using System.Windows.Media;

namespace Ariadna.Engine.GeometryCreator
{
    internal abstract class PreviewSegment : IDisposable
    {
        public abstract void Dispose();
        public abstract void UpdatePoint(Point newPoint);

        public abstract void Update();
        public abstract PathSegment GetSegment(bool isFlip = false);
        public Point EndPoint { get; protected set; }
    }
}