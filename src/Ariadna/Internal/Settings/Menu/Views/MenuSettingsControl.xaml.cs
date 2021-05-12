namespace Ariadna
{
    internal partial class MenuSettingsControl
    {
        public MenuSettingsControl(MenuSettingsViewModel menuSettingsViewModel)
        {
            InitializeComponent();

            DataContext = menuSettingsViewModel;
        }
    }
}