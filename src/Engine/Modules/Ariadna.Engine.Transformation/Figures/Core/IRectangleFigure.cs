using System;
using System.Windows.Media;

namespace Ariadna.Engine.Transformation
{
    internal interface IRectangleFigure : IDisposable
    {
        event EventHandler<ManipulateEventArgs> Manipulate;

        event EventHandler<ManipulateEventArgs> ManipulateEnded;

        void Update();

        void ChangeTransform(Matrix transform);
    }
}