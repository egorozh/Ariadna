namespace Ariadna.Settings;

internal class QuickActionsSettings : BaseSettings
{
    #region Private Fields

    private readonly IUiManager _uiManager;
    private readonly IReadOnlyList<IFeature> _features;

    private QuickActionsSettingsViewModel _vm;

    #endregion

    #region Constructor

    public QuickActionsSettings(IUiManager uiManager,
        IReadOnlyList<IFeature> features)
    {
        _uiManager = uiManager;
        _features = features;
    }

    #endregion

    #region Public Methods

    public override string ToString() => "Кнопки панелей быстрого доступа";

    public override void Init()
    {
        _vm = new QuickActionsSettingsViewModel(this, _uiManager, _features);

        View = new QuickActionsSettingsControl(_vm);
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

    protected override string CreateId() => "8002c8d3-42a8-4110-9ec7-dcd062ef3c3f";

    #endregion
}