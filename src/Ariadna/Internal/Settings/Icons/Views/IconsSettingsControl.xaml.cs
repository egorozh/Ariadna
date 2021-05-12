namespace Ariadna.Settings.Icons.Views
{
    internal partial class IconsSettingsControl
    {
        public IconsSettingsControl(IconsSettingsViewModel iconsSettingsViewModel)
        {
            InitializeComponent();

            DataContext = iconsSettingsViewModel;
        }
    }
}