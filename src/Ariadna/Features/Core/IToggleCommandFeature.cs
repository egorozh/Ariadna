using System;

namespace Ariadna
{
    /// <summary>
    /// Базовый класс функциональности в виде команды-переключателя
    /// </summary>
    public interface IToggleCommandFeature : ICommandFeature
    {
        event EventHandler IsPressedChanged;
        
        bool IsPressed { get; set; }
    }
}