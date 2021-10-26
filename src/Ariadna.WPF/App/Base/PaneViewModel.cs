using Ariadna.Core;
using Prism.Commands;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ariadna;

public abstract class PaneViewModel : BaseViewModel, IPaneViewModel
{
    #region Public Properties

    public string Title { get; set; }

    public ImageSource IconSource { get; set; } = new BitmapImage();

    public string ContentId { get; set; }

    public bool IsSelected { get; set; }

    public bool IsActive { get; set; }

    #endregion

    #region Commands

    public ICommand CloseCommand { get; }

    #endregion

    #region Constructor

    protected PaneViewModel(string title)
    {
        Title = title;
        CloseCommand = new DelegateCommand(Close);
    }

    #endregion

    #region Public Methods

    public virtual void Close()
    {
    }

    public abstract DataTemplate GetTemplate();

    #endregion
}