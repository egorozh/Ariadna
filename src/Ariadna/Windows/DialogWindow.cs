using MahApps.Metro.Controls;
using System.Windows;

namespace Ariadna
{
    public class DialogWindow : MetroWindow
    {
        static DialogWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogWindow),
                new FrameworkPropertyMetadata(typeof(DialogWindow)));
        }

        public DialogWindow()
        {
            Owner = Application.Current.MainWindow;
            ShowInTaskbar = false;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            IconTemplate = AriadnaApp.Instance.IconTemplate;

            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.ToolWindow;
        }
    }
}