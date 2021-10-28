using System.Windows;

namespace Ariadna;

internal partial class MultiProjectView
{
    private readonly MultiProjectViewModel _multiProjectViewModel;

    public MultiProjectView(MultiProjectViewModel multiProjectViewModel)
    {
        _multiProjectViewModel = multiProjectViewModel;

        InitializeComponent();

        multiProjectViewModel.Init(DockManager);

        DataContext = multiProjectViewModel;

        Loaded += MainWindow_Loaded;
        Unloaded += MainWindow_Unloaded;
    }
    
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _multiProjectViewModel.Load();
    }

    private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
    {
        _multiProjectViewModel.Unload();
    }
}