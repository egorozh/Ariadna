using System;
using System.Windows.Media;

namespace Ariadna.Engine.Transformation
{
    internal class ManipulateEventArgs : EventArgs
    {
        public Matrix Matrix { get; }

        public ManipulateEventArgs(Matrix matrix) => Matrix = matrix;
    }
}