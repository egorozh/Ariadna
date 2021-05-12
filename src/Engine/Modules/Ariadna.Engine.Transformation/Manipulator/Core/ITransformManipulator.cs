using System;
using Ariadna.Engine.Core;

namespace Ariadna.Engine.Transformation
{
    internal interface ITransformManipulator : IDisposable
    {
        event EventHandler<TransformActionEventArgs> TransformEnded;
        
        void Update();
    }
}