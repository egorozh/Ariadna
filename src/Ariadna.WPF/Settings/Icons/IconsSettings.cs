namespace Ariadna.Settings;

internal class IconsSettings : BaseSettings
{
    #region Private Fields

    private readonly IUiManager _uiManager;
    private readonly IReadOnlyList<IFeature> _features;
    private readonly IStorage _storage;
    private readonly IInterfaceHelper _interfaceHelper;
    private readonly IImageHelpers _imageHelpers;

    private IconsSettingsViewModel _vm;

    #endregion

    #region Constructor

    public IconsSettings(IUiManager uiManager,
        IReadOnlyList<IFeature> features,
        IStorage storage,
        IInterfaceHelper interfaceHelper,
        IImageHelpers imageHelpers) 
    {
        _uiManager = uiManager;
        _features = features;
        _storage = storage;
        _interfaceHelper = interfaceHelper;
        _imageHelpers = imageHelpers;
    }

    #endregion

    #region Public Methods

    public override string ToString() => "Значки";

    public override void Init()
    {
        _vm = new IconsSettingsViewModel(_uiManager, _features, _storage,
            _interfaceHelper, _imageHelpers, this);

        View = new IconsSettingsControl(_vm);
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

    protected override string CreateId() => "7ad7d3d7-e2ed-4dec-971c-dc738d14fd64";

    #endregion
}