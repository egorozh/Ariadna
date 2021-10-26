using Fluent;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Windows;

namespace Ariadna;

internal partial class MainWindow
{
    #region Private Fields

    private readonly MainWindowViewModel _mainWindowViewModel;

    #endregion

    #region TitelBar

    /// <summary>
    /// Gets ribbon titlebar
    /// </summary>
    public RibbonTitleBar TitleBar
    {
        get => (RibbonTitleBar) this.GetValue(TitleBarProperty);
        private set => this.SetValue(TitleBarPropertyKey, value);
    }

    // ReSharper disable once InconsistentNaming
    private static readonly DependencyPropertyKey TitleBarPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(RibbonTitleBar), typeof(MainWindow),
            new PropertyMetadata());

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="TitleBar"/>.
    /// </summary>
    public static readonly DependencyProperty TitleBarProperty = TitleBarPropertyKey.DependencyProperty;

    #endregion

    #region Constructor

    public MainWindow(MainWindowViewModel mainWindowViewModel, IReadOnlyList<ICustomThemeManager> themes)
    {
        _mainWindowViewModel = mainWindowViewModel;

        foreach (var theme in themes)
            theme.Init();

        InitializeComponent();

        DataContext = mainWindowViewModel;

        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
        Closed += MainWindow_Closed;
    }

    #endregion

    #region Private Methods

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        TitleBar = this.FindChild<RibbonTitleBar>("RibbonTitleBar");

        if (TitleBar != null)
        {
            TitleBar.HeaderAlignment = HorizontalAlignment.Left;
            TitleBar.Margin = new Thickness(60, 0, 0, 0);
            TitleBar.InvalidateArrange();
            TitleBar.UpdateLayout();
        }
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _mainWindowViewModel.Closed();
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        var res = _mainWindowViewModel.Close();

        e.Cancel = res;
    }

    #endregion
}