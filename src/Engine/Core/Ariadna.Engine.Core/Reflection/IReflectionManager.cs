using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using AKIM.Undo;

namespace Ariadna.Engine.Core
{
    public interface IReflectionManager : IEngineComponent
    {
        #region Properties

        /// <summary>
        /// Action Manager
        /// </summary>
        IActionManager ActionManager { get; }

        /// <summary>
        /// Конструктор активен 
        /// </summary>
        bool IsReflecting { get; }

        #endregion

        #region Events

        /// <summary>
        /// Происходит по окончании редактирования
        /// </summary>
        event EventHandler<ReflectionGeometryResultArgs> ReflectionEnded;

        #endregion

        #region Methods

        /// <summary>
        /// Начало поворота фигур
        /// </summary>
        void StartReflecting(List<PathGeometry> geometries);

        /// <summary>
        /// Принять поворот фигур
        /// </summary>
        /// <returns></returns>
        void AccessReflecting();

        /// <summary>
        /// Отмена поворота фигур
        /// </summary>
        void CancelReflecting();

        #endregion
    }

    public class ReflectionGeometryResultArgs : EventArgs
    {
        public double Angle { get; }

        public Point Center { get; }

        public ReflectionGeometryResultArgs(double angle, Point center)
        {
            Angle = angle;
            Center = center;
        }
    }
}