using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using AKIM.Undo;

namespace Ariadna.Engine.Core
{
    public interface IRotationManager : IEngineComponent
    {
        #region Properties

        /// <summary>
        /// Action Manager
        /// </summary>
        IActionManager ActionManager { get; }

        /// <summary>
        /// Конструктор активен 
        /// </summary>
        bool IsRotating { get; }

        #endregion

        #region Events

        /// <summary>
        /// Происходит по окончании редактирования
        /// </summary>
        event EventHandler<RotationGeometryResultArgs> RotationEnded;

        #endregion

        #region Methods

        /// <summary>
        /// Начало поворота фигур
        /// </summary>
        void StartRotating(List<PathGeometry> geometries);

        /// <summary>
        /// Принять поворот фигур
        /// </summary>
        /// <returns></returns>
        void AccessRotating();

        /// <summary>
        /// Отмена поворота фигур
        /// </summary>
        void CancelRotating();

        #endregion
    }

    public class RotationGeometryResultArgs : EventArgs
    {
        public double Angle { get; }

        public Point Center { get; }

        public RotationGeometryResultArgs(double angle, Point center)
        {
            Angle = angle;
            Center = center;
        }
    }
}