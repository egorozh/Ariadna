using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Ariadna.Engine.Core
{
    public class TransformActionEventArgs : EventArgs
    {
        public Matrix Transform { get; }
        public ICollection<ISelectedFigure2D> Figures { get; }

        public TransformActionEventArgs(Matrix transform, ICollection<ISelectedFigure2D> figures)
        {
            Transform = transform;
            Figures = figures;
        }
    }
}