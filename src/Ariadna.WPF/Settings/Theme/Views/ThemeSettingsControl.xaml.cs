namespace Ariadna.Settings;

internal partial class ThemeSettingsControl
{
    public ThemeSettingsControl(ThemeSettingsViewModel themeSettingsViewModel)
    {
        InitializeComponent();

        DataContext = themeSettingsViewModel;
    }
}