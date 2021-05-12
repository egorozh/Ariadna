using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using Ariadna.Core;

namespace Ariadna
{
    public abstract class BaseDialogViewModel : BaseViewModel
    {
        public async Task<AriadnaMessageDialogResult> ShowMessageBoxAsync(string title, string message,
            AriadnaMessageDialogStyle style)
        {
            MetroDialogSettings settings = new()
            {
                NegativeButtonText = "Отмена"
            };

            var showDialogResult = await DialogCoordinator.Instance.ShowMessageAsync(this, title, message,
                (MessageDialogStyle) style, settings);

            return (AriadnaMessageDialogResult) showDialogResult;
        }
    }
}