namespace Ariadna.Settings;

internal class HotKeysSettings : BaseSettings
{
    private readonly IInterfaceHelper _interfaceHelper;
    private readonly IUiManager _uiManager;
    private readonly IReadOnlyList<IFeature> _features;

    #region Private Fields

    private HotKeysSettingsViewModel _vm;

    #endregion

    #region Constructor

    public HotKeysSettings(IInterfaceHelper interfaceHelper,
        IUiManager uiManager, IReadOnlyList<IFeature> features)
    {
        _interfaceHelper = interfaceHelper;
        _uiManager = uiManager;
        _features = features;
    }

    #endregion

    #region Public Methods

    public override string ToString() => "Горячие клавиши";

    public override void Init()
    {
        _vm = new HotKeysSettingsViewModel(this, _interfaceHelper, _uiManager, _features);

        View = new HotKeysSettingsControl(_vm);
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

    protected override string CreateId() => "fc660ec8-c685-4c8f-bc69-3ddcabcf57cd";

    #endregion
}