namespace Ariadna;

public interface IToggleCommandFeature : ICommandFeature
{
    event EventHandler IsPressedChanged;

    bool IsPressed { get; set; }
}