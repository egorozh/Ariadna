using System.Windows;
using System.Windows.Input;

namespace Ariadna;

public interface ICommandFeature : IInterfaceFeature, ICommand
{
    FrameworkElement? GetDefaultIcon();
    KeyBinding? GetDefaultKeyBinding();
    void Update();  
}