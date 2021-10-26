namespace Ariadna.Settings;

internal class MenuSettings : BaseSettings
{
    private readonly IUiManager _uiManager;
    private readonly IReadOnlyList<IFeature> _features;

    #region Private Fields

    private MenuSettingsViewModel _vm;

    #endregion

    #region Constructor

    public MenuSettings(IUiManager uiManager,
        IReadOnlyList<IFeature> features)
    {
        _uiManager = uiManager;
        _features = features;
    }

    #endregion

    #region Public Methods

    public override string ToString() => "Элементы меню";

    public override void Init()
    {
        _vm = new MenuSettingsViewModel(this, _uiManager, _features);

        View = new MenuSettingsControl(_vm);
    }

    public override void Accept()
    {
        base.Accept();

        _vm.Accept();
    }

    public override void Cancel()
    {
        base.Cancel();

        _vm.Cancel();
    }

    #endregion

    #region Protected Methods

    protected override string CreateId() => "6bf38633-7b37-4f4a-9fae-3874527da5a2";

    #endregion
}