using System;
using System.Windows.Media;

namespace Ariadna.Engine.Transformation
{
    internal interface IManipulatorFigure : IDisposable
    {
        event EventHandler<ManipulateEventArgs> Manipulate;

        event EventHandler<ManipulateEventArgs> ManipulateEnded;


        void Update(Matrix transform);
    }
}