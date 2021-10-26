namespace Ariadna;

internal class ShowLeftActionsPanelFeature : ToggleCommandFeature
{
    private readonly IUiManager _uiManager;

    public ShowLeftActionsPanelFeature(AriadnaApp ariadnaApp, IUiManager uiManager) : base(ariadnaApp)
    {
        _uiManager = uiManager;
        AriadnaApp.Started += AkimApp_Started;
    }

    private void AkimApp_Started(object? sender, EventArgs e)
    {
        IsPressed = _uiManager.JsonInterface?.QuickActions.IsShowLeft ?? false;
    }

    public override string ToString() => "Базовые => Отображать левую панель быстрого доступа";

    protected override string CreateId() => "404cd3f2-9a58-4a29-b06d-528bf871eb64";

    public override DefaultMenuProperties GetDefaultMenuProperties() => new()
    {
        Header = "Левая панель б.д."
    };

    protected override bool Unchecked()
    {
        _uiManager.QuickActionsManager.IsShowLeft = false;
        return false;
    }

    protected override void Checked()
    {
        _uiManager.QuickActionsManager.IsShowLeft = true;
    }
}