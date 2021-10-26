using Ariadna.Core;

namespace Ariadna;

public class AriadnaApp : BaseViewModel
{
    public void CatchUnhandledException(Exception exception)
    {
        //    Logger.Error(e.Message);
        //    await AriadnaApp.ShowMessageBoxAsync("Ошибка", "Что-то пошло не так. Обратитесь к разработчикам!");
    }
}