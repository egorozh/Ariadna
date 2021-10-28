using MahApps.Metro.Controls;
using System.Windows;

namespace Ariadna;

public class DialogWindow : MetroWindow
{
    public DialogWindow()
    {
        Owner = Application.Current.MainWindow;
        ShowInTaskbar = false;

        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        ResizeMode = ResizeMode.NoResize;
        WindowStyle = WindowStyle.ToolWindow;

        Style = Application.Current.FindResource(AriadnaResourceKeys.DialogWindow) as Style;
    }
}