using System.Windows;

namespace Ariadna;

public class SettingsItemViewModel : SettingsItemContainerViewModel
{
    #region Private Methods

    private readonly ISettings _setting;

    #endregion

    #region Public Properties

    public override FrameworkElement? View { get; }

    public bool HasChanges => _setting.HasChanges;

    #endregion

    #region Events

    public event EventHandler? HasChangesChanged;

    #endregion

    #region Constructor

    public SettingsItemViewModel(ISettings setting, string name) : base(name)
    {
        _setting = setting;
        View = setting.View;

        _setting.HasChangesChanged += Setting_HasChangesChanged;
    }

    #endregion

    #region Public Methods

    public void Accept()
    {
        _setting.Accept();
    }
        
    public void Cancelled()
    {
        _setting.Cancel();
    }
        
    #endregion

    #region Private Methods

    private void Setting_HasChangesChanged(object? sender, EventArgs e)
    {
        HasChangesChanged?.Invoke(sender, e);
    }

    #endregion
}