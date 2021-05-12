namespace Ariadna.Settings.QuickActions.Views
{
    internal partial class QuickActionsSettingsControl
    {
        public QuickActionsSettingsControl(QuickActionsSettingsViewModel quickActionsSettingsViewModel)
        {
            InitializeComponent();

            DataContext = quickActionsSettingsViewModel;
        }
    }
}