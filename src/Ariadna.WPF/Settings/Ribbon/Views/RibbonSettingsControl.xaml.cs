namespace Ariadna.Settings;

internal partial class RibbonSettingsControl
{
    public RibbonSettingsControl(RibbonSettingsViewModel ribbonSettingsViewModel)
    {
        InitializeComponent();

        DataContext = ribbonSettingsViewModel;
    }
}