using System.Windows;
using System.Windows.Input;

namespace Ariadna;

internal class ExitFeature : CommandFeature
{
    public ExitFeature(AriadnaApp ariadnaApp) : base(ariadnaApp)
    {
    }

    #region Public Methods

    public override string ToString() => "Выйти из программы";

    public override DefaultMenuProperties GetDefaultMenuProperties() => new()
    {
        Header = "Выход"
    };

    public override DefaultRibbonProperties GetDefaultRibbonProperties() => new()
    {
        Header = "Выход"
    };

    public override KeyBinding? GetDefaultKeyBinding() => new(this, Key.F4, ModifierKeys.Alt);

    #endregion

    protected override void Execute() => Application.Current.Shutdown();

    protected override string CreateId() => "c25d0f42-686d-4f66-aa71-75c77f067f03";
}