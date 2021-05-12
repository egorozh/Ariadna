namespace Ariadna.Settings.Ribbon.Views
{
    internal partial class RibbonSettingsControl
    {
        public RibbonSettingsControl(RibbonSettingsViewModel ribbonSettingsViewModel)
        {
            InitializeComponent();

            DataContext = ribbonSettingsViewModel;
        }
    }
}