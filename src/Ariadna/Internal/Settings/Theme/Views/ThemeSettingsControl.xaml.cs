namespace Ariadna.Settings.Theme.Views
{
    internal partial class ThemeSettingsControl
    {
        public ThemeSettingsControl(ThemeSettingsViewModel themeSettingsViewModel)
        {
            InitializeComponent();

            DataContext = themeSettingsViewModel;
        }
    }
}