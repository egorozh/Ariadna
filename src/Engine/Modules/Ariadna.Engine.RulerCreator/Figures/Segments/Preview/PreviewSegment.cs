using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Ariadna.Engine.Core;

namespace AKIM.Engine.TwoD.RulerCreator
{
    internal abstract class PreviewSegment : IDisposable
    {
        /// <summary>
        /// Длина сегмента
        /// </summary>
        public double Length => GetLength();

        /// <summary>
        /// Текст-блок для отображения длины сегмента
        /// </summary>
        public TextBlock LengthTextBlock { get; set; } = new TextBlock();

        public Point EndPoint { get; protected set; }


        public Point PrevPoint;
        protected RulerWorkspace Workspace;

        public Shape EditEndPoint;

        protected PreviewSegment(RulerWorkspace workspace)
        {
            Workspace = workspace;

            LengthTextBlock.SetZIndex(ZOrder.RulerTextBox);
        }

        public virtual void Update()
        {
            LengthTextBlock.Text = Length.ToStr();

            LengthTextBlock.SetTextBlockOnCenter(PrevPoint, GetCenterPoint(), EndPoint,
                Workspace.RulerCreator.CoordinateSystem);
        }

        public virtual Point GetCenterPoint()
        {
            var point1 = PrevPoint;
            var point2 = EndPoint;

            return new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
        }

        public abstract double GetLength();


        public abstract PathSegment GetSegment(bool isFlip = false);

        public abstract void Dispose();
        public abstract void UpdatePoint(Point newPoint);
    }
}