using System.Windows;
using System.Windows.Input;

namespace Ariadna;

public abstract class CommandFeature : InterfaceFeature, ICommandFeature
{
    #region Protected Properties

    protected AriadnaApp AriadnaApp { get; }

    #endregion

    #region Events

    public event EventHandler? CanExecuteChanged;

    #endregion

    #region Constructor

    protected CommandFeature(AriadnaApp ariadnaApp)
    {
        AriadnaApp = ariadnaApp;
    }

    #endregion

    #region Public Methods

    public bool CanExecute(object? parameter)
    {
#if DEBUG
        return CanExecute();
#endif
#if RELEASE
        try
        {
            return CanExecute();
        }
        catch (Exception e)
        {
            AriadnaApp.CatchUnhandledException(e);

            return false;
        }
#endif
    }

    public void Execute(object? parameter)
    {
#if DEBUG
        Execute();
#endif
#if RELEASE
        try
        {
            Execute();
        }
        catch (Exception e)
        {
            AriadnaApp.CatchUnhandledException(e);
        }
#endif
    }

    public FrameworkElement? GetDefaultIcon() => null;

    public KeyBinding? GetDefaultKeyBinding() => null;

    public void Update()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Protected Methods

    protected virtual bool CanExecute() => true;

    protected virtual void Execute()
    {
    }

    #endregion
}