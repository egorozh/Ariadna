using System.Windows;
using System.Windows.Input;

namespace Ariadna;

public interface IInterfaceHelper
{
    object CreateToolTip(IInterfaceFeature feature, List<UiHelpVideo>? videos,
        List<UiKeyBinding>? hotKeys, string? header, string? description, string? disableReason);

    FrameworkElement GetIcon(List<UiIcon> icons, ICommandFeature commandFeature);

    (FrameworkElement, FrameworkElement) GetIcon(List<UiIcon> icons, ITwoStateCommandFeature commandFeature);

    KeyBinding? CreateKeyBinding(List<UiKeyBinding> hotKeys, ICommandFeature commandFeature);

    FrameworkElement GetIcon(string iconFilePath);

    FrameworkElement GetIcon(string relativeIconPath, FrameworkElement defaultIcon);
}