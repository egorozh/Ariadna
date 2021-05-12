using System;

namespace Ariadna.Engine.Core
{
    /// <summary>
    /// Содержит данные события <see cref="ICoordinateSystem.CoordinateChanged"/>
    /// </summary>
    public class CoordinateChangedArgs : EventArgs
    {
        /// <summary>
        /// Текущее значение <see cref="ICoordinateSystem.DeltaX"/>
        /// </summary>
        public double DeltaX { get; }

        /// <summary>
        /// Текущее значение <see cref="ICoordinateSystem.DeltaY"/>
        /// </summary>
        public double DeltaY { get; }

        /// <summary>
        /// Текущее значение <see cref="ICoordinateSystem.Resolution"/>
        /// </summary>
        public decimal Resolution { get; }

        /// <summary>
        /// Новое значение <see cref="ICoordinateSystem.Angle"/>
        /// </summary>
        public double NewAngle { get; }

        /// <summary>
        /// Предыдущее значение <see cref="ICoordinateSystem.Angle"/>
        /// </summary>
        public double OldAngle { get; set; }

        /// <summary>
        /// Дефолтный конструктор
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        /// <param name="resolution"></param>
        /// <param name="newAngle"></param>
        public CoordinateChangedArgs(double deltaX, double deltaY, decimal resolution, double newAngle, double oldAngle)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
            Resolution = resolution;
            NewAngle = newAngle;
            OldAngle = oldAngle;
        }
    }
}