using System.Collections.Specialized;
using System.Windows;

namespace Ariadna;

public interface IMultiProjectApp<TDoc, TTool>
    where TDoc : class, IDocumentViewModel
    where TTool : IToolViewModel
{
    AriadnaApp AriadnaApp { get; }
    ICollection<TTool> Tools { get; }
    ICollection<TDoc> Projects { get; }
    TDoc? CurrentProject { get; set; }

    event EventHandler CurrentProjectChanged;
    event EventHandler<NotifyCollectionChangedEventArgs> ProjectsChanged;

    DataTemplate? GetDocumentHeaderTemplate();
}