namespace Ariadna;

public abstract class ToolViewModel : PaneViewModel, IToolViewModel
{
    public bool IsVisible { get; set; }
    public bool IsEnabled { get; set; } = true;

    public virtual string Name { get; }

    protected ToolViewModel(string name) : base(name)
    {
        Name = name;
    }
}