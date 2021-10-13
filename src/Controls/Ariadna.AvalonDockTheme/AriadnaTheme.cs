using AvalonDock.Themes;
using System;

namespace Ariadna.AvalonDockTheme;

public class AriadnaTheme : Theme
{
    public override Uri GetResourceUri() =>
        new ("/Ariadna.AvalonDockTheme;component/Resources/Theme.xaml", UriKind.Relative);
}