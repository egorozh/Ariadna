namespace Ariadna.Settings;

internal partial class IconsSettingsControl
{
    public IconsSettingsControl(IconsSettingsViewModel iconsSettingsViewModel)
    {
        InitializeComponent();

        DataContext = iconsSettingsViewModel;
    }
}