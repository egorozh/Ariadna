namespace Ariadna;

internal class ShowRibbonFeature : ToggleCommandFeature
{
    private readonly IUiOptions _magicOptions;
    private readonly IUiManager _uiManager;
    private const string ConfigKey = "ShowRibbon";

    public ShowRibbonFeature(AriadnaApp ariadnaApp, IUiOptions magicOptions,
        IUiManager uiManager) : base(ariadnaApp)
    {
        _magicOptions = magicOptions;
        _uiManager = uiManager;
        IsPressed = magicOptions.IsShowRibbon;

        ariadnaApp.Started += AriadnaApp_Started;
    }

    private void AriadnaApp_Started(object? sender, EventArgs e)
    {
        _uiManager.RibbonManager.Visible = IsPressed;
    }

    public override string ToString() => "Отображать ленту";

    protected override string CreateId() => "c31f36db-3654-4f75-ad2a-f3738455deec";

    public override DefaultMenuProperties GetDefaultMenuProperties() => new()
    {
        Header = "Коммандная лента"
    };

    protected override bool Unchecked()
    {
        _uiManager.RibbonManager.Visible = false;
        _magicOptions.IsShowRibbon = false;

        return false;
    }

    protected override void Checked()
    {
        _uiManager.RibbonManager.Visible = true;
        _magicOptions.IsShowRibbon = true;
    }
}