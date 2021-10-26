namespace Ariadna;

public interface IToolViewModel : IPaneViewModel
{
    bool IsVisible { get; set; }
    bool IsEnabled { get; set; }

    string Name { get; }
}