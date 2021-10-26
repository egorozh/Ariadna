namespace Ariadna.Settings;

internal partial class HotKeysSettingsControl
{
    public HotKeysSettingsControl(HotKeysSettingsViewModel hotKeysSettingsViewModel)
    {
        InitializeComponent();

        DataContext = hotKeysSettingsViewModel;
    }
}