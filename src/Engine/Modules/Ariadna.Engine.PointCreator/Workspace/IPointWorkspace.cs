using System;
using System.Windows;

namespace Ariadna.Engine.PointCreator
{
    /// <summary>
    /// Рабочее пространство, где происходит редактирование геометрии
    /// </summary>
    internal interface IPointWorkspace : IDisposable
    {
        bool CanAccessCreating();
        Point GetPoint();
    }
}
