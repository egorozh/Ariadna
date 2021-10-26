namespace Ariadna;

public abstract class DocumentViewModel : PaneViewModel, IDocumentViewModel
{
    public virtual void Dispose()
    {
    }

    protected DocumentViewModel(string title) : base(title)
    {
    }
}