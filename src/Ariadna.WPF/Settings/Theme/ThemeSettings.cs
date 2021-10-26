namespace Ariadna.Settings;

internal class ThemeSettings : BaseSettings
{
    private readonly IThemeAndAccentManager _themeAndAccentManager;

    #region Private Fields

    private ThemeSettingsViewModel _vm;

    #endregion
        
    #region Constructor

    public ThemeSettings(IThemeAndAccentManager themeAndAccentManager) 
    {
        _themeAndAccentManager = themeAndAccentManager;
    }

    #endregion

    #region Public Methods

    public override string ToString() => "Темы";

    public override void Init()
    {
        _vm = new ThemeSettingsViewModel(_themeAndAccentManager, this);

        View = new ThemeSettingsControl(_vm);
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

    protected override string CreateId() => "3453e157-7667-4dd4-92fd-981a02dbddb6";

    #endregion
}