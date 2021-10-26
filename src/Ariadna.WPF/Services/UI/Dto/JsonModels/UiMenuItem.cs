namespace Ariadna;

public class UiMenuItem
{
    public string Header { get; set; }

    public List<UiMenuItem> Children { get; set; } = new();

    public string Id { get; set; }
}