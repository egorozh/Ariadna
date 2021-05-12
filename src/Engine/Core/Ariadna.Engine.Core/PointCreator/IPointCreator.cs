using System;
using System.Windows;

namespace Ariadna.Engine.Core
{
    public interface IPointCreator : IEngineComponent
    {
        #region Properties
        
        /// <summary>
        /// Конструктор активен 
        /// </summary>
        bool IsCreating { get; }

        #endregion

        #region Events

        /// <summary>
        /// Происходит по окончании редактирования
        /// </summary>
        event EventHandler<PointGeometryResultArgs> EndCreated;

        /// <summary>
        /// Происходит при начале и окончании редактирования
        /// </summary>
        event EventHandler<IsPointCreatedEventArgs> IsCreatedChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Начало создания точки
        /// </summary>
        void StartCreating();

        #endregion
    }

    public class IsPointCreatedEventArgs
    {
        public bool IsCreated { get; }

        public IsPointCreatedEventArgs(bool isCreated)
        {
            IsCreated = isCreated;
        }
    }

    public class PointGeometryResultArgs : EventArgs
    {
        public Point Point { get; }

        public bool IsCancel { get; }

        public PointGeometryResultArgs(Point point, bool isCancelled = false)
        {
            Point = point;
            IsCancel = isCancelled;
        }
    }
}

