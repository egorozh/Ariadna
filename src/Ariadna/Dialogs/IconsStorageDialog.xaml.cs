using System.Windows;

namespace Ariadna
{
    public partial class IconsStorageDialog
    {
        private readonly IconsStorageViewModel _vm;

        public IconsStorageDialog(AriadnaApp akimApp)
        {
            InitializeComponent();  

            Owner = Application.Current.MainWindow;
            ShowInTaskbar = false;

            _vm = new IconsStorageViewModel(this, akimApp);
            DataContext = _vm;
        }

        public string GetIconPath() => _vm.Load();
    }
}