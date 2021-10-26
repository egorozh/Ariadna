using Ariadna.Core;
using MahApps.Metro.Controls.Dialogs;

namespace Ariadna;

public abstract class BaseDialogViewModel : BaseViewModel
{
    public async Task<MessageDialogResult> ShowMessageBoxAsync(string title, string message,
        MessageDialogStyle style)
    {
        MetroDialogSettings settings = new()
        {
            NegativeButtonText = "Отмена"
        };

        var showDialogResult = await DialogCoordinator.Instance.ShowMessageAsync(this, title, message, style, settings);

        return showDialogResult;
    }
}