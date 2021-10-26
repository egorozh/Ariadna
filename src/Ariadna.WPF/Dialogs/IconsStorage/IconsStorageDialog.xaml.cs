using System.Windows;

namespace Ariadna;

public partial class IconsStorageDialog
{
    private readonly IconsStorageViewModel _vm;

    public IconsStorageDialog(IInterfaceHelper interfaceHelper,
        IStorage storage,
        IImageHelpers imageHelpers)
    {
        InitializeComponent();

        Owner = Application.Current.MainWindow;
        ShowInTaskbar = false;

        _vm = new IconsStorageViewModel(this, interfaceHelper, storage, imageHelpers);
        DataContext = _vm;
    }

    public string GetIconPath() => _vm.Load();
}