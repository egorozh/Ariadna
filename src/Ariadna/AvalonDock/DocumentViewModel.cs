using System;

namespace Ariadna
{
    public abstract class DocumentViewModel : PaneViewModel, IDocumentViewModel
    {
        public virtual void Dispose()
        {
        }
    }

    public interface IDocumentViewModel : IPaneViewModel, IDisposable
    {
    }
}