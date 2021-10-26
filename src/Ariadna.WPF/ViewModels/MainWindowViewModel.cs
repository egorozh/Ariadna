using Ariadna.Core;
using MahApps.Metro.Controls.Dialogs;

namespace Ariadna;

internal class MainWindowViewModel : BaseViewModel
{
    #region Public Properties

    public AriadnaApp AriadnaApp { get; }
    public IThemeAndAccentManager ThemeManager { get; }
    public IUiManager UiManager { get; }

    #endregion

    #region Constructor

    public MainWindowViewModel(AriadnaApp ariadnaApp, IThemeAndAccentManager themeManager,
        IUiManager uiManager)
    {
        AriadnaApp = ariadnaApp;
        ThemeManager = themeManager;
        UiManager = uiManager;
    }

    #endregion

    #region Public Methods
    
    public bool Close() => AriadnaApp.Close();

    public void Closed() => AriadnaApp.ClosedApp();

    public async Task<MessageDialogResult> ShowMessageBoxAsync(string title, string message,
        MessageDialogStyle style, object? context)
    {   
        MetroDialogSettings settings = new()
        {
            NegativeButtonText = "Отмена"
        };

        if (style == MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary)
        {
            settings.AffirmativeButtonText = "Да";
            settings.FirstAuxiliaryButtonText = "Нет";
        }

        var showDialogResult = await DialogCoordinator.Instance.ShowMessageAsync(context ?? this, title, message,
            style, settings);

        return  showDialogResult;
    }

    #endregion
}