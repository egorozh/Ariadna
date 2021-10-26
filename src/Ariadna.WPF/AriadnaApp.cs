using System.Windows.Controls;
using Ariadna.Core;

namespace Ariadna;

public class AriadnaApp : BaseViewModel
{
    public Button SettingsButton { get; internal set; }

    #region Events

    public event EventHandler<IReadOnlyCollection<string>>? NextInstanceRunned;

    public event EventHandler? Started;

    public event EventHandler? Closed;

    #endregion

    #region Public Methods

    public bool Close()
    {
        return false;
    }

    public void ClosedApp()
    {
    }

    #endregion


    #region Internal Methods

    internal void CatchUnhandledException(Exception exception)
    {
        //    Logger.Error(e.Message);
        //    await AriadnaApp.ShowMessageBoxAsync("Ошибка", "Что-то пошло не так. Обратитесь к разработчикам!");
    }

    #endregion
}