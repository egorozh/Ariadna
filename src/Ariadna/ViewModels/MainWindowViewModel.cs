using Ariadna.Core;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace Ariadna
{
    internal class MainWindowViewModel : BaseViewModel
    {
        #region Public Properties

        public AriadnaApp AriadnaApp { get; }
        public IThemeAndAccentManager ThemeManager { get; }
        public IUiManager UiManager { get; }

        #endregion

        #region Constructor

        public MainWindowViewModel(AriadnaApp ariadnaApp, IThemeAndAccentManager themeManager, IUiManager uiManager)
        {
            AriadnaApp = ariadnaApp;
            ThemeManager = themeManager;
            UiManager = uiManager;
        }

        #endregion

        #region Public Methods

        public async Task LoadAsync() => await AriadnaApp.LoadAsync();

        public bool Close() => AriadnaApp.Close();

        public void Closed() => AriadnaApp.ClosedApp();

        public async Task<AriadnaMessageDialogResult> ShowMessageBoxAsync(string title, string message,
            AriadnaMessageDialogStyle style)
        {
            MetroDialogSettings settings = new()
            {
                NegativeButtonText = "Отмена"
            };

            if (style == AriadnaMessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary)
            {
                settings.AffirmativeButtonText = "Да";
                settings.FirstAuxiliaryButtonText = "Нет";
            }

            var showDialogResult = await DialogCoordinator.Instance.ShowMessageAsync(this, title, message,
                (MessageDialogStyle) style, settings);

            return (AriadnaMessageDialogResult) showDialogResult;
        }

        #endregion
    }
}