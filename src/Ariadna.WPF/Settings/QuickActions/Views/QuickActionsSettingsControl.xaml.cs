namespace Ariadna.Settings;

internal partial class QuickActionsSettingsControl
{
    public QuickActionsSettingsControl(QuickActionsSettingsViewModel quickActionsSettingsViewModel)
    {
        InitializeComponent();

        DataContext = quickActionsSettingsViewModel;
    }
}