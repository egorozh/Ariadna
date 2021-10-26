using Ariadna.Core;

namespace Ariadna;

public class AriadnaApp : BaseViewModel
{
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