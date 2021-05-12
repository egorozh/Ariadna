namespace Ariadna.Settings.HotKeys.Views
{
    internal partial class HotKeysSettingsControl
    {
        public HotKeysSettingsControl(HotKeysSettingsViewModel hotKeysSettingsViewModel)
        {
            InitializeComponent();

            DataContext = hotKeysSettingsViewModel;
        }
    }
}