using System;
using System.Windows;

namespace AKIM.Engine.TwoD.Rotation
{
    /// <summary>
    /// Рабочее пространство, где происходит редактирование геометрии
    /// </summary>
    internal interface IRotationWorkspace : IDisposable
    {
        double GetRotationAngle();
        Point GetRotationCenter();

        bool CanAccessRotating();
    }
}
