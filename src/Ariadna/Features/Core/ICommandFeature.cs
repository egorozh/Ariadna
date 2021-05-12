using System.Windows;
using System.Windows.Input;

namespace Ariadna
{
    /// <summary>
    /// Базовый класс функциональности в виде обычной команды 
    /// </summary>
    public interface ICommandFeature : IInterfaceFeature
    {
        ICommand Command { get; }
        KeyBinding KeyBinding { get; }
        
        FrameworkElement CreateDefaultIcon();
        void Update();
    }
}