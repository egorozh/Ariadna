using System;
using System.Windows;

namespace AKIM.Engine.TwoD.Reflection
{
    /// <summary>
    /// Рабочее пространство, где происходит редактирование геометрии
    /// </summary>
    internal interface IReflectionWorkspace : IDisposable
    {
        double GetReflectionAngle();
        Point GetReflectionCenter();

        bool CanAccessReflecting();
    }
}
