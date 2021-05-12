using System;
using AvalonDock.Themes;

namespace MagicWpf.AvalonDockTheme
{
    public class MagicWpfTheme : Theme
    {
        public override Uri GetResourceUri() =>
            new ("/MagicWpf.AvalonDockTheme;component/Resources/Theme.xaml", UriKind.Relative);
    }
}