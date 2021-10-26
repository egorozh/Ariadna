using System.Windows;

namespace Ariadna;

internal partial class MultiProjectView
{
    private readonly MultiProjectViewModel _multiProjectViewModel;

    public MultiProjectView(MultiProjectViewModel multiProjectViewModel, AriadnaApp ariadnaApp)
    {
        _multiProjectViewModel = multiProjectViewModel;

        InitializeComponent();

        multiProjectViewModel.Init(DockManager);

        DataContext = multiProjectViewModel;

        Loaded += MainWindow_Loaded;
        Unloaded += MainWindow_Unloaded;

        ariadnaApp.Started += AriadnaApp_Started;
    }

    private void AriadnaApp_Started(object? sender, System.EventArgs e)
    {
        _multiProjectViewModel.Load();
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