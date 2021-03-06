namespace Ariadna;

internal class SettingsManager : ISettingsManager
{
    #region Private Fields

    private readonly SettingsDialogViewModel _settingsViewModel;
    private SettingsDialog? _settingsDialog;

    #endregion

    #region Constructor

    public SettingsManager()
    {
        _settingsViewModel = new SettingsDialogViewModel(this);
    }

    #endregion

    #region Public Methods

    public void Show()
    {
        if (_settingsDialog is {IsVisible: true})
            _settingsDialog.Close();

        _settingsDialog = new SettingsDialog()
        {
            DataContext = _settingsViewModel
        };

        _settingsDialog.ShowDialog();
    }

    public void Close()
    {
        _settingsDialog?.Close();
    }

    public void Clear() => _settingsViewModel.Clear();

    public void InitElements(List<UiSettingsItem> uiSettingsItems, IEnumerable<ISettings> settings)
        => _settingsViewModel.InitElements(uiSettingsItems, settings);

    #endregion
}