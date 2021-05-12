using System.Windows;

namespace Ariadna
{
    internal partial class SettingsDialog
    {
        public SettingsDialog()
        {
            InitializeComponent();
            
            Owner = Application.Current.MainWindow;
            ShowInTaskbar = false;
        }
    }
}
