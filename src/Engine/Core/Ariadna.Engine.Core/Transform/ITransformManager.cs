using System;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Инструмент визуального трансформирования элементов(а)
    /// </summary>
    public interface ITransformManager : IEngineComponent
    {
        /// <summary>
        /// Трансформация завершена
        /// </summary>
        event EventHandler<TransformActionEventArgs> TransformEnded;

        void UpdateManipulator();
    }
}